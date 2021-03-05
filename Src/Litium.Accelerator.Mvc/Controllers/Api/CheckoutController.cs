using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Web.Http;
using Litium.Accelerator.Builders.Checkout;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Mvc.Attributes;
using Litium.Accelerator.Mvc.ModelStates;
using Litium.Accelerator.Routing;
using Litium.Accelerator.Services;
using Litium.Accelerator.ViewModels.Checkout;
using Litium.FieldFramework.FieldTypes;
using Litium.Foundation.Modules.ECommerce.Plugins.Utilities;
using Litium.Foundation.Security;
using Litium.Globalization;
using Litium.Studio.Extenssions;
using Litium.Web;
using Litium.Websites;

namespace Litium.Accelerator.Mvc.Controllers.Api
{
    [RoutePrefix("api/checkout")]
    public class CheckoutController : ApiControllerBase
    {
        private readonly CheckoutService _checkoutService;
        private readonly PaymentMethodViewModelBuilder _paymentMethodViewModelBuilder;
        private readonly RequestModelAccessor _requestModelAccessor;
        private readonly PageService _pageService;
        private readonly UrlService _urlService;
        private readonly DeliveryMethodViewModelBuilder _deliveryMethodViewModelBuilder;
        private readonly CountryService _countryService;

        public CheckoutController(
            CheckoutService checkoutService,
            PaymentMethodViewModelBuilder paymentMethodViewModelBuilder,
            UrlService urlService,
            PageService pageService,
            RequestModelAccessor requestModelAccessor, DeliveryMethodViewModelBuilder deliveryMethodViewModelBuilder, CountryService countryService)
        {
            _checkoutService = checkoutService;
            _paymentMethodViewModelBuilder = paymentMethodViewModelBuilder;
            _requestModelAccessor = requestModelAccessor;
            _deliveryMethodViewModelBuilder = deliveryMethodViewModelBuilder;
            _countryService = countryService;
            _urlService = urlService;
            _pageService = pageService;
        }

