using Litium.Web.Models.Websites;
using System.Web.Mvc;
using Litium.Accelerator.Builders.News;
using Litium.Accelerator.Routing;
using Litium.Runtime.AutoMapper;
using Litium.Accelerator.ViewModels.News;
using System.Collections.Generic;

namespace Litium.Accelerator.Mvc.Controllers.News
{
    public class NewsController : ControllerBase
    {
        private readonly NewsViewModelBuilder _newsViewModelBuilder;
        private readonly NewsListViewModelBuilder _newsListViewModelBuilder;
        private readonly RequestModelAccessor _requestModelAccessor;

        public NewsController(NewsViewModelBuilder newsViewModelBuilder, NewsListViewModelBuilder newsListViewModelBuilder, RequestModelAccessor requestModelAccessor)
        {
            _newsViewModelBuilder = newsViewModelBuilder;
            _newsListViewModelBuilder = newsListViewModelBuilder;
            _requestModelAccessor = requestModelAccessor;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var model = _newsViewModelBuilder.Build();
            return View(model);
        }

        [HttpGet]
        public ActionResult List(string tags, int page = 1)
        {
            var tagsList = !string.IsNullOrEmpty(tags) ? tags.Split(',') : new string[0];
            var model = _newsListViewModelBuilder.Build(tagsList, page);
            return View(model);
        }
    }
}