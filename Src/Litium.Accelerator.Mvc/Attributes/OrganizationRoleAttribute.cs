using System.Web;
using System.Web.Mvc;
using Litium.Accelerator.Utilities;
using Litium.Web.Routing;

namespace Litium.Accelerator.Mvc.Attributes
{
    public class OrganizationRoleAttribute : AuthorizeAttribute
    {
        private readonly bool _orderApproval;
        private readonly bool _orderPlacer;

        public OrganizationRoleAttribute(bool orderApproval, bool orderPlacer)
        {
            _orderApproval = orderApproval;
            _orderPlacer = orderPlacer;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var routeRequestLookupInfo = IoC.Resolve<RouteRequestLookupInfoAccessor>().RouteRequestLookupInfo;
            if (routeRequestLookupInfo.IsInAdministration)
            {
                return true;
            }

            var personStorage = IoC.Resolve<PersonStorage>();
            if (_orderApproval && personStorage.HasApproverRole)
            {
                return true;
            }

            if (_orderPlacer && personStorage.HasPlacerRole)
            {
                return true;
            }

            return false;
        }
    }
}
