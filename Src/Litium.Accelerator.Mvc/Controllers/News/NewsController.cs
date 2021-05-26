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
        public ActionResult List(int page = 1)
        {
            var model = _newsListViewModelBuilder.Build(page);
            return View(model);
        }

        [HttpPost]
        public ActionResult List(string tag)
        {
            List<string> tagsToLoad;
            if (Session["TagsToLoad"] != null)
            {
                tagsToLoad = (List<string>)Session["TagsToLoad"];
            }
            else
            {
                tagsToLoad = new List<string>();
            };

            var model = _newsListViewModelBuilder.Build(1);

            if (tag == "Clear selection")
            {
                tagsToLoad.Clear();
                Session["TagsToLoad"] = tagsToLoad;
                model.TagsToLoad = model.BlogTags;
                return View(model);
            }
            if (!tagsToLoad.Contains(tag))
            {
                tagsToLoad.Add(tag);
            }

            Session["TagsToLoad"] = tagsToLoad;
            model.TagsToLoad = tagsToLoad;

            return View(model);
        }
    }
}