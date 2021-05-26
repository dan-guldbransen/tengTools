using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.News;
using Litium.Runtime.AutoMapper;
using Litium.Runtime.DependencyInjection;
using Litium.Web;
using Litium.Web.Models.Websites;
using Litium.Websites;
using System.Collections.Generic;
using System.Linq;

namespace Litium.Accelerator.Builders.News
{
    [Service(ServiceType = typeof(NewsViewModelBuilder))]
    public class NewsViewModelBuilder
    {
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly PageService _pageService;
        private readonly UrlService _urlService;

        public NewsViewModelBuilder(PageService pageService, UrlService urlService, RequestModelAccessor requestModelAccessor)
        {
            _pageService = pageService;
            _urlService = urlService;
            _requestModelAccessor = requestModelAccessor;
        }

        public virtual NewsViewModel Build()
            => Build(_requestModelAccessor.RequestModel.CurrentPageModel);

        public virtual NewsViewModel Build(PageModel pageModel)
        {
            var model = pageModel.MapTo<NewsViewModel>();
            model.News = GetNews(pageModel, out var totalCount);
            return model;
        }

        private List<NewsViewModel> GetNews(PageModel pageModel, out int totalCount)
        {
            var news = new List<NewsViewModel>();
            totalCount = 0;
            var parentPageSystemId = pageModel.Page.ParentPageSystemId;
            var childPages = _pageService.GetChildPages(parentPageSystemId, pageModel.Page.WebsiteSystemId);
            foreach (var childPage in childPages.Where(p => p.IsActive(_requestModelAccessor.RequestModel.ChannelModel.SystemId)))
            {
                if(childPage.SystemId == pageModel.SystemId)
                {
                    continue;
                }

                var newsModel = childPage.MapTo<PageModel>().MapTo<NewsViewModel>();
                var currentActiveModel = pageModel.MapTo<NewsViewModel>();
                if (newsModel == null)
                {
                    continue;
                }
                if(newsModel.BlogTags != currentActiveModel.BlogTags)
                {
                    continue;
                }

                newsModel.Url = _urlService.GetUrl(childPage);
                news.Add(newsModel);
            }
            

            totalCount = news.Count;
            var orderedNews = news.OrderByDescending(n => n.NewsDate).AsEnumerable();
            return orderedNews.ToList();
        }
    }
}
