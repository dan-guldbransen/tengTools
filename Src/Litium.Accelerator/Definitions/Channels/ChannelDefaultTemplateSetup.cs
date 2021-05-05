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
                            SystemFieldDefinitionConstants.Name,
                            ChannelFieldNameConstants.FlagIcon
                        }
                    },
                    new FieldTemplateFieldGroup()
                    {
                        Id = "SocialMedia",
                        Collapsed = false,
                        Fields =
                        {
                            ChannelFieldNameConstants.Instagram,
                            ChannelFieldNameConstants.Youtube,
                            ChannelFieldNameConstants.Twitter,
                            ChannelFieldNameConstants.Facebook,
                            ChannelFieldNameConstants.LinkedIn

                        }
                    },
                     new FieldTemplateFieldGroup()
                    {
                        Id = "ContactInformation",
                        Collapsed = true,
                        Fields =
                        {
                            ChannelFieldNameConstants.ContactName,
                            ChannelFieldNameConstants.ContactStreet,
                            ChannelFieldNameConstants.ContactAddressLine2,
                            ChannelFieldNameConstants.ContactZipCode,
                            ChannelFieldNameConstants.ContactCity,
                            ChannelFieldNameConstants.ContactCounty,
                            ChannelFieldNameConstants.ContactPhone,
                            ChannelFieldNameConstants.ContactEmail,
                            ChannelFieldNameConstants.ContactWebsite,
                            ChannelFieldNameConstants.ContactLat,
                            ChannelFieldNameConstants.ContactLong
                        }
                    },
                }
            };

            return new List<FieldTemplate>() { template };
        }
    }
}
