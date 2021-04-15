using inRiver.DataSyncTask.Constants;
using inRiver.DataSyncTask.Models.Litium;
using inRiver.Remoting.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inRiver.DataSyncTask.Services
{
    public static class VariantService
    {
        public static void ProcessVariants(List<Entity> items, Data data, List<string> cultures)
        {
            foreach(var item in items)
            {
                if (string.IsNullOrEmpty(item.GetField(InRiver.InRiverField.Item.ItemId).Data?.ToString()))
                    continue;

                var baseProduct = data.Products.FirstOrDefault(p => p.ProductÍtemIds.Contains(item.Id));

                if(baseProduct == null)
                    continue;

                var litiumVariant = new Variant(item, baseProduct.ArticleNumber);

                // Name
                foreach (var culture in cultures)
                {
                    var description = item.GetField(InRiver.InRiverField.Item.ItemShortDescription).Data as LocaleString;
                    litiumVariant.Fields.Add(new Models.Litium.Field
                    {
                        FieldDefinitionId = LitiumFieldDefinitions.Name,
                        Culture = culture,
                        Value = description[new System.Globalization.CultureInfo(culture)] ?? description[description.Languages.First()]
                    });
                }

                // Other Fields
                foreach (var inRiverField in item.Fields)
                {

                }

                if (data.Variants == null)
                    data.Variants = new List<Variant>();

                data.Variants.Add(litiumVariant);
            }
        }
    }
}
