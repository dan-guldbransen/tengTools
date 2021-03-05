using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Litium.Accelerator.Mailing;
using Litium.Globalization;
using Litium.Web;
using Litium.Scheduler;
using Litium.Websites;
using Litium.Foundation.Configuration;
using System.Net.Mail;
using Litium.Accelerator.Constants;
using Microsoft.Extensions.Logging;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Services
{
    internal class MailServiceImpl : MailService
    {
        private readonly MailServiceProcessor _mailServiceProcessor;
        private readonly UrlService _urlService;
        private readonly WebsiteService _websiteService;
        private readonly ChannelService _channelService;
        private readonly ILogger<MailService> _logger;
        private readonly SchedulerService _schedulerService;

        public MailServiceImpl(SchedulerService schedulerService,
            UrlService urlService,
            WebsiteService websiteService,
            ChannelService channelService,
            ILogger<MailService> logger,
            MailServiceProcessor mailServiceProcessor)
        {
            _schedulerService = schedulerService;
            _urlService = urlService;
            _websiteService = websiteService;
            _channelService = channelService;
            _logger = logger;
            _mailServiceProcessor = mailServiceProcessor;
        }

        public override void SendEmail(string mailSender, string toEmail, string subject, string body, bool htmlFormat, bool throwException)
        {
            if (string.IsNullOrEmpty(mailSender))
            {
                _logger.LogError("Sending e-mail failed. Mail sender was empty. Check mail configuration settings");
                if (throwException)
                {
                    throw new Exception("Mail sender was empty. Check mail configuration settings");
                }

                return;
            }

            _mailServiceProcessor.SendEmail(mailSender, toEmail, subject, body, htmlFormat, throwException);
        }

        public override void SendEmail(MailDefinition mail, bool throwException)
        {
            var channel = _channelService.Get(mail.ChannelSystemId);
            var mailSender = _websiteService.Get(channel.WebsiteSystemId.Value)?.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.SenderEmailAddress);

            switch (mail)
            {
                case PlainMailDefinition plainTextMail:
                    SendEmail(mailSender, plainTextMail.ToEmail, plainTextMail.Subject, plainTextMail.Body, false, throwException);
                    return;
                case HtmlMailDefinition htmlTextMail:
                    SendEmail(mailSender, htmlTextMail.ToEmail, htmlTextMail.Subject, htmlTextMail.Body, true, throwException);
                    return;
            }

            if (!(mail is PageMailDefinition pageMail))
            {
                return;
            }

            var page = pageMail.Page;
            var pageUrl = _urlService.GetUrl(page, new PageUrlArgs(pageMail.ChannelSystemId) { AbsoluteUrl = true });
            var url = pageMail.UrlTransform(pageUrl);

            var baseUrl = _urlService.GetUrl(channel, new ChannelUrlArgs { AbsoluteUrl = true });
            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
            {
                return;
            }

            if (string.IsNullOrEmpty(mailSender))
            {
                _logger.LogError("Sending e-mail failed. Mail sender was empty. Check mail configuration settings");
                if (throwException)
                {
                    throw new Exception("Mail sender was empty. Check mail configuration settings");
                }

                return;
            }

            var messageInfo = new MessageInfo
            {
                FromEmail = mailSender,
                ToEmail = pageMail.ToEmail,
                Subject = pageMail.Subject,
                Url = url,
                PageSystemId = page.SystemId,
                BaseUrl = baseUrl
            };
            _schedulerService.ScheduleJob<MailServiceProcessor>(x => x.Process(messageInfo));
        }

        internal class MessageInfo
        {
            public string BaseUrl { get; set; }
            public string Body { get; set; }
            public bool IsHtml { get; set; }
            public Guid PageSystemId { get; set; }
            public string Subject { get; set; }
            public string ToEmail { get; set; }
            public string Url { get; set; }
            public string FromEmail { get; set; }
        }

        [Service(ServiceType = typeof(MailServiceProcessor), Lifetime = DependencyLifetime.Singleton)]
        internal class MailServiceProcessor
        {
            private readonly ILogger<MailServiceProcessor> _logger;

            public MailServiceProcessor(ILogger<MailServiceProcessor> logger)
            {
                _logger = logger;
            }

            public async Task Process(MessageInfo message)
            {
                var data = message;
                if (data.Url == null)
                {
                    SendEmail(data.FromEmail, data.ToEmail, data.Subject, data.Body, data.IsHtml, false);
                }
                else
                {
                    SendEmail(data.FromEmail, data.ToEmail, data.Subject, data.Url, data.BaseUrl, false);
                }

                await Task.CompletedTask;
            }

            public virtual void SendEmail(string fromEmail, string toEmail, string subject, string body, bool htmlFormat, bool throwException)
            {
                try
                {
                    SendMail(fromEmail, toEmail, subject, body, htmlFormat);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    if (throwException)
                    {
                        throw;
                    }
                }
            }

            public virtual void SendEmail(string fromEmail, string toEmail, string subject, string url, string baseUrl, bool throwException)
            {
                // Get html content of page as email body
                var emailBody = GetWebPageContent(url);

                if (string.IsNullOrEmpty(emailBody))
                {
                    _logger.LogError($"Could not find email template at {url}");
                    if (throwException)
                    {
                        throw new Exception("Could not find email template at " + url);
                    }

                    return;
                }

                // Replace relative paths with absolute paths in email body
                if (!baseUrl.EndsWith("/"))
                {
                    baseUrl += "/";
                }

                emailBody = emailBody.Replace("href=\"/", "href=\"" + baseUrl);
                emailBody = emailBody.Replace("src=\"/", "src=\"" + baseUrl);

                SendEmail(fromEmail, toEmail, subject, emailBody, true, throwException);
            }

            /// <summary>
            /// Sends an email
            /// </summary>
            /// <param name="from">From address.</param>
            /// <param name="to">To address</param>
            /// <param name="subject">Subject</param>
            /// <param name="body">Body</param>
            /// <param name="isBodyHtml">Specifies wether the mail contains HTML or not</param>
            /// <exception cref="SmtpException">Thrown when there is an SMTP error.</exception>
            public void SendMail(string from, string to, string subject, string body, bool isBodyHtml)
            {
                SendMail(from, to, string.Empty, string.Empty, subject, body, isBodyHtml, SmtpDeliveryMethod.PickupDirectoryFromIis);
            }

            /// <summary>
            /// Sends the mail.
            /// </summary>
            /// <param name="from">From.</param>
            /// <param name="to">To.</param>
            /// <param name="cc">The cc.</param>
            /// <param name="bcc">The BCC.</param>
            /// <param name="subject">The subject.</param>
            /// <param name="body">The body.</param>
            /// <param name="isBodyHtml">Specifies wether the mail contains HTML or not</param>
            /// <exception cref="SmtpException">Thrown when there is an SMTP error.</exception>
            public void SendMail(string from, string to, string cc, string bcc, string subject, string body, bool isBodyHtml)
            {
                SendMail(from, to, cc, bcc, subject, body, isBodyHtml, SmtpDeliveryMethod.PickupDirectoryFromIis);
            }

            /// <summary>
            /// Sends an email
            /// </summary>
            /// <param name="from">From address.</param>
            /// <param name="to">To address</param>
            /// <param name="subject">Subject</param>
            /// <param name="body">Body</param>
            /// <param name="isBodyHtml">Specifies wether the mail contains HTML or not</param>
            /// <param name="deliveryMethod">The delivery method.</param>
            /// <exception cref="SmtpException">Thrown when there is an SMTP error.</exception>
            public void SendMail(string from, string to, string subject, string body, bool isBodyHtml, SmtpDeliveryMethod deliveryMethod)
            {
                SendMail(from, to, string.Empty, string.Empty, subject, body, isBodyHtml, deliveryMethod);
            }

            /// <summary>
            /// Sends an email
            /// </summary>
            /// <param name="from">From address.</param>
            /// <param name="to">To address</param>
            /// <param name="cc">CC address</param>
            /// <param name="bcc">BCC address</param>
            /// <param name="subject">Subject</param>
            /// <param name="body">Body</param>
            /// <param name="isBodyHtml">Specifies wether the mail contains HTML or not</param>
            /// <param name="deliveryMethod">The delivery method.</param>
            /// <exception cref="SmtpException">Thrown when there is an SMTP error.</exception>
            public void SendMail(string from, string to, string cc, string bcc, string subject, string body, bool isBodyHtml, SmtpDeliveryMethod deliveryMethod)
            {
                try
                {
                    var mailClient = new SmtpClient();

                    //Set mail client host to local host if empty
                    if (!string.IsNullOrEmpty(GeneralConfig.Instance.SMTPServer))
                    {
                        var server = GeneralConfig.Instance.SMTPServer.Split(':');
                        mailClient.Host = server[0];
                        if (server.Length > 1)
                        {
                            mailClient.Port = int.Parse(server[1]);
                        }
                        mailClient.EnableSsl = GeneralConfig.Instance.EnableSslForSMTP;
                        mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    }
                    else
                    {
                        mailClient.Host = "127.0.0.1";
                        mailClient.DeliveryMethod = deliveryMethod;
                    }

                    if (string.IsNullOrEmpty(from))
                    {
                        throw new Exception("From address is required");
                    }

                    using (var message = new MailMessage(from, to, subject, body))
                    {
                        if (!string.IsNullOrEmpty(cc))
                        {
                            var ccAddress = new MailAddress(cc);
                            message.CC.Add(ccAddress);
                        }
                        if (!string.IsNullOrEmpty(bcc))
                        {
                            var bccAddress = new MailAddress(bcc);
                            message.Bcc.Add(bccAddress);
                        }
                        message.IsBodyHtml = isBodyHtml;

                        if (!string.IsNullOrEmpty(GeneralConfig.Instance.SMTPUsername) && !string.IsNullOrEmpty(GeneralConfig.Instance.SMTPPassword))
                        {
                            mailClient.Credentials = new NetworkCredential(GeneralConfig.Instance.SMTPUsername, GeneralConfig.Instance.SMTPPassword);
                        }
                        else
                        {
                            mailClient.UseDefaultCredentials = true;
                        }

                        mailClient.Send(message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }

            public string GetWebPageContent(string url)
            {
                // Create a 'WebRequest' object with the specified url.
                var myWebRequest = WebRequest.Create(url);

                // Send the 'WebRequest' and wait for response.
                using (var myWebResponse = myWebRequest.GetResponse())
                {
                    // Obtain a 'Stream' object associated with the response object.
                    using (var myStream = myWebResponse.GetResponseStream())
                    {
                        Debug.Assert(myStream != null, "myStream != null");

                        // Pipe the stream to a higher level stream reader with the required encoding format.
                        var encode = Encoding.UTF8;
                        using (var readStream = new StreamReader(myStream, encode))
                        {
                            return readStream.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
