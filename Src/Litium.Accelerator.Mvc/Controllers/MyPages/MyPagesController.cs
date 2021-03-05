using Litium.Accelerator.Services;
using Litium.Accelerator.ViewModels.MyPages;
using System.Web.Mvc;
using Litium.Accelerator.Builders.MyPages;
using Litium.Accelerator.Mvc.ModelStates;
using Litium.Accelerator.Constants;
using ModelState = Litium.Accelerator.Services.ModelState;

namespace Litium.Accelerator.Mvc.Controllers.MyPages
{
    public class MyPagesController : ControllerBase
    {
        private readonly MyPagesViewModelBuilder _myPagesViewModelBuilder;
        private readonly MyPagesViewModelService _myPagesViewModelService;

        private readonly ModelState _modelState;

        public MyPagesController(MyPagesViewModelBuilder myPagesViewModelBuilder, MyPagesViewModelService myPagesViewModelService)
        {
            _myPagesViewModelBuilder = myPagesViewModelBuilder;
            _myPagesViewModelService = myPagesViewModelService;

            _modelState = new MvcModelState(ModelState);
        }

        [HttpGet]
        public ActionResult Index(string currentTab = MyPagesTabConstants.MyDetails)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new RedirectResult("~/");
            }

            var model = _myPagesViewModelBuilder.Build();
            model.CurrentTab = currentTab;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveMyDetails(MyDetailsViewModel myDetailsPanel)
        {
            if (_myPagesViewModelService.IsValidMyDetailsForm(_modelState, myDetailsPanel))
            {
                _myPagesViewModelService.SaveMyDetails(myDetailsPanel);
                return RedirectToAction(nameof(Index));
            }

            var model = _myPagesViewModelBuilder.Build(myDetailsPanel);
            model.CurrentTab = MyPagesTabConstants.MyDetails;
            return View(nameof(Index), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveBusinessCustomerDetails(BusinessCustomerDetailsViewModel businessCustomerDetailsPanel)
        {
            if (_myPagesViewModelService.IsValidBusinessCustomerDetailsForm(_modelState, businessCustomerDetailsPanel))
            {
                _myPagesViewModelService.SaveMyDetails(businessCustomerDetailsPanel);
                return RedirectToAction(nameof(Index));
            }

            var model = _myPagesViewModelBuilder.Build(businessCustomerDetailsPanel);
            model.CurrentTab = MyPagesTabConstants.MyDetails;
            return View(nameof(Index), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveUserName(ChangeUserNameFormViewModel userNameForm)
        {
            if (_myPagesViewModelService.IsValidUserNameForm(_modelState, userNameForm))
            {
                _myPagesViewModelService.SaveUserName(userNameForm.UserName);
                return RedirectToAction(nameof(Index), new { CurrentTab = MyPagesTabConstants.LoginInfo });
            }

            var model = _myPagesViewModelBuilder.Build();
            model.CurrentTab = MyPagesTabConstants.LoginInfo;
            return View(nameof(Index), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePassword(ChangePasswordFormViewModel passwordForm)
        {
            if (_myPagesViewModelService.IsValidPasswordForm(_modelState, passwordForm))
            {
                _myPagesViewModelService.SavePassword(passwordForm.Password);
                return RedirectToAction(nameof(Index), new { CurrentTab = MyPagesTabConstants.LoginInfo });
            }

            var model = _myPagesViewModelBuilder.Build();
            model.CurrentTab = MyPagesTabConstants.LoginInfo;
            return View(nameof(Index), model);
        }
    }
}