using Litium.Accelerator.Search;
using Litium.Foundation.Modules.ECommerce.ShoppingCarts;
using Litium.Runtime.AutoMapper;
using Litium.Web.Models.Globalization;
using Litium.Web.Models.Websites;
using Litium.Websites;
using System;

namespace Litium.Accelerator.Routing
{
    public abstract class RequestModel
    {
        private readonly Lazy<WebsiteModel> _websiteModel;
        private readonly Lazy<CountryModel> _countryModel;
        private readonly CartAccessor _cartAccessor;

        protected RequestModel(CartAccessor cartAccessor)
        {
            _websiteModel = new Lazy<WebsiteModel>(() => ChannelModel.Channel.WebsiteSystemId.GetValueOrDefault().MapTo<Website>().MapTo<WebsiteModel>());
            _countryModel = new Lazy<CountryModel>(() => Cart.OrderCarrier.CountryID.MapTo<CountryModel>());
            _cartAccessor = cartAccessor;
        }

        public abstract ChannelModel ChannelModel { get; }
        public abstract PageModel CurrentPageModel { get; }
        public abstract SearchQuery SearchQuery { get; }
        public virtual WebsiteModel WebsiteModel => _websiteModel.Value;
        public Cart Cart => _cartAccessor.Cart;
        public virtual CountryModel CountryModel => _countryModel.Value;
        public DateTimeOffset DateTimeUtc { get; } = DateTimeOffset.UtcNow;
    }
}
