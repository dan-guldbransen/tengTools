using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Litium.Common;
using Litium.Events;
using Litium.Foundation.Configuration;
using Litium.Runtime;
using Litium.Runtime.DependencyInjection;
using Litium.Studio.Extenssions;
using Microsoft.Extensions.Logging;

namespace Litium.Accelerator.Search.Indexing.PopularProducts
{
    [Service(ServiceType = typeof(MostSoldDataHolder), Lifetime = DependencyLifetime.Singleton)]
    public class MostSoldDataHolder
    {
        private readonly RetryableLazy<MostSoldDataHolderData> _mostSoldData;

        public MostSoldDataHolder(
            IApplicationLifetime applicationLifetime,
            EventBroker eventBroker,
            ILogger<MostSoldDataHolder> logger)
        {
            var filePath = new FileInfo(Path.Combine(GeneralConfig.Instance.SearchDirectory, "MostSoldFiltering.bin"));
            _mostSoldData = new RetryableLazy<MostSoldDataHolderData>(() =>
            {
                var items = filePath.LoadData<MostSoldDataHolderData>();

                if (items is null)
                {
                    logger.LogTrace("Persisted data does not exists in {path}", filePath.FullName);

                    items = new MostSoldDataHolderData
                    {
                        Data = new ConcurrentDictionary<string, ConcurrentDictionary<Guid, decimal>>(StringComparer.OrdinalIgnoreCase)
                    };

                    Task.Run(async () =>
                    {
                        await Task.Delay(TimeSpan.FromMinutes(5));
                        if (items.Data.Count > 0)
                        {
                            return;
                        }

                        eventBroker.Publish(new MostSoldCollector.CollectMostSoldProducts());
                    });
                }
                else
                {
                    logger.LogTrace("Loaded persisted data from {path}", filePath.FullName);
                }

                return items;
            });

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                if (_mostSoldData.IsValueCreated)
                {
                    logger.LogTrace("Persisting to {path}", filePath.FullName);
                    filePath.Persist(_mostSoldData.Value);
                    logger.LogTrace("Persisted");
                }
                else
                {
                    logger.LogTrace("No data exists, no need to persist");
                }
            });
        }

        public bool AddOrUpdate(string articleNumber, Guid webSiteId, decimal count)
        {
            var isChanged = false;

            _mostSoldData.Value.Data
                         .GetOrAdd(articleNumber, _ => new ConcurrentDictionary<Guid, decimal>())
                         .AddOrUpdate(webSiteId, _ =>
                         {
                             isChanged = true;
                             return count;
                         }, (_, o) =>
                         {
                             if (o == count)
                             {
                                 return o;
                             }

                             isChanged = true;
                             return count;
                         });

            return isChanged;
        }

        public Dictionary<string, HashSet<Guid>> GetCurrentArticleNumbers()
        {
            return _mostSoldData.Value.Data.ToDictionary(x => x.Key, x => new HashSet<Guid>(x.Value.Keys));
        }

        public void Remove(string articleNumber, Guid iitem)
        {
            if (_mostSoldData.Value.Data.TryRemove(articleNumber, out var item))
            {
                item.TryRemove(iitem, out _);
            }
        }

        public bool TryGet(string articleNumber, out IDictionary<Guid, decimal> items)
        {
            if (_mostSoldData.Value.Data.TryGetValue(articleNumber, out var item))
            {
                items = item;
                return true;
            }

            items = null;
            return false;
        }

        [Serializable]
        private class MostSoldDataHolderData
        {
            public ConcurrentDictionary<string, ConcurrentDictionary<Guid, decimal>> Data { get; set; }
        }
    }
}