        /// <summary>
        /// Submits the current shopping cart and places the order.
        /// </summary>
        /// <param name="model">Object containing all information of the order including delivery info and payment info.</param>
        [Route]
        [HttpPost]
        [ApiValidateAntiForgeryToken]
        public IHttpActionResult PlaceOrder(CheckoutViewModel model)
        {
            if (!_checkoutService.Validate(new ApiModelState(ModelState), model))
            {
                return BadRequest(ModelState);
            }

            if (!_checkoutService.ValidateOrder(out string message))
            {
                ModelState.AddModelError("general", message);
                return BadRequest(ModelState);
            }

            try
            {
                var checkoutPage = _requestModelAccessor.RequestModel.WebsiteModel.GetValue<PointerPageItem>(AcceleratorWebsiteFieldNameConstants.CheckoutPage);
                var checkoutPageUrl = GetAbsolutePageUrl(checkoutPage);
                var responseUrl = new QueryString()
                        .Add(CheckoutConstants.QueryStringStep, CheckoutConstants.Response)
                        .ReplaceQueryString($"{checkoutPageUrl}.HandleResponse");
                var cancelUrl = new QueryString()
                        .Add(CheckoutConstants.QueryStringStep, CheckoutConstants.Cancel)
                        .ReplaceQueryString($"{checkoutPageUrl}.HandleCancelResponse");
                var executePaymentResult = _checkoutService.PlaceOrder(model, responseUrl, cancelUrl, out string redirectUrl);
                if (executePaymentResult == null)
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    model.RedirectUrl = redirectUrl;
                    return Ok(model);
                }

                redirectUrl = _checkoutService.HandlePaymentResult(executePaymentResult.Success, executePaymentResult.ErrorMessage);
                model.RedirectUrl = redirectUrl;
                return Ok(model);
            }
            catch(CheckoutException e)
            {
                ModelState.AddModelError("general", e.Message);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                this.Log().Error("Error when placing an order", ex);
                ModelState.AddModelError("general", "checkout.generalerror".AsWebSiteString());
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Updates the payment method on the current shopping cart.
        /// </summary>
        /// <param name="model">Object containing the selected payment method.</param>
        [HttpPut]
        [ApiValidateAntiForgeryToken]
        [Route("setPaymentProvider")]
        public IHttpActionResult SetPaymentProvider(CheckoutViewModel model)
        {
            try
            {
                _checkoutService.ChangePaymentMethod(model.SelectedPaymentMethod);
                model.PaymentWidget = _paymentMethodViewModelBuilder.BuildWidget(model.SelectedPaymentMethod);
                return Ok(model);
            }
            catch (Exception ex)
            {
                this.Log().Error("Error when changing payment provider", ex);
                ModelState.AddModelError("general", "checkout.setpaymenterror".AsWebSiteString());
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Updates the delivery method on the current shopping cart.
        /// </summary>
        /// <param name="model">Object containing the selected delivery method.</param>
        [HttpPut]
        [ApiValidateAntiForgeryToken]
        [Route("setDeliveryProvider")]
        public IHttpActionResult SetDeliveryProvider(CheckoutViewModel model)
        {
            try
            {
                _checkoutService.ChangeDeliveryMethod(model.SelectedDeliveryMethod.Value);
                if (model.PaymentWidget != null
                    && model.PaymentWidget.DisplayDeliveryMethods)
                {
                    // when we change the delivery method, we need to reload the payment widget in order to re-calculate the total order
                    model.PaymentWidget = _paymentMethodViewModelBuilder.BuildWidget(model.SelectedPaymentMethod);
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                this.Log().Error("Error when changing delivery provider", ex);
                ModelState.AddModelError("general", "checkout.setdeliveryerror".AsWebSiteString());
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Updates the country to delivery the order.
        /// </summary>
        /// <param name="model">Object containing the selected country.</param>
        [HttpPut]
        [ApiValidateAntiForgeryToken]
        [Route("setCountry")]
        public IHttpActionResult SetCountry(CheckoutViewModel model)
        {
            try
            {
                var country = _countryService.Get(model.SelectedCountry);
                //Check if country is connected to the channel
                if (country != null &&_requestModelAccessor.RequestModel.ChannelModel.Channel.CountryLinks.Any(x =>x.CountrySystemId == country.SystemId))
                {
                    // Set user's country from the address to the channel
                    _requestModelAccessor.RequestModel.Cart.SetChannel(_requestModelAccessor.RequestModel.ChannelModel.Channel, country, SecurityToken.CurrentSecurityToken);

                    model.DeliveryMethods = _deliveryMethodViewModelBuilder.Build();
                    model.SelectedDeliveryMethod = model.DeliveryMethods.FirstOrDefault()?.Id;
                    _checkoutService.ChangeDeliveryMethod(model.SelectedDeliveryMethod.Value);

                    model.PaymentMethods = _paymentMethodViewModelBuilder.Build();
                    model.SelectedPaymentMethod = model.PaymentMethods?.FirstOrDefault()?.Id;
                    _checkoutService.ChangePaymentMethod(model.SelectedPaymentMethod);
                    if (model.PaymentWidget != null)
                    {
                        // when we change the delivery method, we need to reload the payment widget in order to re-calculate the total order
                        model.PaymentWidget = _paymentMethodViewModelBuilder.BuildWidget(model.SelectedPaymentMethod);
                    }
                }

                return Ok(model);
            }
            catch (Exception ex)
            {
                this.Log().Error("Error when changing delivery provider", ex);
                ModelState.AddModelError("general", "checkout.setdeliveryerror".AsWebSiteString());
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Submit a campaign code to apply for the order.
        /// </summary>
        /// <param name="model">Object containing the campaign code.</param>
        [HttpPut]
        [ApiValidateAntiForgeryToken]
        [Route("setCampaignCode")]
        public IHttpActionResult SetCampaignCode(CheckoutViewModel model)
        {
            try
            {
				if (_checkoutService.SetCampaignCode(model.CampaignCode ?? string.Empty))
				{
					if (model.PaymentWidget != null)
					{
						// when we change the campaign code, we need to reload the payment widget in order to re-calculate the total order
						model.PaymentWidget = _paymentMethodViewModelBuilder.BuildWidget(model.SelectedPaymentMethod);
					}
					return Ok(model);
				}

				ModelState.AddModelError(nameof(CheckoutViewModel.CampaignCode), "checkout.campaigncodeinvalid".AsWebSiteString());
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                this.Log().Error("Error when setting campaign code", ex);
                ModelState.AddModelError("general", "checkout.setcampaigncodeerror".AsWebSiteString());
                return BadRequest(ModelState);
            }
        }

        private string GetAbsolutePageUrl(PointerPageItem pointer)
        {
            if (pointer == null)
            {
                return null;
            }
            var page = _pageService.Get(pointer.EntitySystemId);
            if (page == null)
            {
                return null;
            }

            var channelSystemId = pointer.ChannelSystemId != Guid.Empty ? pointer.ChannelSystemId : _requestModelAccessor.RequestModel.ChannelModel.SystemId;
            return _urlService.GetUrl(page, new PageUrlArgs(channelSystemId) { AbsoluteUrl = true });
        }
    }
}
