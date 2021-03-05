using System.Collections.Generic;
using Litium.Products;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Definitions
{
    [Service(ServiceType = typeof(DisplayTemplateSetup))]
    public abstract class DisplayTemplateSetup
    {
        public abstract IEnumerable<DisplayTemplate> GetDisplayTemplates();
    }
}
