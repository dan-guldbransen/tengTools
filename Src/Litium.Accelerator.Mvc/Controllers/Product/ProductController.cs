using System.Collections.Generic;
using System.Web.Mvc;
using Litium.Accelerator.Builders.Product;
using Litium.Products;
using Litium.Web.Rendering;

namespace Litium.Accelerator.Mvc.Controllers.Product
{
    public class ProductController : ControllerBase
    {
        private readonly ProductPageViewModelBuilder _productPageViewModelBuilder;
        private readonly BaseProductService _baseProductService;
        private readonly ICollection<IRenderingValidator<BaseProduct>> _renderingValidators;

        public ProductController(
            ProductPageViewModelBuilder productPageViewModelBuilder,
            BaseProductService baseProductService,
            ICollection<IRenderingValidator<BaseProduct>> renderingValidators)
        {
            _productPageViewModelBuilder = productPageViewModelBuilder;
            _baseProductService = baseProductService;
            _renderingValidators = renderingValidators;
        }

        [HttpGet]
        public ActionResult ProductWithVariants(Variant variant)
        {
            if (variant == null)
            {
                return HttpNotFound();
            }

            var baseProduct = _baseProductService.Get(variant.BaseProductSystemId);
            if (baseProduct == null || !_renderingValidators.Validate<BaseProduct>(baseProduct))
            {
                return HttpNotFound();
            }

            var productPageModel = _productPageViewModelBuilder.Build(variant);
            return View(productPageModel);
        }

        [HttpGet]
        public ActionResult ProductWithVariantListing(BaseProduct baseProduct)
        {
            if (!_renderingValidators.Validate(baseProduct))
            {
                return HttpNotFound();
            }

            var productPageModel = _productPageViewModelBuilder.Build(baseProduct);
            return View(productPageModel);
        }
    }
}
