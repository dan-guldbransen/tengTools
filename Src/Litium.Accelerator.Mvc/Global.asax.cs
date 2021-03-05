using Litium.Accelerator.Mvc.App_Start;
using System;
using System.Web;
using System.Web.SessionState;

namespace Litium.Accelerator.Mvc
{
    public class MvcApplication : HttpApplication
    {
        public override void Init()
        {
            // enable Session for WebApi controller
            PostAuthenticateRequest += MvcApplication_PostAuthenticateRequest;
            base.Init();
        }

        void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            // force the session to be enabled for Web Api request only, not every requests
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }

        private static bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(WebApiConfig.RoutePrefix);
        }
    }
}
