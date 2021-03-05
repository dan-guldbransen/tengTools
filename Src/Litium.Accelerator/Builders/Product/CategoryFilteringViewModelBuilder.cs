using System.Collections.Generic;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Extensions;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Search;
using Litium.Accelerator.ViewModels.Brand;
using Litium.Accelerator.ViewModels.Product;
using Litium.Accelerator.ViewModels.Search;
using Litium.Studio.Extenssions;
using Direction = Litium.Framework.Search.SortDirection;

namespace Litium.Accelerator.Builders.Product
{
    public class CategoryFilteringViewModelBuilder : IViewModelBuilder<CategoryFilteringViewModel>
    {
        private readonly RequestModelAccessor _requestModelAccessor;

        public CategoryFilteringViewModelBuilder(RequestModelAccessor requestModelAccessor)
        {
            _requestModelAccessor = requestModelAccessor;
        }

        public CategoryFilteringViewModel Build(int totalHits)
        {
            if (totalHits > 1)
            {
                return new CategoryFilteringViewModel
                {
                    SortItems = GetSortSelection()
                };
            }

            return null;
        }

        private IEnumerable<ListItem> GetSortSelection()
        {
            var sortList = new List<ListItem>();
            var searchQuery = _requestModelAccessor.RequestModel.SearchQuery.Clone();
            var pageTypeName = _requestModelAccessor.RequestModel.CurrentPageModel.GetPageType();
            if (pageTypeName == PageTemplateNameConstants.Brand)
            {
                searchQuery.Tags.Remove(BrandListViewModel.TagName);
            }
            searchQuery.PageType = pageTypeName;
            searchQuery.PageSystemId = _requestModelAccessor.RequestModel.CurrentPageModel.SystemId;

            if (!string.IsNullOrWhiteSpace(searchQuery.Text) || pageTypeName == PageTemplateNameConstants.SearchResult)
            {
                sortList.Add(new ListItem("sort.byscore".AsWebSiteString(), searchQuery.GetUrlSort(string.Empty, Direction.Ascending)));
                sortList.Add(new ListItem("sort.bypopular".AsWebSiteString(), searchQuery.GetUrlSort(SearchQueryConstants.Popular, Direction.Ascending)) { Selected = searchQuery.IsSortedBy(SearchQueryConstants.Popular, Direction.Ascending) });
            }
            else
            {
                sortList.Add(new ListItem("sort.bypopular".AsWebSiteString(), searchQuery.GetUrlSort(string.Empty, Direction.Ascending)));
            }

            sortList.Add(new ListItem("sort.byrecommend".AsWebSiteString(), searchQuery.GetUrlSort(SearchQueryConstants.Recommended, Direction.Ascending)) { Selected = searchQuery.IsSortedBy(SearchQueryConstants.Recommended, Direction.Ascending) });
            sortList.Add(new ListItem("sort.bynews".AsWebSiteString(), searchQuery.GetUrlSort(SearchQueryConstants.News, Direction.Ascending)) { Selected = searchQuery.IsSortedBy(SearchQueryConstants.News, Direction.Ascending) });
            sortList.Add(new ListItem("sort.bynameasc".AsWebSiteString(), searchQuery.GetUrlSort(SearchQueryConstants.Name, Direction.Ascending)) { Selected = searchQuery.IsSortedBy(SearchQueryConstants.Name, Direction.Ascending) });
            sortList.Add(new ListItem("sort.bynamedesc".AsWebSiteString(), searchQuery.GetUrlSort(SearchQueryConstants.Name, Direction.Descending)) { Selected = searchQuery.IsSortedBy(SearchQueryConstants.Name, Direction.Descending) });
            sortList.Add(new ListItem("sort.bypriceasc".AsWebSiteString(), searchQuery.GetUrlSort(SearchQueryConstants.Price, Direction.Ascending)) { Selected = searchQuery.IsSortedBy(SearchQueryConstants.Price, Direction.Ascending) });
            sortList.Add(new ListItem("sort.bypricedesc".AsWebSiteString(), searchQuery.GetUrlSort(SearchQueryConstants.Price, Direction.Descending)) { Selected = searchQuery.IsSortedBy(SearchQueryConstants.Price, Direction.Descending) });

            return sortList;
        }
    }
}
