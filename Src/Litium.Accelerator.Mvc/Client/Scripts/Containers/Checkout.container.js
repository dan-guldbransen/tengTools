import React, { Component, Fragment } from 'react';
import { connect } from 'react-redux';

import Cart from '../Components/Cart';
import CheckoutPrivateCustomerInfo from '../Components/Checkout.PrivateCustomerInfo';
import CheckoutBusinessCustomerInfo from '../Components/Checkout.BusinessCustomerInfo';
import CheckoutDeliveryMethods from '../Components/Checkout.DeliveryMethods';
import CheckoutPaymentMethods from '../Components/Checkout.PaymentMethods';
import CheckoutOrderNote from '../Components/Checkout.OrderNote';
import CheckoutOrderInfo from '../Components/Checkout.OrderInfo';
import PaymentWidget from '../Components/Payments/PaymentWidget';

import constants from '../constants';

import {
    acceptTermsOfCondition,
    setBusinessCustomer,
    setCampaignCode,
    setCountry,
    setDelivery,
    setPayment,
    setOrderNote,
    submit,
    verify,
    setCustomerDetails,
    setAlternativeAddress,
    setSignUp,
    setSelectedCompanyAddress,
    submitError,
    reloadPayment,
    submitCampaignCode,
    submitRequest,
    submitDone,
} from '../Actions/Checkout.action';
import { update as updateOrderRowQuantity } from '../Actions/Cart.action';

import { translate } from '../Services/translation';
import { string, object, boolean, mixed } from 'yup';

const privateCustomerAdditionalDetailsSchema = object().shape({
    acceptTermsOfCondition: boolean().required(translate(`validation.checkrequired`)).oneOf([true], translate(`validation.checkrequired`)),
    selectedDeliveryMethod: string().required(translate(`validation.required`)),
});

const privateCustomerAddressSchema = object().shape({
    email: string().required(translate(`validation.required`)).email(translate(`validation.email`)),
    phoneNumber: string().required(translate(`validation.required`)),
    country: mixed().required(translate(`validation.required`)).notOneOf([''], translate('validation.required')),
    city: string().required(translate(`validation.required`)),
    zipCode: string().required(translate(`validation.required`)),
    address: string().required(translate(`validation.required`)),
    lastName: string().required(translate(`validation.required`)),
    firstName: string().required(translate(`validation.required`)),
});

const privateCustomerAlternativeAddressSchema = object().shape({
    phoneNumber: string().required(translate(`validation.required`)),
    country: mixed().required(translate(`validation.required`)).notOneOf([''], translate('validation.required')),
    city: string().required(translate(`validation.required`)),
    zipCode: string().required(translate(`validation.required`)),
    address: string().required(translate(`validation.required`)),
    lastName: string().required(translate(`validation.required`)),
    firstName: string().required(translate(`validation.required`)),
});

const businessCustomerDetailsSchema = object().shape({
    acceptTermsOfCondition: boolean().required(translate(`validation.checkrequired`)).oneOf([true], translate(`validation.checkrequired`)),
    selectedDeliveryMethod: string().required(translate(`validation.required`)),
    email: string().required(translate(`validation.required`)).email(translate(`validation.email`)),
    phoneNumber: string().required(translate(`validation.required`)),
    lastName: string().required(translate(`validation.required`)),
    firstName: string().required(translate(`validation.required`)),
    selectedCompanyAddressId: string().required(translate(`validation.required`)),
});

class CheckoutContainer extends Component {
    componentDidMount() {
        if (!this.props || !this.props.checkout) {
            return;
        }
        // set selected value for payment method on load.
        const { selectedPaymentMethod } = this.props.checkout.payload;
        selectedPaymentMethod && this.props.setPayment(selectedPaymentMethod);

        // fill default select value to the state
        const { customerDetails, alternativeAddress } = this.props.checkout.payload;
        (!customerDetails || !customerDetails.country) && constants.countries && constants.countries[0] && this.props.onCustomerDetailsChange('customerDetails', 'country', constants.countries[0].value);
        (!alternativeAddress || !alternativeAddress.country) && constants.countries && constants.countries[0] && this.props.onCustomerDetailsChange('alternativeAddress', 'country', constants.countries[0].value);
    }

