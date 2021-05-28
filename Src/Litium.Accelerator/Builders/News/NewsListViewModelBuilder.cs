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

        public virtual NewsListViewModel Build(string[] tags, int pageIndex)
            => Build(_requestModelAccessor.RequestModel.CurrentPageModel, tags, pageIndex);

        public virtual NewsListViewModel Build(PageModel pageModel, string[] tags, int pageIndex)
        {
            var model = pageModel.MapTo<NewsListViewModel>();
            model.FeaturedBlog = GetFeaturedBlog(pageModel, tags);
            model.News = GetNews(pageModel, tags, model.NumberOfNewsPerPage, pageIndex, out var totalCount);
            model.Pagination = new PaginationViewModel(totalCount, pageIndex, model.NumberOfNewsPerPage);
            model.BlogTags = GetTags(tags, pageIndex, pageModel);
            model.BaseUrl = _urlService.GetUrl(pageModel.Page);

            return model;
        }

        private NewsViewModel GetFeaturedBlog(PageModel pageModel, string[] tags)
        {
            var news = new List<NewsViewModel>();

            var childPages = _pageService.GetChildPages(pageModel.SystemId, pageModel.Page.WebsiteSystemId);
            foreach (var childPage in childPages.Where(p => p.IsActive(_requestModelAccessor.RequestModel.ChannelModel.SystemId)))
            {
                var newsModel = childPage.MapTo<PageModel>().MapTo<NewsViewModel>();
                if (newsModel == null)
                {
                    continue;
                }

                if (tags.Any() && !tags.Contains(newsModel.BlogTags))
                {
                    continue;
                }

                newsModel.Url = _urlService.GetUrl(childPage);
                news.Add(newsModel);
            }

            var featuredBlog = news.Where(x => x.FeaturedBlog == true).FirstOrDefault();
            return featuredBlog;
        }

        private List<NewsViewModel> GetNews(PageModel pageModel, string[] tags, int pageSize, int pageIndex, out int totalCount)
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

                if (tags.Any() && !tags.Contains(newsModel.BlogTags))
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

        private IList<BlogTag> GetTags(string[] tags, int pageIndex, PageModel pageModel) //TODO: add page as parameter so we can add it to url
        {
            var retval = new List<BlogTag>();

            var field = _fieldDefinitionService.Get<WebsiteArea>(AcceleratorWebsiteFieldNameConstants.BlogTagList);
            var options = field.Option as TextOption;

            if (options != null && options.Items != null && options.Items.Any())
            {
                if (tags.Any())
                {
                    retval = options.Items.Select(option =>
                    new BlogTag
                    {
                        Value = option.Value,
                        Text = option.Name[CultureInfo.CurrentUICulture.Name],
                        IsActive = tags?.Contains(option.Value) ?? false,
                        Url = _urlService.GetUrl(pageModel.Page) + $"?tags={string.Join(",", tags)},{option.Value}"
                    }).ToList();
                }
                else
                {
                    retval = options.Items.Select(option =>
                    new BlogTag
                    {
                        Value = option.Value,
                        Text = option.Name[CultureInfo.CurrentUICulture.Name],
                        IsActive = tags?.Contains(option.Value) ?? false,
                        Url = _urlService.GetUrl(pageModel.Page) + $"?tags={option.Value}"
                    }).ToList();
                }

            }

            return retval;
        }
    }
}
