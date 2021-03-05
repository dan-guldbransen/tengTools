using System;

namespace Litium.Accelerator.Payments
{
    public class PaymentWidgetResult
    {
        public bool DisplayCustomerDetails { get; set; }
        public bool DisplayDeliveryMethods { get; set; } = true;
        public bool IsChangeable { get; set; } = true;
        public string ResponseString { get; set; }
        public Uri RedirectUrl { get; set; }
        public string Id { get; set; }
        public long Hash { get; set; }
    }
}
