using Litium.Runtime.DependencyInjection;
using Litium.Web.Models.Websites;
using System;
using System.IO;
using System.Reflection;
using System.Web;


namespace Litium.Accelerator.Services
{
    [Service(ServiceType = typeof(DealerService))]
    public class DealerService
    {
        private static readonly string Path = $"{AppDomain.CurrentDomain.BaseDirectory}\\App_Files\\";
        private const string FileName = "Dealers.xlsx";

        public string SaveFile(HttpPostedFileBase file, WebsiteModel website)
        {
            try
            {
               

                if (!Directory.Exists(Path))
                {
                    Directory.CreateDirectory(Path);
                }
                
                file.SaveAs(System.IO.Path.Combine(Path, FileName));
                return website.Texts.GetValue("dealer.backoffice.fileupload.success");
            }
            catch(Exception e)
            {
                return website.Texts.GetValue("dealer.backoffice.fileupload.failed") + $" Error: {e.Message}";
            }
        }

        public (byte[], string) GetDealerFileBytes()
        {
            
            var filePath = Directory.GetCurrentDirectory() + "/Files/" + FileName;

            if (File.Exists(filePath))
            {
                return (File.ReadAllBytes(filePath), FileName);
            }

            return (null, "");

        }
    }
}
