using Litium.Customers;
using Litium.Web.Administration.Panels;
using Litium.Websites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.Panels
{
    public class DealerUploadPanel : PanelDefinitionBase<CustomerArea, DealerUploadPanel.SettingsModel>
    {
        public override string ComponentName => null;

        public override string Url => "/litium/dealers";

        public override bool PermissionCheck()
        {
            return true;
        }

        public override async Task<SettingsModel> GetSettingsAsync()
        {
            return new SettingsModel();
        }

        public class SettingsModel : IPanelSettings
        {
            public string Title { get; set; } = "Dealers";
        }
    }
}
