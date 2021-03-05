using Litium.Runtime.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Litium.Customers;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.FieldFramework.Internal;
using Litium.Globalization;
using Litium.Runtime;
using Litium.Security;
using Litium.Web.Administration.FieldFramework;
using Litium.Websites;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Litium.Products;
using Litium.Foundation.Modules.ECommerce;

namespace Litium.Accelerator.Deployments
{
    [Service(ServiceType = typeof(StructureInfoService))]
    public class StructureInfoService
    {
        private readonly FieldDefinitionService _fieldDefinitionService;
        private readonly JsonSerializer _jsonSerializer;
        private readonly GroupService _groupService;
        private readonly WebsiteService _websiteService;
        private readonly ChannelService _channelService;
        private readonly InventoryService _inventoryService;
        private readonly PriceListService _priceListService;
        private readonly FieldTypeMetadataService _fieldTypeMetadataService;

        public StructureInfoService(
            FieldDefinitionService fieldDefinitionService,
            GroupService groupService,
            WebsiteService websiteService,
            ChannelService channelService,
            InventoryService inventoryService,
            PriceListService priceListService,
            FieldTypeMetadataService fieldTypeMetadataService,
            ApplicationJsonConverter applicationJsonConverter)
        {
            _fieldDefinitionService = fieldDefinitionService;
            _groupService = groupService;
            _websiteService = websiteService;
            _channelService = channelService;
            _inventoryService = inventoryService;
            _priceListService = priceListService;
            _fieldTypeMetadataService = fieldTypeMetadataService;
            _jsonSerializer = JsonSerializer.Create(applicationJsonConverter.GetSerializerSettings());
        }

        public void AddProperties<TArea>(StructureInfo structureInfo, IFieldFramework oldFieldContainer, IFieldFramework fieldContainer, bool changeUrl = true, IList<string> excludeFields = null)
            where TArea : IArea
        {
            if (oldFieldContainer[SystemFieldDefinitionConstants.Images] is List<Guid> images)
            {
                var newImages = images.Select(structureInfo.Id).ToList();

                fieldContainer[SystemFieldDefinitionConstants.Images] = newImages;
            }

            var fields = Get<TArea>((IFieldContainerAccessor)oldFieldContainer);
            foreach (var field in fields)
            {
                if (excludeFields?.Contains(field.Key) == true)
                {
                    continue;
                }

                var fieldDefinition = _fieldDefinitionService.Get<TArea>(field.Key);
                if (fieldDefinition == null)
                {
                    continue;
                }

                if (fieldDefinition.MultiCulture)
                {
                    foreach (var cultureValue in field.Value)
                    {
                        var value = CorrectValue<TArea>(structureInfo, field.Key, cultureValue.Value, fieldDefinition, changeUrl);
                        fieldContainer[fieldDefinition.Id, cultureValue.Key] =
                            ConvertFromEditValue(
                                new EditFieldTypeConverterArgs(fieldDefinition, new CultureInfo(cultureValue.Key)),
                                value);
                    }
                }
                else
                {
                    if (field.Value.TryGetValue("*", out var value))
                    {
                        var newValue = CorrectValue<TArea>(structureInfo, field.Key, value, fieldDefinition, changeUrl);
                        fieldContainer[fieldDefinition.Id] =
                            ConvertFromEditValue(
                                new EditFieldTypeConverterArgs(fieldDefinition, CultureInfo.CurrentCulture), newValue);
                    }
                    else
                    {
                        fieldContainer.TryRemoveValue(fieldDefinition.Id, out _);
                    }
                }
            }
        }

        private object ConvertFromEditValue(EditFieldTypeConverterArgs args, JToken item)
        {
            var fieldTypeMetadata = _fieldTypeMetadataService.Get(args.FieldDefinition.FieldType);
            var fieldTypeInstance = fieldTypeMetadata.CreateInstance(args.FieldDefinition);
            return args.FieldDefinition.Id.Equals(SystemFieldDefinitionConstants.ThumbnailsMetadata, StringComparison.OrdinalIgnoreCase)
                ? item.ToObject(Type.GetType("System.Collections.Generic.List`1[[Litium.Application.Common.ThumbnailMetadata, Litium.Application]], mscorlib"), _jsonSerializer)
                : fieldTypeInstance.ConvertFromJsonValue(item.ToObject(fieldTypeMetadata.JsonType, _jsonSerializer));
        }

