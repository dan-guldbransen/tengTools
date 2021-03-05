using Litium.Web.Models.Blocks;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Litium.Accelerator.Mvc
{
    public static class BlockContainerHelperExtension
    {
        public static MvcHtmlString BlockContainer(this HtmlHelper html, Dictionary<string, List<BlockModel>> containers, string containerId, string partialView = "Framework/_BlockContainer")
        {
            if (containers == null || !containers.TryGetValue(containerId, out var blocks))
            {
                return null;
            }
            return html.Partial(partialView, blocks);
        }
    }
}