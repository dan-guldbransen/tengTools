using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models.Blocks;
using Litium.FieldFramework;
using Litium.Blocks;
using JetBrains.Annotations;
using Litium.Accelerator.Builders;
using Litium.Accelerator.Constants;
using Litium.Web.Models;
using Litium.FieldFramework.FieldTypes;

namespace Litium.Accelerator.ViewModels.Block
{
    public class BannersBlockViewModel : IViewModel, IAutoMapperConfiguration
    {
        public Guid SystemId { get; set; }

        public List<BannerBlockItemViewModel> Banners { get; set; } = new List<BannerBlockItemViewModel>();

        [UsedImplicitly]
        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<BlockModel, BannersBlockViewModel>()
               .ForMember(x => x.Banners, m => m.MapFrom<BannersResolver>());
        }

        private class BannersResolver : IValueResolver<BlockModel, BannersBlockViewModel, List<BannerBlockItemViewModel>>
        {
            private readonly FieldTemplateService _fieldTemplateService;

            public BannersResolver(FieldTemplateService fieldTemplateService)
            {
                _fieldTemplateService = fieldTemplateService;
            }

            public List<BannerBlockItemViewModel> Resolve(BlockModel block, BannersBlockViewModel bannersViewModel, List<BannerBlockItemViewModel> destMember, ResolutionContext context)
            {
                var result = new List<BannerBlockItemViewModel>();
                var blockTemplate = _fieldTemplateService.Get<FieldTemplateBase>(block.FieldTemplateSystemId);
                if (blockTemplate.FieldGroups.Any(x => x.Id == "Banners"))
                {
                    var banners = block.GetValue<IList<MultiFieldItem>>(BlockFieldNameConstants.Banners);
                    if (banners != null)
                    {
                        result.AddRange(banners.Select(c => c.MapTo<BannerBlockItemViewModel>()));
                    }
                }
                else if (blockTemplate.FieldGroups.Any(x => x.Id == "Banner"))
                {
                    var linkToPage = block.GetValue<PointerPageItem>(BlockFieldNameConstants.Link)?.MapTo<LinkModel>();
                    var banner = new BannerBlockItemViewModel()
                    {
                        ActionText = block.GetValue<string>(BlockFieldNameConstants.BlockText),
                        Image = block.GetValue<Guid>(BlockFieldNameConstants.BlockImagePointer).MapTo<ImageModel>(),
                        LinkUrl = !string.IsNullOrEmpty(linkToPage?.Href) ? linkToPage.Href : ""
                    };
                    result.Add(banner);
                }

                return result;
            }
        }
    }
}