    placeOrder() {
        const { payload } = this.props.checkout,
            { isBusinessCustomer, selectedCompanyAddressId, acceptTermsOfCondition,
                selectedPaymentMethod, selectedDeliveryMethod } = this.props.checkout.payload;
        const notCustomerDetailFields = ['selectedCompanyAddressId', 'selectedPaymentMethod', 'selectedDeliveryMethod', 'acceptTermsOfCondition'];
        const onError = (error, addressPath = 'customerDetails') => {
            error.path = notCustomerDetailFields.indexOf(error.path) >= 0 ? error.path : `${addressPath}-${error.path}`;
            this.props.submitError(error)
        };
        this.props.submitRequest();
        if (isBusinessCustomer) {
            businessCustomerDetailsSchema.validate({
                ...payload.customerDetails,
                selectedCompanyAddressId,
                selectedPaymentMethod,
                selectedDeliveryMethod,
                acceptTermsOfCondition
            })
                .then(() => {
                    this.props.submit();
                })
                .catch(onError);
        } else {
            const checkAltAddress = payload.alternativeAddress.showAlternativeAddress && (payload.alternativeAddress.firstName ||
                payload.alternativeAddress.lastName ||
                payload.alternativeAddress.address ||
                payload.alternativeAddress.zipCode ||
                payload.alternativeAddress.city ||
                payload.alternativeAddress.phoneNumber);

            privateCustomerAddressSchema.validate({
                ...payload.customerDetails
            })
                .then(() => {
                    payload.showAlternativeAddress = payload.alternativeAddress.showAlternativeAddress;
                    if (checkAltAddress) {
                        privateCustomerAlternativeAddressSchema.validate({
                            ...payload.alternativeAddress
                        })
                            .then(() => {
                                privateCustomerAdditionalDetailsSchema.validate({
                                    selectedPaymentMethod,
                                    selectedDeliveryMethod,
                                    acceptTermsOfCondition
                                })
                                    .then(() => {
                                        this.props.submit();
                                    })
                                    .catch(onError);
                            })
                            .catch((error) => { onError(error, 'alternativeAddress') })
                    } else {
                        privateCustomerAdditionalDetailsSchema.validate({
                            selectedPaymentMethod,
                            selectedDeliveryMethod,
                            acceptTermsOfCondition
                        })
                            .then(() => {
                                this.props.submit();
                            })
                            .catch(onError);
                    }
                })
                .catch(onError);
        }
    }

    render() {
        const { cart } = this.props;
        if (!cart || !cart.orderRows || cart.orderRows.length < 1) {
            return (
                <div className="row">
                    <div className="small-12">
                        <h2 className="checkout__title">{translate(`checkout.cart.empty`)}</h2>
                    </div>
                </div>
            );
        }

        const { checkout } = this.props,
            { payload, errors = {} } = checkout,
            { orderNote, paymentWidget, paymentMethods, authenticated, isBusinessCustomer, checkoutMode} = payload;
        return (
            <Fragment>
                {(!paymentWidget || paymentWidget.isChangeable) && this.renderCartSection()}
                {(!paymentWidget || paymentWidget.displayCustomerDetails) && this.renderCustomerDetailsSection()}
                {(!paymentWidget || paymentWidget.displayDeliveryMethods) && this.renderDeliveryMethodsSection()}
                {(paymentMethods && paymentMethods.length > 1 && (!paymentWidget || paymentWidget.isChangeable)) && this.renderPaymentMethodsSection()}
                <PaymentWidget {...paymentWidget} verify={this.props.verify} />

                {!paymentWidget && (
                    <Fragment>
                        <div className="row">
                            <h3 className="checkout__section-title">{translate('checkout.order.title')}</h3>
                        </div>

                        <section className="row checkout-info__container checkout-info__summary">
                            <CheckoutOrderNote {...orderNote} onChange={(note) => this.props.setOrderNote(note)} />
                            <CheckoutOrderInfo cart={cart} />
                        </section>

                        <div className="row">
                            <input className="checkout-info__checkbox-input" type="checkbox" id="acceptTermsOfCondition" checked={payload.acceptTermsOfCondition} onChange={(event) => this.props.acceptTermsOfCondition(event.target.checked)} />
                            <label className="checkout-info__checkbox-label" htmlFor="acceptTermsOfCondition">
                                {translate('checkout.terms.acceptTermsOfCondition')} <a className="checkout__link" href={payload.termsUrl} target="_blank">{translate('checkout.terms.link')}</a>
                            </label>
                            {errors['acceptTermsOfCondition'] &&
                                <span className="form__validator--error form__validator--top-narrow" data-error-for="acceptTermsOfCondition">{errors['acceptTermsOfCondition'][0]}</span>
                            }
                        </div>

                        <div className="row">
                            {!authenticated && (isBusinessCustomer || checkoutMode === constants.checkoutMode.companyCustomers) ?
                                (<button className="checkout__submit-button" onClick={() => location.href = payload.loginUrl} >{translate('checkout.login.to.placeorder')}</button>)
                                : (<button type="submit" className="checkout__submit-button" disabled={checkout.isSubmitting} onClick={() => this.placeOrder()} >{translate('checkout.placeorder')}</button>)
                            }
                        </div>
                    </Fragment>
                )}

                <div className="row">
                    {errors && errors['general'] && <p className="checkout__validator--error">{errors['general'][0]}</p>}
                </div>
            </Fragment>
        );
    }

