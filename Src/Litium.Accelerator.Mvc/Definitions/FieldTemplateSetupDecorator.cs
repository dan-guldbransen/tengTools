using System;
using System.Collections.Generic;
using System.Linq;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Definitions;
using Litium.Accelerator.Mvc.Controllers.Blocks;
using Litium.FieldFramework;
using Litium.Runtime.AutoMapper;
using Litium.Runtime.DependencyInjection;

namespace Litium.Accelerator.Mvc.Definitions
{
    [ServiceDecorator(typeof(FieldTemplateSetup))]
    public class FieldTemplateSetupDecorator : FieldTemplateSetup
    {
        private readonly FieldTemplateSetup _parent;
        private readonly IDictionary<(Type areaType, string id), (Type controllerType, string action)> _controllerMapping = new Dictionary<(Type areaType, string id), (Type controllerType, string action)>
        {
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.Article)] = (typeof(Controllers.Article.ArticleController), nameof(Controllers.Article.ArticleController.Index)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.ContactUs)] = (typeof(Controllers.ContactUs.ContactUsController), nameof(Controllers.ContactUs.ContactUsController.Index)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.BrandList)] = (typeof(Controllers.Brand.BrandController), nameof(Controllers.Brand.BrandController.List)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.Brand)] = (typeof(Controllers.Brand.BrandController), nameof(Controllers.Brand.BrandController.Index)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.Checkout)] = (typeof(Controllers.Checkout.CheckoutController), nameof(Controllers.Checkout.CheckoutController.Index)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.Error)] = (typeof(Controllers.Error.ErrorController), nameof(Controllers.Error.ErrorController.Error)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.ForgotPassword)] = (typeof(Controllers.Login.LoginController), nameof(Controllers.Login.LoginController.ForgotPassword)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.Home)] = (typeof(Controllers.Home.HomeController), nameof(Controllers.Home.HomeController.Index)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.Landing)] = (typeof(Controllers.LandingPage.LandingPageController), nameof(Controllers.LandingPage.LandingPageController.Category)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.Login)] = (typeof(Controllers.Login.LoginController), nameof(Controllers.Login.LoginController.Login)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.MegaMenu)] = (typeof(Controllers.Article.ArticleController), nameof(Controllers.Article.ArticleController.Index)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.MyPages)] = (typeof(Controllers.MyPages.MyPagesController), nameof(Controllers.MyPages.MyPagesController.Index)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.NewsList)] = (typeof(Controllers.News.NewsController), nameof(Controllers.News.NewsController.List)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.News)] = (typeof(Controllers.News.NewsController), nameof(Controllers.News.NewsController.Index)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.OrderConfirmationEmail)] = (typeof(Controllers.Order.OrderController), nameof(Controllers.Order.OrderController.Order)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.OrderConfirmation)] = (typeof(Controllers.Order.OrderController), nameof(Controllers.Order.OrderController.Confirmation)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.OrderHistory)] = (typeof(Controllers.Order.OrderController), nameof(Controllers.Order.OrderController.List)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.Order)] = (typeof(Controllers.Order.OrderController), nameof(Controllers.Order.OrderController.Order)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.PageNotFound)] = (typeof(Controllers.Error.ErrorController), nameof(Controllers.Error.ErrorController.NotFound)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.ProductList)] = (typeof(Controllers.ProductList.ProductListController), nameof(Controllers.ProductList.ProductListController.Index)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.SearchResult)] = (typeof(Controllers.Search.SearchController), nameof(Controllers.Search.SearchController.Index)),
            [(typeof(Websites.WebsiteArea), PageTemplateNameConstants.WelcomeEmail)] = (typeof(Controllers.Article.ArticleController), nameof(Controllers.Article.ArticleController.Index)),
            [(typeof(Blocks.BlockArea), BlockTemplateNameConstants.Banner)] = (typeof(BannerBlockController), nameof(BannerBlockController.Index)),
            [(typeof(Blocks.BlockArea), BlockTemplateNameConstants.Brand)] = (typeof(BrandBlockController), nameof(BrandBlockController.Index)),
            [(typeof(Blocks.BlockArea), BlockTemplateNameConstants.Product)] = (typeof(ProductBlockController), nameof(ProductBlockController.Index)),
            [(typeof(Blocks.BlockArea), BlockTemplateNameConstants.ProductsAndBanner)] = (typeof(ProductsAndBannerBlockController), nameof(ProductsAndBannerBlockController.Index)),
            [(typeof(Blocks.BlockArea), BlockTemplateNameConstants.Slider)] = (typeof(SliderBlockController), nameof(SliderBlockController.Index)),
            [(typeof(Blocks.BlockArea), BlockTemplateNameConstants.Video)] = (typeof(VideoBlockController), nameof(VideoBlockController.Index))
        };

        public FieldTemplateSetupDecorator(FieldTemplateSetup parent)
        {
            _parent = parent;
        }

        public override IEnumerable<FieldTemplate> GetTemplates()
        {
            return _parent.GetTemplates().Select(x =>
            {
                var prop = x.GetType().GetProperty("TemplatePath");
                if (prop != null && _controllerMapping.TryGetValue((x.AreaType, x.Id), out var map))
                {
                    prop.SetValue(x, "~/MVC:" + map.controllerType.MapTo<string>() + ":" + map.action);
                }
                return x;
            });
        }
    }
}