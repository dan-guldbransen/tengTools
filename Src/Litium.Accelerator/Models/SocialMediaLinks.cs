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
using Litium.Web.Models.Globalization;

namespace Litium.Accelerator.Models
{
    public class SocialMediaLinks
    {
        public string TwitterURL { get; set; }
        public string LinkedInURL { get; set; }
        public string FacebookURL { get; set; }
        public string YoutubeURL { get; set; }
        public string InstagramURL { get; set; }
    }
}
