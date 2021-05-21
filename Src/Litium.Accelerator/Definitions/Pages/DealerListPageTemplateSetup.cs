using Litium.Accelerator.Constants;
using Litium.FieldFramework;
using Litium.Websites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.Definitions.Pages
{
    internal class DealerListPageTemplateSetup : FieldTemplateSetup
    {
        public override IEnumerable<FieldTemplate> GetTemplates()
        {
            var templates = new List<FieldTemplate>
            {
                new PageFieldTemplate(PageTemplateNameConstants.DealerList)
                {
                    IndexThePage = true,
                    TemplatePath = "",
                    FieldGroups = new []
                    {
                        new FieldTemplateFieldGroup()
                        {
                            Id = "General",
                            Collapsed = false,
                            Fields =
                            {
                                SystemFieldDefinitionConstants.Name,
                                SystemFieldDefinitionConstants.Url,
                                PageFieldNameConstants.PageSize,
                                PageFieldNameConstants.TitleFilterSelector,
                            }
                        }
                    }
                },
            };
            return templates;
        }
    }
}
