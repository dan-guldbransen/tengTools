using Litium.Accelerator.ViewModels.Block;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models.Blocks;

namespace Litium.Accelerator.Builders.Block
{
    public class ProductsAndBannerBlockViewModelBuilder : IViewModelBuilder<ProductsAndBannerBlockViewModel>
    {
        private readonly BannersBlockViewModelBuilder _bannersViewModelBuilder;
        private readonly ProductBlockViewModelBuilder _productViewModelBuilder;

        public ProductsAndBannerBlockViewModelBuilder(BannersBlockViewModelBuilder bannersViewModelBuilder, ProductBlockViewModelBuilder productViewModelBuilder)
        {
            _bannersViewModelBuilder = bannersViewModelBuilder;
            _productViewModelBuilder = productViewModelBuilder;
        }

        /// <summary>
        /// Build the mixed block view model
        /// </summary>
        /// <param name="blockModel">The current mixed block</param>
        /// <returns>Return the mixed block view model</returns>
        public virtual ProductsAndBannerBlockViewModel Build(BlockModel blockModel)
        {
            var sectionBanners = _bannersViewModelBuilder.Build(blockModel);
            var sectionProducts = _productViewModelBuilder.Build(blockModel);

            var mixedBlockViewModel = blockModel.MapTo<ProductsAndBannerBlockViewModel>();
            mixedBlockViewModel.Products = sectionProducts;
            mixedBlockViewModel.Banners = sectionBanners;
            return mixedBlockViewModel;
        }
    }
}
