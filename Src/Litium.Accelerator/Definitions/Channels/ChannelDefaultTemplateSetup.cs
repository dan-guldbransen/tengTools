using Litium.Accelerator.Constants;
using Litium.FieldFramework;
using Litium.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.Definitions.Channels
{
    public class ChannelDefaultTemplateSetup : FieldTemplateSetup
    {
        public override IEnumerable<FieldTemplate> GetTemplates()
        {
            var template = new ChannelFieldTemplate("DefaultChannel")
            {
                FieldGroups = new[]
                {
                    new FieldTemplateFieldGroup()
                    {
                        Id = "General",
                        Collapsed = false,
                        Fields =
                        {
                            SystemFieldDefinitionConstants.Name
                        }
                    },
                    new FieldTemplateFieldGroup()
                    {
                        Id = "SocialMedia",
                        Collapsed = false,
                        Fields =
                        {
                            ChannelFieldNameConstants.InstagramLink,
                            ChannelFieldNameConstants.Youtube,
                            ChannelFieldNameConstants.Twitter,
                            ChannelFieldNameConstants.Facebook,
                            ChannelFieldNameConstants.LinkedIn
                        }
                    },
                }
            };

            return new List<FieldTemplate>() { template };
        }
    }
}
