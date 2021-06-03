using Litium.Accelerator.Builders.Product;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Accelerator.ViewModels.Framework;
using Litium.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.Builders.Framework
{
    public class FavoritesViewModelBuilder<TViewModel> : IViewModelBuilder<TViewModel>
        where TViewModel : FavoritesViewModel
    {
        private readonly VariantService _variantService;
        private readonly ProductItemViewModelBuilder _productItemBuilder;

        public FavoritesViewModelBuilder(VariantService variantService, ProductItemViewModelBuilder productItemBuilder)
        {
            _variantService = variantService;
            _productItemBuilder = productItemBuilder;
        }

        public FavoritesViewModel Build(string[] variantSystemIds)
        {
            var viewModel = new FavoritesViewModel();

            if(variantSystemIds != null && variantSystemIds.Any())
            {
                foreach(var id in variantSystemIds)
                {
                    var variant = _variantService.Get(id);
                    if(variant != null)
                    {
                        var variantViewModel = _productItemBuilder.Build(variant);
                        viewModel.Products.Add(variantViewModel);
                    }
                }
            }

            return viewModel;
        }
    }
}
