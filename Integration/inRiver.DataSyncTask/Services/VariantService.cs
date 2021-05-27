using inRiver.DataSyncTask.Constants;
using inRiver.DataSyncTask.Models.Litium;
using inRiver.DataSyncTask.Utils;
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
        public static void ProcessVariants(List<Entity> items, Data data, List<string> cultures, List<PimLitiumFieldMap> variantFields)
        {
            foreach(var item in items)
            {
                if (string.IsNullOrEmpty(item.GetField(InRiver.InRiverField.Item.ItemId).Data?.ToString()))
                    continue;

                var baseProduct = data.Products.FirstOrDefault(p => p.ProductÍtemIds.Contains(item.Id));

                if(baseProduct == null)
                    continue;

                var litiumVariant = new Variant(item, baseProduct.ArticleNumber);

                if (string.IsNullOrEmpty(item.GetField(InRiver.InRiverField.Item.ItemApprovedForMarket).Data?.ToString()))
                    continue;
                var markets = item.GetField(InRiver.InRiverField.Item.ItemApprovedForMarket).Data.ToString();
                var litiumMarkets = MapToLitiumChannelCultures(markets);

                ExtractFieldData(item, litiumVariant, cultures, litiumMarkets, variantFields);

                if (data.Variants == null)
                    data.Variants = new List<Variant>();

                data.Variants.Add(litiumVariant);
            }
        }

        private static void ExtractFieldData(Entity item, Variant litiumVariant, List<string> cultures, List<string> litiumMarkets, List<PimLitiumFieldMap> variantFields)
        {
            // Markets -> published to channels in the create/update event
            litiumVariant.Fields.Add(new Models.Litium.Field
            {
                FieldDefinitionId = "ItemApprovedForMarket",
                Culture = null,
                Value = string.Join(",", litiumMarkets)
            });

            // Map fields dynamically
            foreach (var field in variantFields)
            {
                if (field.IsMultiLingual)
                    MapLocaleStringField(item, litiumVariant, field, cultures);
                else
                    MapField(item, litiumVariant, field);
            }
        }

        private static void MapField(Entity item, Variant litiumVariant, PimLitiumFieldMap field)
        {
            var inRiverData = item.GetField(field.InRiverFieldId)?.Data?.ToString();

            if (string.IsNullOrEmpty(inRiverData))
                return;

            litiumVariant.Fields.Add(new Models.Litium.Field
            {
                FieldDefinitionId = field.LitiumFieldId,
                Culture = null,
                Value = inRiverData
            });
        }

        private static void MapLocaleStringField(Entity item, Variant litiumVariant, PimLitiumFieldMap field, List<string> cultures)
        {
            var inRiverData = item.GetField(field.InRiverFieldId)?.Data;

            if (inRiverData == null)
                return;

            if (!(inRiverData is LocaleString values))
                return;

            // Foreach culture in Litium we try map over values from inRiver localestring
            foreach (var culture in cultures)
            {
                // all english mapped from en-UK in inRiver
                if (culture.Contains("en"))
                {
                    litiumVariant.Fields.Add(new Models.Litium.Field
                    {
                        FieldDefinitionId = field.LitiumFieldId,
                        Culture = culture,
                        Value = values[new System.Globalization.CultureInfo("en-GB")]
                    });
                }
                else
                {
                    var value = values[new System.Globalization.CultureInfo(culture)] ?? values[new System.Globalization.CultureInfo("en-GB")];
                    litiumVariant.Fields.Add(new Models.Litium.Field
                    {
                        FieldDefinitionId = field.LitiumFieldId,
                        Culture = culture,
                        Value = value
                    });
                }
            }
        }

        // TODO refactor to json for quickadd
        private static List<string> MapToLitiumChannelCultures(string markets)
        {
            var retval = new List<string>() { "International " };
            var marketList = markets.Split(';');
            foreach (var market in marketList)
            {
                switch (market.ToUpper())
                {
                    case "AU":
                        retval.Add("en-au");
                        break;
                    case "IE":
                        retval.Add("en-ie");
                        break;
                    case "GB":
                        retval.Add("en-gb");
                        break;
                    case "NO":
                        retval.Add("nb-no");
                        break;
                    case "PL":
                        retval.Add("pl-pl");
                        break;
                    case "SE":
                        retval.Add("sv-se");
                        break;
                    default:
                        break;
                }
            }

            return retval;
        }
    }
}