    componentDidUpdate(prevProps) {
        if (this.props.checkout.result && this.props.checkout.result.redirectUrl) {
            window.location = this.props.checkout.result.redirectUrl;
            return;
        }

        const { checkout } = this.props;
        if (!checkout.isSubmitting || !checkout.errors) {
            return;
        }

        const errorKeys = Object.keys(checkout.errors);
        if (!errorKeys || errorKeys.length < 1) {
            return;
        }

        const errorNode = document.querySelector(`[data-error-for="${errorKeys[0]}"]`);
        if (!errorNode) {
            return;
        }

        const inputNode = errorNode.parentElement.querySelector('input');
        if (inputNode) {
            setTimeout(() => inputNode.focus(), 1000);
            inputNode.scrollIntoView({ behavior: 'smooth' });
        } else {
            errorNode.scrollIntoView({ behavior: 'smooth' });
        }
    }

    renderCartSection() {
        const { checkout } = this.props,
            { errors = {} } = checkout;

        return (
            <Fragment>
                <div className="row">
                    <div className="small-12">
                        <h2 className="checkout__title">{translate('checkout.title')}</h2>
                    </div>
                </div>
                <div className="row">
                    <h3 className="checkout__section-title">{translate('checkout.cart.title')}</h3>
                </div>
                <div className="row">
                    {errors && errors['cart'] && <p className="checkout__validator--error">{errors['cart'][0]}</p>}
                </div>
                <Cart {...{
                    ...this.props.cart,
                    updateOrderRowQuantity: this.props.updateOrderRowQuantity,
                    removeOrderRow: this.props.removeOrderRow,
                }} />
            </Fragment>
        );
    }

    renderCustomerDetailsSection() {
        const { checkout, onCustomerDetailsChange, onSignUpChange, setSelectedCompanyAddress, setCountry } = this.props,
            { payload, errors = {} } = checkout,
            { companyName, authenticated, customerDetails, alternativeAddress, companyAddresses, selectedCompanyAddressId,
                isBusinessCustomer, signUp, checkoutMode } = payload;
        const privateCustomerInfoComponent = <CheckoutPrivateCustomerInfo {...{ customerDetails, alternativeAddress, authenticated, onChange: onCustomerDetailsChange, signUp, onSignUpChange, setCountry, errors }} />;
        const businessCustomerInfoComponent = <CheckoutBusinessCustomerInfo {...{ customerDetails, companyAddresses, companyName, authenticated, selectedCompanyAddressId, onChange: onCustomerDetailsChange, setSelectedCompanyAddress, errors }} />;
        if (!authenticated) {
            return (
                <Fragment>
                    <div className="row">
                        <h3 className="checkout__section-title">{translate('checkout.customerinfo.title')}</h3>
                        <Fragment>
                            <label className="checkout__text--in-line">{translate('checkout.customerinfo.existingcustomer')}</label>&nbsp;
                            <a href={payload.loginUrl} className="checkout__link">{translate('checkout.customerinfo.clicktologin')}</a>&nbsp;
                            {!isBusinessCustomer && checkoutMode === constants.checkoutMode.both && <a onClick={() => this.props.setBusinessCustomer(true)} className="checkout__link">{translate('checkout.customerinfo.businesscustomer')}</a>}
                        </Fragment>
                        {isBusinessCustomer && checkoutMode === constants.checkoutMode.both && <a onClick={() => this.props.setBusinessCustomer(false)} className="checkout__link">{translate('checkout.customerinfo.privatecustomer')}</a>}
                    </div>
                    {!isBusinessCustomer && checkoutMode !== constants.checkoutMode.companyCustomers && privateCustomerInfoComponent}
                    {(isBusinessCustomer || checkoutMode === constants.checkoutMode.companyCustomers) && businessCustomerInfoComponent}
                </Fragment>
            );
        }
        if (isBusinessCustomer) {
            return (
                <Fragment>
                    <div className="row">
                        <h3 className="checkout__section-title">{translate('checkout.customerinfo.title')}</h3>
                    </div>
                    {authenticated && businessCustomerInfoComponent}
                </Fragment>
            );
        }

        return (
            <Fragment>
                <div className="row">
                    <h3 className="checkout__section-title">{translate('checkout.customerinfo.title')}</h3>
                </div>
                {privateCustomerInfoComponent}
            </Fragment>
        );
    }

