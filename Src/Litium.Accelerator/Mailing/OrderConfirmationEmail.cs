using Litium.Accelerator.Caching;
using Litium.Studio.Extenssions;
using Litium.Websites;
using System;

namespace Litium.Accelerator.Mailing
{
    public class OrderConfirmationEmail : PageMailDefinition
    {
        private readonly Guid _channelSystemId;
        private readonly Guid _orderId;
        private readonly string _toEmail;
        private Page _page;

        public OrderConfirmationEmail(Guid channelSystemId, Guid orderId, string toEmail)
        {
            _channelSystemId = channelSystemId;
            _orderId = orderId;
            _toEmail = toEmail;
        }

        public override Guid ChannelSystemId => _channelSystemId;

        public override string ToEmail => _toEmail;

        public override string Subject => "orderconfirmation.emailsubject".AsWebSiteString();

        public override Page Page
        {
            get
            {
                if (_page == null)
                {
                    IoC.Resolve<PageByFieldTemplateCache<OrderConfirmationPageByFieldTemplateCache>>().TryFindPage(orderConfirmation =>
                    {
                        _page = orderConfirmation;
                        return true;
                    });
                }

                return _page;
            }
        }

        public override string UrlTransform(string url)
        {
            return $"{url}?orderId={_orderId}&isEmail={true}";
        }
    }
}