        public virtual JToken ConvertToEditValue(EditFieldTypeConverterArgs args, object item)
        {
            var fieldTypeMetadata = _fieldTypeMetadataService.Get(args.FieldDefinition.FieldType);
            var fieldTypeInstance = fieldTypeMetadata.CreateInstance(args.FieldDefinition);
            return JToken.FromObject(fieldTypeInstance.ConvertToJsonValue(item), _jsonSerializer);
        }

        private Dictionary<string, Dictionary<string, JToken>> Get<TArea>(IFieldContainerAccessor fieldData)
            where TArea : IArea
        {
            return ((IFieldDataContainerAccessor)fieldData.Item).Item.CacheItems
                                                        .Where(x => x.Id != SystemFieldDefinitionConstants.Images)
                                                        .GroupBy(x => x.Id, StringComparer.OrdinalIgnoreCase)
                                                        .ToDictionary(x => x.Key.ToLowerInvariant(), x =>
                                                        {
                                                            var fieldDefinition = _fieldDefinitionService.Get<TArea>(x.Key);
                                                            if (fieldDefinition == null)
                                                            {
                                                                return null;
                                                            }

                                                            return x.ToDictionary(z => z.Culture,
                                                                z =>
                                                                    ConvertToEditValue(
                                                                        new EditFieldTypeConverterArgs(fieldDefinition,
                                                                            z.Culture == "*" ? CultureInfo.CurrentCulture : new CultureInfo(z.Culture)), z.Value));
                                                        });
        }

        private JToken CorrectValue<TArea>(StructureInfo structureInfo, string field, JToken fieldValue, FieldDefinition fieldDefinition, bool changeUrl)
            where TArea : IArea
        {
            if (field.Equals(SystemFieldDefinitionConstants.ThumbnailsMetadata, StringComparison.OrdinalIgnoreCase))
            {
                var channelId = structureInfo.Id(structureInfo.Website.Channel.SystemId).ToString("N");
                foreach (var item in ((JArray)fieldValue))
                {
                    var fileNameSuffix = item.SelectToken("fileNameSuffix");
                    if (fileNameSuffix.Value<string>() == channelId)
                    {
                        fileNameSuffix.Replace(new JValue(channelId));
                    }
                }
                return fieldValue;
            }
            var value = ConvertFromJsonValue(fieldValue);
            switch (value)
            {
                case List<string> _:
                    var items = value as List<string>;
                    for (var i = 0; i < items.Count; i++)
                    {
                        items[i] = CorrectValue(structureInfo, field, items[i], fieldDefinition, changeUrl);
                    }

                    return new JArray(items);
                case string _:
                    return CorrectValue(structureInfo, field, (string)value, fieldDefinition, changeUrl);

                case List<PointerItem> pointerItems:
                    for (var i = 0; i < pointerItems.Count; i++)
                    {
                        pointerItems[i] = CorrectValue(structureInfo, pointerItems[i]);
                    }
                    return JArray.FromObject(pointerItems, _jsonSerializer);
                case List<MultiFieldItem> multiFieldItems:
                    foreach (var item in multiFieldItems)
                    {
                        if (item != null)
                        {
                            AddProperties<TArea>(structureInfo, item.Fields, item.Fields);
                        }
                    }
                    return JArray.FromObject(multiFieldItems, _jsonSerializer);
            }

            return fieldValue;
        }

        private object ConvertFromJsonValue(JToken item)
        {
            switch (item)
            {
                case JArray array:
                    switch (array.First)
                    {
                        case JObject o when o.TryGetValue(nameof(PointerItem.EntitySystemId), StringComparison.OrdinalIgnoreCase, out _):
                        {
                            return array.ToObject<List<PointerItem>>(_jsonSerializer);
                        }
                        case JObject o when o.TryGetValue(nameof(MultiFieldItem.AreaType), StringComparison.OrdinalIgnoreCase, out _):
                        {
                            return array.ToObject<List<MultiFieldItem>>(_jsonSerializer);
                        }
                    }

                    return array.ToObject<List<string>>(_jsonSerializer);
                case JValue value:
                    return value.ToObject<string>(_jsonSerializer);
            }

            return item;
        }

