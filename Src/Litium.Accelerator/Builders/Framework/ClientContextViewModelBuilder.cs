using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Helpers;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Routing;
using Litium.Accelerator.ViewModels.Framework;
using Litium.Accelerator.ViewModels.Search;
using Litium.FieldFramework.FieldTypes;
using Litium.Foundation.Modules.ECommerce.ShoppingCarts;
using Litium.Globalization;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models;

namespace Litium.Accelerator.Builders.Framework
{
    public class ClientContextViewModelBuilder : IViewModelBuilder<ClientContextViewModel>
    {
        private readonly SiteSettingViewModelBuilder _siteSettingViewModelBuilder;
        private readonly CartViewModelBuilder _cartViewModelBuilder;
        private readonly NavigationViewModelBuilder _navigationViewModelBuilder;
        private readonly RequestModelAccessor _requestModelAccessor;

        private Cart Cart => _requestModelAccessor.RequestModel.Cart;

        public ClientContextViewModelBuilder(RequestModelAccessor requestModelAccessor, SiteSettingViewModelBuilder siteSettingViewModelBuilder, 
            CartViewModelBuilder cartViewModelBuilder, NavigationViewModelBuilder navigationViewModelBuilder)
        {
            _requestModelAccessor = requestModelAccessor;
            _siteSettingViewModelBuilder = siteSettingViewModelBuilder;
            _cartViewModelBuilder = cartViewModelBuilder;
            _navigationViewModelBuilder = navigationViewModelBuilder;
        }

        public ClientContextViewModel Build()
        {
            AntiForgery.GetTokens(null, out string cookieToken, out string formToken);
            return new ClientContextViewModel()
            {
                SiteSetting = _siteSettingViewModelBuilder.Build(),
                Cart = _cartViewModelBuilder.Build(Cart),
                Navigation = _navigationViewModelBuilder.Build(),
                Countries = _requestModelAccessor.RequestModel.ChannelModel.Channel.CountryLinks
                            .Select(link => link.CountrySystemId.MapTo<Country>())
                            .Select(country => new ListItem(new RegionInfo(country.Id).DisplayName, country.Id))
                            .OrderBy(country => country.Text)
                            .ToList(),
                RequestVerificationToken = cookieToken + ":" + formToken,
                QuickSearchUrl = _requestModelAccessor.RequestModel.WebsiteModel.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.SearchResultPage)?.MapTo<LinkModel>()?.Href ?? "",
                Texts = GetClientTexts()
            };
        }

        private IDictionary<string, string> GetClientTexts()
        {
            var webSite = _requestModelAccessor.RequestModel.WebsiteModel;
            if (webSite == null)
            {
                return new Dictionary<string, string>();
            }
            return webSite.Website.Texts.GetTextContainer(CultureInfo.CurrentUICulture).Where(t => t.Key.StartsWith("js.")).ToDictionary(t => t.Key.Replace("js.", string.Empty), t => t.Value);
        }
    }
}
