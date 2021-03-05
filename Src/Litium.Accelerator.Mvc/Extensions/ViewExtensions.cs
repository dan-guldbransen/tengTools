using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Litium.Accelerator.Mvc.Extensions
{
    public static class ViewExtensions
    {
        public static string RenderViewToString(this ApiController controller, string viewName, object viewData)
        {
            var sw = new StringWriter();
            var httpContext = new HttpContextWrapper(HttpContext.Current);

            // point to an empty controller
            var routeData = new RouteData();
            routeData.Values.Add("controller", "EmptyController");

            var controllerContext = new ControllerContext(new RequestContext(httpContext, routeData), new EmptyController());

            var view = ViewEngines.Engines.FindPartialView(controllerContext, viewName).View;

            view.Render(new ViewContext(controllerContext, view, new ViewDataDictionary { Model = viewData }, new TempDataDictionary(), sw), sw);

            return sw.ToString();
        }

        class EmptyController : Controller { }
    }
}