using Litium.Accelerator.Builders;
using Litium.Accelerator.ViewModels.Framework;
using Litium.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.ViewModels.ContactUs
{
    public class ContactAccordionViewModel : IViewModel
    {
        public string PageTitle { get; set; }
        
        public bool IsPartnerList { get; set; }

        public List<ContentViewModel> Content { get; set; } = new List<ContentViewModel>();
    }

    public class ContentViewModel
    {
        public string Country { get; set; }

        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }

        public SocialMediaViewModel SocialMedia { get; set; } = new SocialMediaViewModel();

    }
}
