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
                 new FieldDefinition<GlobalizationArea>(ChannelFieldNameConstants.InstagramLink, SystemFieldTypeConstants.Text)
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
            };

            return fields;
        }
    }
}
