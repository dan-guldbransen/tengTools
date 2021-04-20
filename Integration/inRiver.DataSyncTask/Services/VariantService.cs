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

                var markets = item.GetField(InRiver.InRiverField.Item.ItemApprovedForMarket).Data.ToString();
                var litiumMarkets = MapToLitiumChannels(markets);

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

                // Markets -> mapped to channels in the create/update event
                litiumVariant.Fields.Add(new Models.Litium.Field
                {
                    FieldDefinitionId = "ItemApprovedForMarket",
                    Culture = null,
                    Value = string.Join(",", litiumMarkets)
                });

                if (data.Variants == null)
                    data.Variants = new List<Variant>();

                data.Variants.Add(litiumVariant);
            }
        }

        private static List<string> MapToLitiumChannels(string markets)
        {
            var retval = new List<string>() { "International " };
            var marketList = markets.Split(';');
            foreach (var market in marketList)
            {
                switch (market.ToUpper())
                {
                    case "AU":
                        retval.Add("Australia");
                        break;
                    case "IE":
                        retval.Add("Ireland");
                        break;
                    case "GB":
                        retval.Add("UK");
                        break;
                    case "NO":
                        retval.Add("Norway");
                        break;
                    case "PL":
                        retval.Add("Poland");
                        break;
                    case "SE":
                        retval.Add("Sweden");
                        break;
                    default:
                        break;
                }
            }

            return retval;
        }
    }
}
