using Litium.Accelerator.Constants;
using Litium.Blocks;
using Litium.FieldFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.Definitions.Blocks
{
    internal class HeroBlockTemplateSetup : FieldTemplateSetup
    {
        private readonly CategoryService _categoryService;

        public HeroBlockTemplateSetup(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public override IEnumerable<FieldTemplate> GetTemplates()
        {
            var pageCategoryId = _categoryService.Get(BlockCategoryNameConstants.Pages)?.SystemId ?? Guid.Empty;


            var templates = new List<FieldTemplate>
            {
                new BlockFieldTemplate(BlockTemplateNameConstants.Hero)
                {
                    CategorySystemId = pageCategoryId,
                    Icon = "fas fa-image",
                    FieldGroups = new []
                    {
                        new FieldTemplateFieldGroup()
                        {
                            Id = "General",
                            Collapsed = false,
                            Fields =
                            {
                                SystemFieldDefinitionConstants.Name,
                            }
                        },
                        new FieldTemplateFieldGroup()
                        {
                            Id = "Content",
                            Collapsed = false,
                            Fields =
                            {
                               BlockFieldNameConstants.BlockTitle,
                               BlockFieldNameConstants.BlockSubTitle,
                               BlockFieldNameConstants.BlockText,
                               BlockFieldNameConstants.Link,
                               BlockFieldNameConstants.LinkText,
                               BlockFieldNameConstants.ContentPosition,
                               BlockFieldNameConstants.ContentColor,
                               BlockFieldNameConstants.BackgroundImage
                            }
                        }
                    }
                }
            };
            return templates;
        }
    }
}
