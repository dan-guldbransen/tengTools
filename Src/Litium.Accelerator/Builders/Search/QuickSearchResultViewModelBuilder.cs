using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Searchers;
using Litium.Accelerator.ViewModels.Search;
using Litium.FieldFramework.FieldTypes;
using Litium.Runtime.AutoMapper;
using Litium.Studio.Extenssions;
using Litium.Web.Models;

namespace Litium.Accelerator.Builders.Search
{
    public class QuickSearchResultViewModelBuilder : IViewModelBuilder<QuickSearchResultViewModel>
    {
        private readonly IEnumerable<BaseSearcher> _searchers;
        private readonly RequestModelAccessor _requestModelAccessor;

        public QuickSearchResultViewModelBuilder(IEnumerable<BaseSearcher> searchers, RequestModelAccessor requestModelAccessor)
        {
            _searchers = searchers;
            _requestModelAccessor = requestModelAccessor;
        }

        public virtual QuickSearchResultViewModel Build(string query)
        {
            if (string.IsNullOrWhiteSpace(query?.Trim()))
            {
                return new QuickSearchResultViewModel();
            }
            var result = new List<SearchItem>();
            var website = _requestModelAccessor.RequestModel.WebsiteModel;
            _requestModelAccessor.RequestModel.SearchQuery.CategorySystemId = null;
            foreach (var searcher in _searchers.OrderBy(s => s.SortOrder))
            {
                var searchResult = searcher.QueryCompact(query);
                if (searchResult == null || searchResult.Total <= 0)
                {
                    continue;
                }

                var items = searcher.ToSearchItems(searchResult.Items.Value).ToList();
                var categoryStr = website.Texts.GetValue("quicksearchheader." + searcher.ModelKey) ?? searcher.ModelKey;
                items.ForEach(c => c.Category = categoryStr);

                result.AddRange(items);
            }
            
            if (result.Any())
            {
                result.Add(new SearchItem()
                {
                    Category = "ShowAll",
                    Name = "search.showall".AsWebSiteString(),
                    ShowAll = true
                });
            }
            else
            {
                result.Add(new SearchItem()
                {
                    Category = "NoHit",
                    Name = "search.nohit".AsWebSiteString(),
                });
            }

            return new QuickSearchResultViewModel() { Results = result };
        }
    }
}
