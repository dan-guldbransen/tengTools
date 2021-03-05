using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Litium.Accelerator.Mvc.Routing;
using Litium.Owin.Lifecycle;

namespace Litium.Accelerator.Mvc.App_Start
{
    internal class RouteConfig : IPostSetupTask
    {
        public void PostSetup(IEnumerable<Assembly> assemblies)
        {
            var routes = RouteTable.Routes;

            // Register direct access for controllers
            routes.MapRoute(
                "ControllerDirect", // Route name
                "site.axd/{controller}/{action}/{id}", // URL with parameters
                new { action = "Index", id = UrlParameter.Optional } // Parameter defaults
                );

            var filters = GlobalFilters.Filters;

            filters.Add(new RequestModelActionFilter());
        }
    }
}
