using Litium.Accelerator.Payments;
using Litium.Accelerator.ViewModels.Persons;
using System;
using System.Collections.Generic;
using Litium.Accelerator.Builders;

namespace Litium.Accelerator.ViewModels.Checkout
{
    public class CheckoutViewModel : IViewModel
    {
        public PaymentWidgetResult PaymentWidget { get; set; }

        public IList<DeliveryMethodViewModel> DeliveryMethods { get; set; }
        public IList<PaymentMethodViewModel> PaymentMethods { get; set; }
        public Guid? SelectedDeliveryMethod { get; set; }
        public string SelectedCountry { get; set; }
        public string SelectedPaymentMethod { get; set; }
        public CustomerDetailsViewModel CustomerDetails { get; set; }
        public CustomerDetailsViewModel AlternativeAddress { get; set; }
        public IList<AddressViewModel> CompanyAddresses { get; set; }
        public Guid? SelectedCompanyAddressId { get; set; }
        public bool Authenticated { get; set; }
        public bool AcceptTermsOfCondition { get; set; }
        public bool ShowAlternativeAddress { get; set; }
        public int CheckoutMode { get; set; }
        public bool SignUp { get; set; }
        public bool IsBusinessCustomer { get; set; }
        public string OrderNote { get; set; }
        public string CompanyName { get; set; }
        public string CampaignCode { get; set; }

        public string OrderId { get; set; }
        public string Payload { get; set; }
        public bool Success { get; set; }
        public Dictionary<string,List<string>> ErrorMessages { get; set; } = new Dictionary<string, List<string>>();

        public string CheckoutUrl { get; set; }
        public string TermsUrl { get; set; }
        public string LoginUrl { get; set; }
        public string RedirectUrl { get; set; }
    }
}
