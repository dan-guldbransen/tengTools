using Litium.Accelerator.Constants;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.Definitions.Channels
{
    public class ChannelFieldDefinitionSetup : FieldDefinitionSetup
    {
        public override IEnumerable<FieldDefinition> GetFieldDefinitions()
        {
            var fields = new[]
            {
                  new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.ContactName, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                    Option = new TextOption()
                },
                    new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.ContactStreet, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                    Option = new TextOption()
                },
                    new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.ContactAddressLine2, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                    Option = new TextOption()
                },
                    new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.ContactZipCode, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                    Option = new TextOption()
                },
                    new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.ContactCity, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                    Option = new TextOption()
                },
                    new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.ContactCounty, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                    Option = new TextOption()
                },
                    new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.ContactPhone, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                    Option = new TextOption()
                },
                    new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.ContactEmail, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                    Option = new TextOption()
                },
                    new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.ContactWebsite, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                    Option = new TextOption()
                },
                     new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.ContactLat, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                    Option = new TextOption()
                }, new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.ContactLong, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                    Option = new TextOption()
                },

                 new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.Instagram, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                    Option = new TextOption()
                },
                  new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.Youtube, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                    Option = new TextOption()
                },
                   new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.Twitter, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                    Option = new TextOption()
                },
                    new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.Facebook, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                    Option = new TextOption()
                },
                     new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.LinkedIn, SystemFieldTypeConstants.Text)
                {
                    CanBeGridColumn = false,
                    CanBeGridFilter = false,
                    MultiCulture = false,
                    Option = new TextOption()
                },
                new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.FlagIcon, SystemFieldTypeConstants.MediaPointerImage),
                new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.IsInternational, SystemFieldTypeConstants.Boolean),
            };

            return fields;
        }
    }
}
