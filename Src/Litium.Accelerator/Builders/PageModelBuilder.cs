using System;
using Litium.Accelerator.ViewModels;

namespace Litium.Accelerator.Builders
{
    public abstract class PageModelBuilder<T> : IViewModelBuilder<T>
        where T : PageViewModel
    {
        public abstract T Build(Guid systemId, DataFilterBase dataFilter = null);
    }
}
