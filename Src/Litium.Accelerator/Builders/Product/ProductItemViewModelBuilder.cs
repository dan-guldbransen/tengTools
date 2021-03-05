using System;
using System.Collections.Generic;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Services;
using Litium.Accelerator.ViewModels.Product;
using Litium.FieldFramework;
using Litium.Foundation.Modules.ECommerce.ShoppingCarts;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models;
using Litium.Web.Models.Products;

namespace Litium.Accelerator.Builders.Product
{
    public class ProductItemViewModelBuilder : IViewModelBuilder<ProductItemViewModel>
    {
        private Cart Cart => _requestModelAccessor.RequestModel.Cart;

        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly ProductPriceModelBuilder _productPriceModelBuilder;
        private readonly FieldDefinitionService _fieldDefinitionService;
        private readonly StockService _stockService;
        private readonly ProductModelBuilder _productModelBuilder;

        public ProductItemViewModelBuilder(RequestModelAccessor requestModelAccessor,
            ProductPriceModelBuilder productPriceModelBuilder,
            FieldDefinitionService fieldDefinitionService,
            StockService stockService,
            ProductModelBuilder productModelBuilder)
        {
            _requestModelAccessor = requestModelAccessor;
            _productPriceModelBuilder = productPriceModelBuilder;
            _fieldDefinitionService = fieldDefinitionService;
            _stockService = stockService;
            _productModelBuilder = productModelBuilder;
        }

        public virtual ProductItemViewModel Build(Variant variant)
        {
            var productModel = _productModelBuilder.BuildFromVariant(variant);
            return productModel == null ? null : Build(productModel);
        }

        public virtual ProductItemViewModel Build(ProductModel productModel, bool inProductListPage = true)
        {
            var currency = Cart.Currency;
            var websiteModel = _requestModelAccessor.RequestModel.WebsiteModel;
            var productPriceModel = _productPriceModelBuilder.Build(productModel.SelectedVariant, currency,_requestModelAccessor.RequestModel.ChannelModel.Channel);

            return new ProductItemViewModel
            {
                Id = productModel.SelectedVariant.Id,
                Price = productPriceModel,
                StockStatusDescription = _stockService.GetStockStatusDescription(productModel.SelectedVariant),
                Currency = currency,
                IsInStock = _stockService.HasStock(productModel.SelectedVariant),
                Images = productModel.SelectedVariant.Fields.GetValue<IList<Guid>>(SystemFieldDefinitionConstants.Images).MapTo<IList<ImageModel>>(),
                Color = _fieldDefinitionService.Get<ProductArea>("Color").GetTranslation(productModel.GetValue<string>("Color")),
                Size = _fieldDefinitionService.Get<ProductArea>("Size").GetTranslation(productModel.GetValue<string>("Size")),
                Brand = _fieldDefinitionService.Get<ProductArea>("Brand").GetTranslation(productModel.GetValue<string>("Brand")),
                Description = productModel.GetValue<string>(SystemFieldDefinitionConstants.Description),
                Name = productModel.GetValue<string>(SystemFieldDefinitionConstants.Name),
                Url = productModel.GetUrl(websiteModel.SystemId, channelSystemId: _requestModelAccessor.RequestModel.ChannelModel.SystemId),
                QuantityFieldId = Guid.NewGuid().ToString(),
                ShowBuyButton = websiteModel.GetValue<bool>(AcceleratorWebsiteFieldNameConstants.ShowBuyButton),
                ShowQuantityField = inProductListPage ? websiteModel.GetValue<bool>(AcceleratorWebsiteFieldNameConstants.ShowQuantityFieldProductList)
                                                      : websiteModel.GetValue<bool>(AcceleratorWebsiteFieldNameConstants.ShowQuantityFieldProductPage),
                UseVariantUrl = productModel.UseVariantUrl
            };
        }
    }
}
