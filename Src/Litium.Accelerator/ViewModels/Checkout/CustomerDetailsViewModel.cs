using AutoMapper;
using JetBrains.Annotations;
using Litium.Accelerator.ViewModels.Persons;
using Litium.Foundation.Modules.ECommerce.Carriers;
using Litium.Runtime.AutoMapper;

namespace Litium.Accelerator.ViewModels.Checkout
{
    public class CustomerDetailsViewModel : AddressViewModel, IAutoMapperConfiguration
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        [UsedImplicitly]
        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<CustomerDetailsViewModel, AddressCarrier>()
                .ForMember(x => x.Address1, m => m.MapFrom(address => address.Address))
                .ForMember(x => x.MobilePhone, m => m.MapFrom(address => address.PhoneNumber))
                .ForMember(x => x.Zip, m => m.MapFrom(address => address.ZipCode));
        }
    }
}
