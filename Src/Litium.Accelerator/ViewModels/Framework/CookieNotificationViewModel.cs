using Litium.Accelerator.Builders;
using Litium.Web.Models;

namespace Litium.Accelerator.ViewModels.Framework
{
    public class CookieNotificationViewModel : IViewModel
    {
        public LinkModel PolicyPage { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        public bool ShouldRender { get; set; }
    }
}
