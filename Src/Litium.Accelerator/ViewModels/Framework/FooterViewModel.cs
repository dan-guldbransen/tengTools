using Litium.Web.Models;
using System.Collections.Generic;
using Litium.Runtime.AutoMapper;
using JetBrains.Annotations;
using Litium.FieldFramework;
using AutoMapper;
using Litium.Accelerator.Constants;
using Litium.FieldFramework.FieldTypes;
using System.Linq;
using System.Globalization;
using Litium.Accelerator.Builders;
using System;
using Litium.Accelerator.Models;

namespace Litium.Accelerator.ViewModels.Framework
{
    public class FooterViewModel : IViewModel
    {
        public virtual List<SectionModel> SectionList { get; set; } = new List<SectionModel>();
        public string TopText { get; set; }
        public LinkModel GetOrganised { get; set; } = new LinkModel();
        public LinkModel Newsletter { get; set; } = new LinkModel();
        public string SocialMediaText { get; set; }
        public string VisionHeader { get; set; }
        public string VisionText { get; set; }
        public string Legal { get; set; }
        public SocialMediaLinks SocialMediaLinks { get; set; } = new SocialMediaLinks();

        // Render logic
        public bool HasCTASection => GetOrganised != null || Newsletter != null;
        public bool HasVisionSection => !string.IsNullOrEmpty(VisionHeader) || !string.IsNullOrEmpty(VisionText);

        public int SectionCol
        {
            get
            {
                if(SectionList != null && SectionList.Any())
                {
                    return SectionList.Count switch
                    {
                        1 => 12,
                        2 => 6,
                        3 => 4,
                        4 => 3,
                        _ => 0
                    };
                }

                return 0;
            }
        }
    }

    public class SectionModel : IAutoMapperConfiguration
    {
        public virtual IList<LinkModel> SectionLinkList { get; set; } = new List<LinkModel>();
        public virtual EditorString SectionText { get; set; }
        public virtual string SectionTitle { get; set; }

        [UsedImplicitly]
        void IAutoMapperConfiguration.Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<MultiFieldItem, SectionModel>()
               .ForMember(x => x.SectionTitle, m => m.MapFrom(c => c.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterHeader, CultureInfo.CurrentUICulture)))
               .ForMember(x => x.SectionLinkList, m => m.MapFrom(c => c.Fields.GetValue<IList<PointerItem>>(AcceleratorWebsiteFieldNameConstants.FooterLinkList).OfType<PointerPageItem>().ToList().Select(x => x.MapTo<LinkModel>()).Where(c => c != null).ToList() ?? new List<LinkModel>()))
               .ForMember(x => x.SectionText, m => m.MapFrom(c => c.Fields.GetValue<string>(AcceleratorWebsiteFieldNameConstants.FooterText, CultureInfo.CurrentUICulture)));
        }
    }
}