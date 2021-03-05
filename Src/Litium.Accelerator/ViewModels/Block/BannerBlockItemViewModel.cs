using AutoMapper;
using JetBrains.Annotations;
using Litium.Accelerator.Constants;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models;
using System;
using System.Globalization;

namespace Litium.Accelerator.ViewModels.Block
{
    public class BannerBlockItemViewModel : IAutoMapperConfiguration
    {
        public ImageModel Image { get; set; }
        public string ActionText { get; set; }
        public string LinkUrl { get; set; }

        [UsedImplicitly]
        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<MultiFieldItem, BannerBlockItemViewModel>()
               .ForMember(x => x.ActionText, m => m.MapFrom(c => c.Fields.GetValue<string>(BlockFieldNameConstants.LinkText, CultureInfo.CurrentUICulture)))
               .ForMember(x => x.Image, m => m.MapFrom(c => c.Fields.GetValue<Guid>(BlockFieldNameConstants.BlockImagePointer, CultureInfo.CurrentUICulture).MapTo<ImageModel>()))
               .ForMember(x => x.LinkUrl, m => m.MapFrom<LinkUrlResolver>());
        }

        [UsedImplicitly]
        private class LinkUrlResolver : IValueResolver<MultiFieldItem, BannerBlockItemViewModel, string>
        {
            public string Resolve(MultiFieldItem source, BannerBlockItemViewModel destination, string destMember, ResolutionContext context)
            {
                var linkToPage = source.Fields.GetValue<PointerPageItem>(BlockFieldNameConstants.LinkToPage)?.MapTo<LinkModel>();
                if (!string.IsNullOrEmpty(linkToPage?.Href))
                {
                    return linkToPage.Href;
                }
                return "";
            }
        }
    }
}
