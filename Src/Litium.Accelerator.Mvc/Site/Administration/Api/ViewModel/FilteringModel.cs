using System;
using System.Collections.Generic;

namespace Litium.Accelerator.WebForm.Site.Administration.Api.ViewModel
{
    public class FilteringModel
    {
        public List<Item> Items { get; set; }
        public List<Item> Filters { get; set; }

        public class Item
        {
            public string Title { get; set; }
            public string FieldId { get; set; }
            public string GroupName { get; set; }
        }
    }
}