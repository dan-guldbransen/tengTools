using System;
using System.Collections.Generic;

namespace Litium.Accelerator.WebForm.Site.Administration.Api.ViewModel
{
    public class IndexingModel
    {
        public List<Template> Templates { get; set; }

        public class Template
        {
            public string Title { get; set; }
            public string GroupingFieldId { get; set; }
            public List<FieldGroup> Fields { get; set; }
            public string TemplateId { get; set; }
        }

        public class FieldGroup
        {
            public string Title { get; set; }
            public string FieldId { get; set; }
        }
    }
}