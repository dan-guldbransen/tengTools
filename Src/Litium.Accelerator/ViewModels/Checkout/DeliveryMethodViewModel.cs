using System;
using Litium.Accelerator.Builders;

namespace Litium.Accelerator.ViewModels.Checkout
{
    public class DeliveryMethodViewModel : IViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string FormattedPrice { get; set; }
    }
}
