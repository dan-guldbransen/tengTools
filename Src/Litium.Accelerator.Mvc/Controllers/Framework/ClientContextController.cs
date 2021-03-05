using System.Web.Mvc;
using Litium.Accelerator.Builders.Framework;

namespace Litium.Accelerator.Mvc.Controllers.Framework
{
    public class ClientContextController : ControllerBase
    {
        private readonly ClientContextViewModelBuilder _clientContextViewModelBuilder;

        public ClientContextController(ClientContextViewModelBuilder clientContextViewModelBuilder)
        {
            _clientContextViewModelBuilder = clientContextViewModelBuilder;
        }

        [ChildActionOnly]
        public ActionResult Index()
        {
            var viewModel = _clientContextViewModelBuilder.Build();
            return PartialView("Framework/ClientContext", viewModel);
        }
    }
}