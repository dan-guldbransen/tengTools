using inRiver.DataSyncTask.Constants;
using inRiver.DataSyncTask.Models.inRiver;
using inRiver.Remoting.Extension;
using inRiver.Remoting.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace inRiver.DataSyncTask.Services
{
    public static class ChannelBuilder
    {
        public static CategoryStructure GetChannelStructure(inRiverContext context, List<Entity> products)
        {
            var model = new CategoryStructure();
            var channels = context.ExtensionManager.DataService.GetAllEntitiesForEntityType(InRiver.EntityType.ChannelTypeId, LoadLevel.DataAndLinks);

            if(channels != null && channels.Any())
            {
                var ecomChannel = channels.FirstOrDefault(x => x.Fields.First(y => y.FieldType.Id == "ChannelName")?.Data.ToString() == "E-Com");

                if(ecomChannel.OutboundLinks == null || !ecomChannel.OutboundLinks.Any(x => x.LinkType.Id == "ChannelChannelNodes"))
                    return model;

                foreach(var channelChannelNode in ecomChannel.OutboundLinks.Where(x => x.LinkType.Id == "ChannelChannelNodes"))
                {
                    var headCategory = context.ExtensionManager.DataService.GetEntity(channelChannelNode.Target.Id, LoadLevel.DataAndLinks);
                    
                    if(headCategory == null)
                        continue;

                    var category = new inRiverCat
                    {
                        Id = headCategory.Id,
                        Name = headCategory.Fields.FirstOrDefault(x => x.FieldType.Id == "ChannelNodeName")?.Data.ToString()
                    };

                    if (headCategory.OutboundLinks != null && headCategory.OutboundLinks.Any(x => x.LinkType.Id == "ChannelNodeChannelNode"))
                        GetSubCategories(category, headCategory.OutboundLinks.Where(x => x.LinkType.Id == "ChannelNodeChannelNode"), context);

                    model.HeadCategories.Add(category);
                }
            }

            return model;
        }

        private static void GetSubCategories(inRiverCat category, IEnumerable<Link> channelChannelNodes, inRiverContext context)
        {
            foreach(var link in channelChannelNodes)
            {
                var entity = context.ExtensionManager.DataService.GetEntity(link.Target.Id, LoadLevel.DataAndLinks);

                if(entity == null)
                    continue;

                var subCategory = new inRiverCat
                {
                    Id = entity.Id,
                    Name = entity.Fields.FirstOrDefault(x => x.FieldType.Id == "ChannelNodeName")?.Data?.ToString() ?? string.Empty,
                    ProductCategoryNumber = entity.Fields.FirstOrDefault(x => x.FieldType.Id == "ChannelNodeProductCategoryNumber")?.Data?.ToString() ?? string.Empty,
                    ProductGroupNumber = entity.Fields.FirstOrDefault(x => x.FieldType.Id == "ChannelNodeGroupNumber")?.Data?.ToString() ?? string.Empty,
                };

                if(entity.OutboundLinks != null && entity.OutboundLinks.Any(x => x.LinkType.Id == "ChannelNodeChannelNode"))
                    GetSubCategories(subCategory, entity.OutboundLinks.Where(x => x.LinkType.Id == "ChannelNodeChannelNode"), context);

                category.Categories.Add(subCategory);
            }
        }
    }
}
