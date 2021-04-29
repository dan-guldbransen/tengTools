using Litium.Web.Models;
using System.Collections.Generic;
using Litium.Accelerator.Builders;

namespace Litium.Accelerator.ViewModels.Framework
{
    public class HeaderViewModel : IViewModel
    {
        public bool IsLoggedIn { get; set; }
        public LinkModel LoginPage { get; set; }
        public ImageModel Logo { get; set; }
        public LinkModel MyPage { get; set; }
        public LinkModel GetOrganised { get; set; }
        public string StartPageUrl { get; set; }
        public IList<LinkModel> TopLinkList { get; set; }
        public QuickSearchViewModel QuickSearch { get; set; }
        public string ExternalB2BLink { get; set; }
    }
}