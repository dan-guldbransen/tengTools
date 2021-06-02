using System.Collections.Generic;
using AutoMapper;
using Litium.Accelerator.Builders;
using Litium.Accelerator.ViewModels.Search;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models;

namespace Litium.Accelerator.ViewModels.Framework
{
    public class SubNavigationLinkModel : IViewModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsSelected { get; set; }
        public IList<SubNavigationLinkModel> Links { get; set; } = new List<SubNavigationLinkModel>();

        public ImageModel Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CtaLink { get; set; }


        internal class Mapper : IAutoMapperConfiguration
        {
            void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
            {
                cfg.CreateMap<FilterItem, SubNavigationLinkModel>();
            }
        }
    }
}