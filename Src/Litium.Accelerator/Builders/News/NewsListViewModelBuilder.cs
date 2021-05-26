using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels;
using Litium.Accelerator.ViewModels.News;
using Litium.Common;
using Litium.FieldFramework;
using Litium.FieldFramework.FieldTypes;
using Litium.Runtime.AutoMapper;
using Litium.Web;
using Litium.Web.Models.Websites;
using Litium.Websites;

namespace Litium.Accelerator.Builders.News
{
    public class NewsListViewModelBuilder : IViewModelBuilder<NewsListViewModel>
    {
        private readonly PageService _pageService;
        private readonly UrlService _urlService;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly FieldDefinitionService _fieldDefinitionService;

        public NewsListViewModelBuilder(PageService pageService, UrlService urlService, RequestModelAccessor requestModelAccessor, FieldDefinitionService fieldDefinitionService)
        {
            _pageService = pageService;
            _urlService = urlService;
            _requestModelAccessor = requestModelAccessor;
            _fieldDefinitionService = fieldDefinitionService;
        }

        public virtual NewsListViewModel Build(int pageIndex) 
            => Build(_requestModelAccessor.RequestModel.CurrentPageModel, pageIndex);

        public virtual NewsListViewModel Build(PageModel pageModel, int pageIndex)
        {
            var model = pageModel.MapTo<NewsListViewModel>();
            model.News = GetNews(pageModel, model.NumberOfNewsPerPage, pageIndex, out var totalCount);
            model.Pagination = new PaginationViewModel(totalCount, pageIndex, model.NumberOfNewsPerPage);
            model.BlogTags = GetTags();
            model.TagsToLoad = TagsToLoad(model.TagsToLoad, model.BlogTags);

            return model;
        }

        private IList<string> TagsToLoad(IList<string> tagsToLoad, IList<string> blogTags)
        {
            if(tagsToLoad.Count == 0)
            {
                foreach (var tag in blogTags)
                {
                    tagsToLoad.Add(tag);
                }
                return tagsToLoad;
            }
            else
            {                
                return tagsToLoad;
            }
        }

        private List<NewsViewModel> GetNews(PageModel pageModel, int pageSize, int pageIndex, out int totalCount)
        {
            var news = new List<NewsViewModel>();
            totalCount = 0;
            if (pageSize == 0)
            {
                return news;
            }

            var childPages = _pageService.GetChildPages(pageModel.SystemId, pageModel.Page.WebsiteSystemId);
            foreach (var childPage in childPages.Where(p => p.IsActive(_requestModelAccessor.RequestModel.ChannelModel.SystemId)))
            {
                var newsModel = childPage.MapTo<PageModel>().MapTo<NewsViewModel>();
                if (newsModel == null)
                {
                    continue;
                }

                newsModel.Url = _urlService.GetUrl(childPage);
                news.Add(newsModel);
            }

            totalCount = news.Count;
            var orderedNews = news.OrderByDescending(n => n.NewsDate).AsEnumerable();

            var doPageResult = pageSize > 0 && news.Count() > pageSize;
            if (!doPageResult)
            {
                return orderedNews.ToList();
            }
            
            var skip = pageSize * (pageIndex - 1);
            orderedNews = orderedNews.Skip(skip).Take(pageSize);

            return orderedNews.ToList();
        }

        private IList<string> GetTags()
        {
            var field = _fieldDefinitionService.Get<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.BlogTagList);
            var options = field.Option as TextOption;
            var tags = options.Items.Select(option => option.Value);

            return tags.ToList();
        }
    }
}
