using System;
using System.Collections.Generic;

namespace Litium.Accelerator.WebForm.Site.Administration.Api.ViewModel
{
    public class SearchIndexingModel
    {
        public List<TemplateGroup> GroupedTemplates { get; set; }

        public class Template
        {
            public string Title { get; set; }
            public List<FieldGroup> Fields { get; set; }
            public List<FieldGroup> SelectedFields { get; set; } = new List<FieldGroup>();
            public string TemplateId { get; set; }
            public Type AreaType { get; internal set; }
        }

        public class FieldGroup
        {
            public string Title { get; set; }
            public string FieldId { get; set; }
        }

        public class TemplateGroup
        {
            public string Title { get; set; }
            public List<Template> Templates { get; set; }
        }
    }
}