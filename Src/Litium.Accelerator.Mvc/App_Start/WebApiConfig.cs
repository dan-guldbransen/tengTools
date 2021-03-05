using Litium.Accelerator.Mvc.Routing;
using Litium.Web.WebApi;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Web.Http;

namespace Litium.Accelerator.Mvc.App_Start
{
    public class WebApiConfig : IWebApiSetup
    {
        public static readonly string RoutePrefix = "~/api/";
        private readonly IServiceProvider _serviceProvider;

        public WebApiConfig(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void SetupWebApi(HttpConfiguration config)
        {
            config.MessageHandlers.Add(ActivatorUtilities.CreateInstance<RequestModelHandler>(_serviceProvider));
        }
    }
}