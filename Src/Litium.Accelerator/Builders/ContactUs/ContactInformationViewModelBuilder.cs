using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.ContactUs;
using Litium.Runtime.AutoMapper;

namespace Litium.Accelerator.Builders.ContactUs
{
    public class ContactInformationViewModelBuilder<TViewModel> : IViewModelBuilder<TViewModel>
        where TViewModel : ContactInformationViewModel
    {
        private readonly RequestModelAccessor _requestModelAccessor;

        public ContactInformationViewModelBuilder(RequestModelAccessor requestModelAccessor)
        {
            _requestModelAccessor = requestModelAccessor;
        }

        public virtual ContactInformationViewModel Build()
        {
            var model = _requestModelAccessor.RequestModel.ChannelModel.MapTo<ContactInformationViewModel>();
            return model;
        }
    }
}
