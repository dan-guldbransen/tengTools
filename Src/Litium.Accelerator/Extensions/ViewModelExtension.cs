using System.Collections.Generic;
using Litium.Accelerator.Builders;
using Litium.Accelerator.Services;
using Litium.Accelerator.Utilities;

namespace Litium.Accelerator.Extensions
{
    public static class ViewModelExtension
    {
        public static bool IsValid<TForm>(this IViewModel viewModel, IEnumerable<ValidationRuleItem<TForm>> validationRules, ModelState modelState)
            where TForm : IViewModel
        {
            foreach (var validationRule in validationRules ?? new List<ValidationRuleItem<TForm>>())
            {
                if (validationRule.Rule((TForm)viewModel))
                    continue;

                modelState.AddModelError(validationRule.Field, validationRule.ErrorMessage?.Invoke());
                break;
            }

            return modelState.IsValid;
        }
    }
}
