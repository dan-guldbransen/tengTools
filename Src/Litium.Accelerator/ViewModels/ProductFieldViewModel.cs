using Litium.Accelerator.Builders;
using Litium.Studio.FieldFramework;

namespace Litium.Accelerator.ViewModels
{
    public class ProductFieldViewModel : IViewModel
    {
        public ProductFieldViewModel()
        {
        }

        public ProductFieldViewModel(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public FieldFormatArgs Args { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string ViewName { get; set; }
    }
}