using System.Web.Http.ModelBinding;
using ModelState = Litium.Accelerator.Services.ModelState;

namespace Litium.Accelerator.Mvc.ModelStates
{
    public class ApiModelState : ModelState
    {
        private readonly ModelStateDictionary _modelState;

        public ApiModelState(ModelStateDictionary modelState)
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