    renderDeliveryMethodsSection() {
        const { checkout } = this.props,
            { payload, errors = {} } = checkout,
            { deliveryMethods } = payload;

        return (
            <Fragment>
                <div className="row">
                    <h3 className="checkout__section-title">{translate('checkout.delivery.title')}</h3>
                </div>
                <CheckoutDeliveryMethods deliveryMethods={deliveryMethods} selectedId={payload.selectedDeliveryMethod} onChange={this.props.setDelivery} />
                {errors['selectedDeliveryMethod'] &&
                    <span className="form__validator--error form__validator--top-narrow">{errors['selectedDeliveryMethod'][0]}</span>
                }
            </Fragment>
        );
    }

    renderPaymentMethodsSection() {
        const { checkout } = this.props,
            { payload, errors = {} } = checkout,
            { paymentMethods } = payload;
        return (
            <Fragment>
                <div className="row">
                    <h3 className="checkout__section-title">{translate('checkout.payment.title')}</h3>
                </div>
                <CheckoutPaymentMethods paymentMethods={paymentMethods} 
                    selectedId={payload.selectedPaymentMethod} 
                    onChange={this.props.setPayment} 
                    errors={errors}
                    onCampaignCodeChange={this.props.setCampaignCode}
                    onSubmitCampaignCode={this.props.submitCampaignCode}/>
                {errors['selectedPaymentMethod'] &&
                    <span className="form__validator--error form__validator--top-narrow">{errors['selectedPaymentMethod'][0]}</span>
                }
            </Fragment>
        );
    }
}

const mapStateToProps = state => {
    const { cart, checkout } = state;
    return {
        cart,
        checkout,
    }
}

const mapDispatchToProps = dispatch => {
    return {
        submit: () => dispatch(submit()),
        setBusinessCustomer: (value) => dispatch(setBusinessCustomer(value)),
        setCampaignCode: (code) => dispatch(setCampaignCode(code)),
        setCountry: (systemId) => dispatch(setCountry(systemId)),
        setDelivery: (systemId) => dispatch(setDelivery(systemId)),
        setPayment: (systemId) => dispatch(setPayment(systemId)),
        setOrderNote: (note) => dispatch(setOrderNote(note)),
        acceptTermsOfCondition: (accept) => dispatch(acceptTermsOfCondition(accept)),
        verify: (url, orderId, payload) => dispatch(verify(url, orderId, payload)),
        onSignUpChange: (signUp) => dispatch(setSignUp(signUp)),
        submitCampaignCode: () => dispatch(submitCampaignCode()),
        submitRequest: () => dispatch(submitRequest()),
        submitError: (error) => {
            dispatch(submitError(error));
            dispatch(submitDone(null));
        },
        setSelectedCompanyAddress: (companyAddressId, country) => {
            dispatch(setSelectedCompanyAddress(companyAddressId));
            dispatch(setCountry(country));
        },
        onCustomerDetailsChange: (stateKey, propName, value) => {
            switch (stateKey) {
                case 'customerDetails':
                    dispatch(setCustomerDetails(propName, value));
                    break;
                case 'alternativeAddress':
                    dispatch(setAlternativeAddress(propName, value));
                    break;
            }
        },

        updateOrderRowQuantity: (rowId, quantity) => dispatch(updateOrderRowQuantity(rowId, quantity)).then(() => dispatch(reloadPayment())),
        removeOrderRow: (rowId) => dispatch(updateOrderRowQuantity(rowId, 0)).then(() => dispatch(reloadPayment())),
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(CheckoutContainer);