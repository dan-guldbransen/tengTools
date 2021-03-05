using System.Collections.Generic;
using System.Web.Mvc;
using Litium.Accelerator.Builders;

namespace Litium.Accelerator.ViewModels.Deployment
{
    public class DeploymentViewModel : IViewModel
    {
        public ImportViewModel ImportForm { get; set; } = new ImportViewModel();
        public ExportViewModel ExportForm { get; set; } = new ExportViewModel();

        public IEnumerable<SelectListItem> Channels { get; set; }
        public IEnumerable<SelectListItem> Folders { get; set; }
        public IList<SelectListItem> Packages { get; set; }

        public bool CanImport { get; set; }
        public bool ShowExport { get; set; }
        public bool CanExport { get; set; }
        public string ImportMessage { get; set; }
        public string ExportMessage { get; set; }
        public bool ShowImport { get; set; }

        public class ImportViewModel
        {
            public string Name { get; set; }
            public string DomainName { get; set; }
            public string PackageName { get; set; }
            public bool CreateExampleProducts { get; set; } = true;
        }

        public class ExportViewModel
        {
            public string ChannelSystemId { get; set; }
            public string FolderSystemId { get; set; }
        }
    }
}
