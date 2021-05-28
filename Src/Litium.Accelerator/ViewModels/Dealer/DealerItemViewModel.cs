using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.ViewModels.Dealer
{
    public class DealerItemViewModel
    {
        public string CompanyName { get; set; }

        public string City { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public string StreetAdress { get; set; }

        public string ZipCode { get; set; }

        public string Country { get; set; }

        public string Ecom { get; set; }

        public string DealerNumber { get; set; }

        public string DealerGroup { get; set; }

        public bool IsEcom { get => Ecom.Equals("ja", StringComparison.OrdinalIgnoreCase); }
    }
}