        private string CorrectValue(StructureInfo structureInfo, string field, string fieldValue, FieldDefinition fieldDefinition, bool changeUrl)
        {
            if (changeUrl && field == SystemFieldDefinitionConstants.Url)
            {
                fieldValue = $"{fieldValue}-{structureInfo.Prefix}";
            }
            else
            {
                switch (fieldDefinition.FieldType)
                {
                    case SystemFieldTypeConstants.Text:
                    case SystemFieldTypeConstants.MultirowText:
                    case SystemFieldTypeConstants.Editor:
                        fieldValue = structureInfo.ReplaceText(fieldValue);
                        break;
                    case SystemFieldTypeConstants.MediaPointerFile:
                    case SystemFieldTypeConstants.MediaPointerImage:
                    case SystemFieldTypeConstants.Pointer:
                        fieldValue = structureInfo.Id(new Guid(fieldValue)).ToString();
                        break;
                }
            }

            return fieldValue;
        }

        private PointerItem CorrectValue(StructureInfo structureInfo, PointerItem fieldValue)
        {
            if (fieldValue is PointerPageItem pointerPageItem)
            {
                pointerPageItem.ChannelSystemId = structureInfo.Id(pointerPageItem.ChannelSystemId);
            }
            fieldValue.EntitySystemId = structureInfo.Id(fieldValue.EntitySystemId);
            return fieldValue;
        }

        public ISet<AccessControlEntry> GetAccessControlList(ISet<AccessControlEntry> oldAccessControlList)
        {
            var visitorGroupId = (_groupService.Get<Group>("Visitors") ?? _groupService.Get<Group>("Besökare"))?.SystemId ?? Guid.Empty;
            var visitorGroupPermissions = oldAccessControlList.Where(x => x.GroupSystemId == visitorGroupId);
            return new HashSet<AccessControlEntry>(visitorGroupPermissions);
        }

        public void UpdatePropertyReferences(StructureInfo structureInfo, PackageInfo packageInfo)
        {
            var website = _websiteService.Get(structureInfo.Id(packageInfo.Website.SystemId)).MakeWritableClone();
            AddProperties<WebsiteArea>(structureInfo, structureInfo.Website.Website.Fields, website.Fields, false);
            _websiteService.Update(website);

            var channel = _channelService.Get(packageInfo.Channel.SystemId).MakeWritableClone();
            channel.CountryLinks = structureInfo.Website.Channel.CountryLinks.Select(x => new ChannelToCountryLink(structureInfo.Id(x.CountrySystemId))
            {
                DeliveryMethodSystemIds = x.DeliveryMethodSystemIds.Select(z => ModuleECommerce.Instance.DeliveryMethods.Get(packageInfo.DeliveryMethods.Find(zz => zz.ID == z)?.Name ?? string.Empty, ModuleECommerce.Instance.AdminToken)?.ID ?? Guid.Empty).Where(z => z != Guid.Empty).ToList(),
                PaymentMethodSystemIds = x.PaymentMethodSystemIds.Select(z =>
                {
                    var payment = packageInfo.PaymentMethods.Find(zz => zz.ID == z);
                    if (payment == null)
                    {
                        return Guid.Empty;
                    }
                    return ModuleECommerce.Instance.PaymentMethods.Get(payment.Name, payment.PaymentProviderName, ModuleECommerce.Instance.AdminToken)?.ID ?? Guid.Empty;
                }).Where(z => z != Guid.Empty).ToList(),
            }).ToList();
            AddProperties<GlobalizationArea>(structureInfo, structureInfo.Website.Channel.Fields, channel.Fields, false, new List<string> { SystemFieldDefinitionConstants.Name });
            _channelService.Update(channel);

            var inventory = _inventoryService.Get(packageInfo.Inventory.SystemId).MakeWritableClone();
            inventory.CountryLinks = structureInfo.ProductCatalog.Inventory.CountryLinks.Select(x => new InventoryToCountryLink(structureInfo.Id(x.CountrySystemId))).ToList();
            _inventoryService.Update(inventory);

            var priceList = _priceListService.Get(packageInfo.PriceList.SystemId).MakeWritableClone();
            priceList.CountryLinks = structureInfo.ProductCatalog.PriceList.CountryLinks.Select(x => new PriceListToCountryLink(structureInfo.Id(x.CountrySystemId))).ToList();
            _priceListService.Update(priceList);
        }
    }
}
