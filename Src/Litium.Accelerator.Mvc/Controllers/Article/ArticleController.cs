using Litium.Web.Models.Websites;
using System.Web.Mvc;
using Litium.Accelerator.Builders.Article;

namespace Litium.Accelerator.Mvc.Controllers.Article
{
    public class ArticleController : ControllerBase
    {
        private readonly ArticleViewModelBuilder _articleViewModelBuilder;

        public ArticleController(ArticleViewModelBuilder articleViewModelBuilder)
        {
            _articleViewModelBuilder = articleViewModelBuilder;
        }

        [HttpGet]
        public ActionResult Index(PageModel currentPageModel)
        {
            var model = _articleViewModelBuilder.Build(currentPageModel);
            return View(model);
        }
    }
}