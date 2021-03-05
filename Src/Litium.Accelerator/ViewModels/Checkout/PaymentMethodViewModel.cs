using Litium.Accelerator.Payments;
using System;
using Litium.Accelerator.Builders;

namespace Litium.Accelerator.ViewModels.Checkout
{
    public class PaymentMethodViewModel : IViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string FormattedPrice { get; set; }

        public PaymentWidgetResult Widget { get; set; }
    }
}
