using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Definitions;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Mvc.Definitions
{
    [ServiceDecorator(typeof(DisplayTemplateSetup))]
    public class DisplayTemplateSetupDecorator : DisplayTemplateSetup
    {
        private readonly DisplayTemplateSetup _parent;
        private readonly IDictionary<string, (Type controllerType, string action)> _controllerMapping = new Dictionary<string, (Type controllerType, string action)>
        {
            [ProductTemplateNameConstants.Category] = (typeof(Controllers.Category.CategoryController), nameof(Controllers.Category.CategoryController.Index)),
            [ProductTemplateNameConstants.Product] = (typeof(Controllers.Product.ProductController), nameof(Controllers.Product.ProductController.ProductWithVariants)),
            [ProductTemplateNameConstants.ProductWithVariantList] = (typeof(Controllers.Product.ProductController), nameof(Controllers.Product.ProductController.ProductWithVariantListing))
        };

        public DisplayTemplateSetupDecorator(DisplayTemplateSetup parent)
        {
            _parent = parent;
        }

        public override IEnumerable<DisplayTemplate> GetDisplayTemplates()
        {
            return _parent.GetDisplayTemplates().Select(x =>
            {
                var prop = x.GetType().GetProperty("TemplatePath");
                if (prop != null && _controllerMapping.TryGetValue(x.Id, out var map))
                {
                    prop.SetValue(x, "~/MVC:" + map.controllerType.MapTo<string>() + ":" + map.action);
                }
                return x;
            });
        }
    }
}