using Litium.Accelerator.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Litium.Accelerator.ViewModels.Dealer
{
    public class DealerViewModel : IViewModel
    {
        public ImportViewModel ImportForm { get; set; } = new ImportViewModel();
        
        public string ImportMessage { get; set; }

        public List<DealerItemViewModel> Dealers { get; set; }

        public class ImportViewModel
        {
            [Required]
            public HttpPostedFileBase DealerFile { get; set; }
        }
        
    }
}
