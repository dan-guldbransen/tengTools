using Litium.Customers;
using Litium.Studio.Extenssions;
using System;

namespace Litium.Accelerator.Mailing
{
    public class ForgotPasswordEmail : HtmlMailDefinition
    {
        private readonly Guid _channelSystemId;
        private readonly Person _user;
        private readonly string _password;

        public ForgotPasswordEmail(Person user, string password, Guid channelSystemId)
        {
            _channelSystemId = channelSystemId;
            _user = user;
            _password = password;
        }

        public override string Body => $"{"general.username".AsWebSiteString()}: {_user.LoginCredential.Username}<br />{"general.password".AsWebSiteString()}: {_password}";

        public override Guid ChannelSystemId => _channelSystemId;

        public override string Subject => "forgotpassword.emailsubject".AsWebSiteString();

        public override string ToEmail => _user.Email;
    }
}
