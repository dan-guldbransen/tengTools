using System.Web.Mvc;
using Litium.Accelerator.ViewModels.Framework;
using Litium.Accelerator.Builders.Framework;
using Litium.Web.Models.Websites;
using Litium.Web.Models.Products;
using System.Web;

namespace Litium.Accelerator.Mvc.Controllers.Framework
{
    /// <summary>
    /// LayoutController renders views for the layout (header,foter,BreadCrumbs)
    /// </summary>
    public class LayoutController : ControllerBase
    {
        private readonly HeadViewModelBuilder<HeadViewModel> _headViewModelBuilder;
        private readonly HeaderViewModelBuilder<HeaderViewModel> _headerViewModelBuilder;
        private readonly BreadCrumbsViewModelBuilder<BreadCrumbsViewModel> _breadCrumbsViewModelBuilder;
        private readonly FooterViewModelBuilder _footerViewModelBuilder;
        private readonly BodyViewModelBuilder _bodyViewModelBuilder;
        private readonly UtilityMenuViewModelBuilder<UtilityMenuViewModel> _utilityMenuViewModelBuilder;

        public LayoutController(
            BreadCrumbsViewModelBuilder<BreadCrumbsViewModel> breadCrumbsViewModelBuilder,
            HeadViewModelBuilder<HeadViewModel> headViewModelBuilder,
            HeaderViewModelBuilder<HeaderViewModel> headerViewModelBuilder,
            FooterViewModelBuilder footerViewModelBuilder,
            BodyViewModelBuilder bodyViewModelBuilder,
            UtilityMenuViewModelBuilder<UtilityMenuViewModel> utilityMenuViewModelBuilder)
        {
            _breadCrumbsViewModelBuilder = breadCrumbsViewModelBuilder;
            _headViewModelBuilder = headViewModelBuilder;
            _headerViewModelBuilder = headerViewModelBuilder;
            _footerViewModelBuilder = footerViewModelBuilder;
            _bodyViewModelBuilder = bodyViewModelBuilder;
            _utilityMenuViewModelBuilder = utilityMenuViewModelBuilder;
        }

        [ChildActionOnly]
        public ActionResult Head(WebsiteModel currentWebsiteModel, PageModel currentPageModel, CategoryModel currentCategory, ProductModel currentProductModel)
        {
            var viewModel = _headViewModelBuilder.Build(currentWebsiteModel, currentPageModel, currentCategory, currentProductModel);
            return PartialView("Framework/Head", viewModel);
        }

        /// <summary>
        /// Builds header for the site
        /// </summary>
        /// <returns>Return view for the header</returns>
        [ChildActionOnly]
        public ActionResult Header()
        {
            var viewModel = _headerViewModelBuilder.Build();
            return PartialView("Framework/Header", viewModel);
        }

        [ChildActionOnly]
        public ActionResult BodyEnd(WebsiteModel currentWebsiteModel, PageModel currentPageModel, CategoryModel currentCategory, ProductModel currentProductModel)
        {
            var viewModel = _bodyViewModelBuilder.BuildBodyEnd(currentWebsiteModel, currentPageModel, currentCategory, currentProductModel);
            return PartialView("Framework/BodyEnd", viewModel);
        }

        [ChildActionOnly]
        public ActionResult BodyStart(WebsiteModel currentWebsiteModel, PageModel currentPageModel, CategoryModel currentCategory, ProductModel currentProductModel)
        {
            var viewModel = _bodyViewModelBuilder.BuildBodyStart(currentWebsiteModel, currentPageModel, currentCategory, currentProductModel);
            return PartialView("Framework/BodyStart", viewModel);
        }

        /// <summary>
        /// Builds bread crumbs for the site
        /// </summary>
        /// <param name="currentPageModel">The current page</param>
        /// <param name="categoryModel">The current category</param>
        /// <param name="currentProductModel">The current product</param>
        /// <param name="startLevel">Defines from which level the breadcrumbs will be rendered</param>
        /// <returns>Return view for the bread crumbs</returns>
        [ChildActionOnly]
        public ActionResult BreadCrumbs(PageModel currentPageModel, CategoryModel categoryModel, ProductModel currentProductModel, int startLevel = 0)
        {
            var viewModel = _breadCrumbsViewModelBuilder.BuildBreadCrumbs(currentPageModel, categoryModel, currentProductModel, startLevel);
            return PartialView("Framework/BreadCrumbs", viewModel);
        }

        [ChildActionOnly]
        public ActionResult Footer()
        {
            var viewModel = _footerViewModelBuilder.Build();
            return PartialView("Framework/Footer", viewModel);
        }

        /// <summary>
        /// Builds utility menu with marketselector
        /// </summary>
        /// <returns>Return utility menu top of site</returns>
        [ChildActionOnly]
        public ActionResult UtilityMenu()
        {
            var viewModel = _utilityMenuViewModelBuilder.Build();
            return PartialView("Framework/UtilityMenu", viewModel);
        }

        /// <summary>
        /// Builds utility menu with marketselector
        /// </summary>
        /// <returns>Return utility menu top of site</returns>
        [ChildActionOnly]
        public ActionResult CookieNotificationMessage()
        {
            var accepted = ReadCookieValue();

            return PartialView("Framework/CookieNotificationMessage", accepted);
        }

        [HttpPost]
        public ActionResult CookieNotificationMessage(bool accepted)
        {
            SetCookieValue(accepted);
            return new HttpStatusCodeResult(201, accepted.ToString());
        }

        private bool ReadCookieValue()
        {
            var cookiesAcceptedCookie = Request.Cookies[Constants.CookieNotificationMessage.CookieName];
            if(cookiesAcceptedCookie == null)
            {
                cookiesAcceptedCookie = new HttpCookie(Constants.CookieNotificationMessage.CookieName);
                cookiesAcceptedCookie.Expires.Add(Constants.CookieNotificationMessage.Expires);
                cookiesAcceptedCookie[Constants.CookieNotificationMessage.UserAcceptedValue] = "false";

                Response.Cookies.Add(cookiesAcceptedCookie);
            }
            
            bool.TryParse(cookiesAcceptedCookie[Constants.CookieNotificationMessage.UserAcceptedValue].ToString(), out var value);

            return value;
        }

        private bool SetCookieValue(bool accepted = false)
        {
            Response.Cookies[Constants.CookieNotificationMessage.CookieName].Value = accepted.ToString();
            return accepted;
        }
    }
}