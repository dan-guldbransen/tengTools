using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.Framework;
using Litium.FieldFramework;
using Litium.Products;
using Litium.Runtime.AutoMapper;
using Litium.Web;
using Litium.Web.Models;
using Litium.Web.Models.Products;
using Litium.Web.Rendering;

namespace Litium.Accelerator.Builders.Product
{
    public class ChildCategoryNavigationBuilder: IViewModelBuilder<ContentLinkModel>
    {
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly CategoryService _categoryService;
        private readonly UrlService _urlService;
        private readonly ICollection<IRenderingValidator<Category>> _renderingValidators;

        public ChildCategoryNavigationBuilder(
            RequestModelAccessor requestModelAccessor,
            CategoryService categoryService,
            UrlService urlService,
            ICollection<IRenderingValidator<Category>> renderingValidators)
        {
            _requestModelAccessor = requestModelAccessor;
            _categoryService = categoryService;
            _urlService = urlService;
            _renderingValidators = renderingValidators;
        }

        public IEnumerable<ContentLinkModel> Build(CategoryModel currentCategory)
        {
            IEnumerable<ContentLinkModel> categories;

            if (_requestModelAccessor.RequestModel.WebsiteModel.GetNavigationType() != NavigationType.Category)
            {
                return null;
            }
            var channelSystemId = _requestModelAccessor.RequestModel.ChannelModel.Channel.SystemId;
            if (currentCategory == null)
            {
                categories = Enumerable.Empty<ContentLinkModel>();
            }
            else
            {
                categories = _categoryService
                    .GetChildCategories(currentCategory.SystemId, currentCategory.Category.AssortmentSystemId)
                    .Where(z => z.ChannelLinks.Any(zz => zz.ChannelSystemId == channelSystemId)
                                && _renderingValidators.Validate(z))
                    .Select(x => new ContentLinkModel
                    {
                        Name = x.Localizations.CurrentUICulture.Name,
                        Url = _urlService.GetUrl(x),
                        Image = x.Fields.GetValue<IList<Guid>>(SystemFieldDefinitionConstants.Images)?.FirstOrDefault().MapTo<ImageModel>(),
                        Links = _categoryService
                            .GetChildCategories(x.SystemId)
                            .Where(z => z.ChannelLinks.Any(zz =>zz.ChannelSystemId == channelSystemId)
                                        && _renderingValidators.Validate(z))
                            .Select(z => new ContentLinkModel
                            {
                                Name = z.Localizations.CurrentCulture.Name,
                                Url = _urlService.GetUrl(z)
                            }).ToList()
                    }).ToList();
            }
            return categories;
        }
    }
}
