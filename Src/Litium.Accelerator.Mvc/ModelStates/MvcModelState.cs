using System.Web.Mvc;
using ModelState = Litium.Accelerator.Services.ModelState;

namespace Litium.Accelerator.Mvc.ModelStates
{
    public class MvcModelState : ModelState
    {
        private readonly ModelStateDictionary _modelState;

        public MvcModelState(ModelStateDictionary modelState)
        {
            _modelState = modelState;
        }

        public override void AddModelError(string key, string errorMessage)
        {
            _modelState.AddModelError(key, errorMessage);
        }

        public override bool IsValid => _modelState.IsValid;
    }
}