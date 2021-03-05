using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Hosting;
using Litium.Foundation.Configuration;
using Litium.Owin.Lifecycle;
using Litium.Web.Administration;

namespace Litium.Accelerator.WebForm.Site.Administration.Settings
{
    internal class SettingsPageSetup : IStartupTask, IAppExtension
    {
        void IStartupTask.Start()
        {
            List<ControlPanelPage> pages;
            if (!ControlPanelPagesConfig.Instance.Groups.TryGetValue("Accelerator", out pages))
            {
                ControlPanelPagesConfig.Instance.Groups.Add("Accelerator", pages = new List<ControlPanelPage>());
            }
            pages.Add(new ControlPanelPage("~/Litium/UI/settings/iframe/%2FLitium%2FappSettings%2Faccelerator%2Findexing", "accelerator.indexing.setting", ControlPanelPagePermission.All)
            {
                //AuthorizeFunc = () => FoundationContext.Current.User.HasModulePermission(ModuleCMS.Instance.ID, BuiltInModulePermissionTypes.PERMISSION_ID_ALL, true, true)
            });
            pages.Add(new ControlPanelPage("~/Litium/UI/settings/iframe/%2FLitium%2FappSettings%2Faccelerator%2Ffilter", "accelerator.filter.setting", ControlPanelPagePermission.All)
            {
                //AuthorizeFunc = () => FoundationContext.Current.User.HasModulePermission(ModuleCMS.Instance.ID, BuiltInModulePermissionTypes.PERMISSION_ID_ALL, true, true)
            }); 
            
            var elasticSearchconnectionString = ConfigurationManager.ConnectionStrings["ElasticSearchConnectionString"]?.ToString();
            if (!string.IsNullOrWhiteSpace(elasticSearchconnectionString))
            {
                pages.Add(new ControlPanelPage("~/Litium/UI/settings/iframe/%2FLitium%2FappSettings%2Faccelerator%2Fsearchindexing", "accelerator.search.indexing.setting", ControlPanelPagePermission.All));
            }
        }

        IEnumerable<string> IAppExtension.ScriptFiles => HostingEnvironment.VirtualPathProvider.GetDirectory("/Site/Administration/Settings")
            .Files.OfType<VirtualFile>()
            .Where(x => x.VirtualPath.EndsWith(".js"))
            .Select(x => x.VirtualPath);

        IEnumerable<string> IAppExtension.AngularModules { get; }
        IEnumerable<string> IAppExtension.StylesheetFiles { get; }
    }
}
