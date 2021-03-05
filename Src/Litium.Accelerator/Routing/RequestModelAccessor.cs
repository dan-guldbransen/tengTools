using System.Threading;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Routing
{
    [Service(ServiceType = typeof(RequestModelAccessor))]
    public class RequestModelAccessor
    {
        private static readonly AsyncLocal<RequestModel> _routeRequest = new AsyncLocal<RequestModel>();

        public virtual RequestModel RequestModel
        {
            get
            {
                var context = System.Web.HttpContext.Current;
                if (context != null)
                {
                    return (RequestModel)context.Items[nameof(RequestModel)];
                }

                return _routeRequest.Value;
            }

            set
            {
                var context = System.Web.HttpContext.Current;
                if (context != null)
                {
                    context.Items[nameof(RequestModel)] = value;
                }
                else
                {
                    _routeRequest.Value = value;
                }
            }
        }
    }
}
