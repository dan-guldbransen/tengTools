using AutoMapper;
using JetBrains.Annotations;
using Litium.Accelerator.Builders;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models;
using Litium.Web.Models.Blocks;
using System;

namespace Litium.Accelerator.ViewModels.Block
{
    public class HeroBlockViewModel : IViewModel, IAutoMapperConfiguration
    {
        public Guid SystemId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public ImageModel BackgroundImage { get; set; }
        public string ContentPosition { get; set; }
        public string LinkText { get; set; }
        public LinkModel LinkUrl { get; set; }

        [UsedImplicitly]
        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<BlockModel, HeroBlockViewModel>()
               .ForMember(x => x.Title, m => m.MapFromField(BlockFieldNameConstants.BlockTitle))
               .ForMember(x => x.Text, m => m.MapFromField(BlockFieldNameConstants.BlockText))
               .ForMember(x => x.LinkText, m => m.MapFromField(BlockFieldNameConstants.LinkText));
        }
    }
}
