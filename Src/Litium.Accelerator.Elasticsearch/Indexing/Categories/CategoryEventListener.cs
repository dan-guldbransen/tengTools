﻿using Litium.Events;
using Litium.Search.Indexing;
using Litium.Products;
using Litium.Products.Events;
using System.Collections.Generic;

namespace Litium.Accelerator.Search.Indexing.Categories
{
    public class CategoryEventListener : IIndexQueueHandlerRegistration
    {
        private readonly IndexQueueService _indexQueueService;
        private readonly CategoryService _categoryService;

        public CategoryEventListener(EventBroker eventBroker, IndexQueueService indexQueueService, CategoryService categoryService)
        {
            _indexQueueService = indexQueueService;
            _categoryService = categoryService;

            eventBroker.Subscribe<CategoryCreated>(x => QueueItem(x.Item));
            eventBroker.Subscribe<CategoryUpdated>(x => QueueItem(x.Item));
            eventBroker.Subscribe<CategoryDeleted>(x => _indexQueueService.Enqueue(new IndexQueueItem<CategoryDocument>(x.SystemId) { Action = IndexAction.Delete }));

            eventBroker.Subscribe<CategoryToChannelLinkAdded>(x => QueueItem(x.Item));
            eventBroker.Subscribe<CategoryToChannelLinkRemoved>(x => QueueItem(x.Item));

            eventBroker.Subscribe<CategoryMoved>(x =>
            {
                if (x.CurrentAssortmentSystemId != x.OriginalAssortmentSystemId)
                {
                    var queue = new Stack<Category>();
                    var category = categoryService.Get(x.CategorySystemId);
                    if (category != null)
                    {
                        QueueItem(category);
                        foreach (var item in GetChildCategories(category))
                        {
                            QueueItem(item);
                        }
                    }
                }
            });
        }

        private void QueueItem(Category category)
        {
            _indexQueueService.Enqueue(new IndexQueueItem<CategoryDocument>(category.SystemId));
        }

        private IEnumerable<Category> GetChildCategories(Category category)
        {
            var stack = new Stack<Category>();
            stack.Push(category);
            while (stack.Count > 0)
            {
                var next = stack.Pop();
                yield return next;
                foreach (var child in _categoryService.GetChildCategories(next.SystemId))
                {
                    stack.Push(child);
                }
            }
        }
    }
}
