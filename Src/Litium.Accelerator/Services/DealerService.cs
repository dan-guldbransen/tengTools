using Litium.Accelerator.ViewModels.Dealer;
using Litium.Runtime.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Litium.Accelerator.Services
{
    [Service(ServiceType = typeof(DealerService))]
    public class DealerService
    {
        public bool SaveFile(DealerViewModel model, string filePath)
        {
            try
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                var name = "Dealers";
                filePath = filePath + name + Path.GetExtension(model.ImportForm.DealerFile.FileName);
                model.ImportForm.DealerFile.SaveAs(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
