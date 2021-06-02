using Litium.Accelerator.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.ViewModels.Framework
{
    public class SubCategoryNavigationVievModel : IViewModel
    {
        public SubNavigationLinkModel LinkModel { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string CtaLink { get; set; }
    }
}
