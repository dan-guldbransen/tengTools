using System.Collections.Generic;
using JetBrains.Annotations;
using AutoMapper;
using Litium.Runtime.AutoMapper;
using Litium.Accelerator.Builders;
using Litium.Accelerator.Constants;
using Litium.Accelerator.ViewModels.Framework;
using Litium.Web.Models.Websites;
using Litium.Accelerator.Extensions;

namespace Litium.Accelerator.ViewModels.Brand
{
    public class BrandListViewModel : IAutoMapperConfiguration, IViewModel
    {
        public readonly int MaxColumns = 4;
        public const string TagName = "Brand";

        public int PageSize { get; set; }
        public string TitleFilterSelector { get; set; }
        public BrandNode Nodes { get; set; }
        public class BrandNode : List<BrandNode>
        {
            public BrandNode() : this(null)
            {
            }

            public BrandNode(ContentLinkModel value)
            {
                Value = value;
            }

            public ContentLinkModel Value { get; set; }
        }

        [UsedImplicitly]
        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<PageModel, BrandListViewModel>()
               .ForMember(x => x.PageSize, m => m.MapFromField(PageFieldNameConstants.PageSize))
               .ForMember(x => x.TitleFilterSelector, m => m.MapFrom(brandListPage => brandListPage.Page.Localizations.CurrentUICulture.Name));
        }
    }
}
