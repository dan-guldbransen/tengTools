import { 
    CHECKOUT_SUBMIT, 
    CHECKOUT_SUBMIT_ERROR, 
    CHECKOUT_SET_CUSTOMER_INFO,
    CHECKOUT_SET_CAMPAIGN_CODE,
    CHECKOUT_SET_DELIVERY,
    CHECKOUT_SET_PAYMENT,
    CHECKOUT_SET_ORDER_NOTE,
    CHECKOUT_ACCEPT_TERMS_OF_CONDITION,
    CHECKOUT_SET_PRIVATE_CUSTOMER,
    CHECKOUT_SET_PAYMENT_WIDGET,
    CHECKOUT_SET_SELECTED_COMPANY_ADDRESS,
    CHECKOUT_SET_SIGN_UP,
    CHECKOUT_SET_COUNTRY,
} from '../Actions/Checkout.action';
import { error as errorReducer } from './Error.reducer';

const defaultState = { 
    payload: {
        alternativeAddress: {},
        customerDetails: {},
        selectedCompanyAddressId: null,
        selectedDeliveryMethod: {},
        selectedPaymentMethod: {},
        selectedCountry: {},
        campaignCode: '',
        orderNote: {},
        paymentWidget: null,
        isBusinessCustomer: false,
        signUp: false,
        acceptTermsOfCondition: false,

        authenticated: false,
        deliveryMethods: [],
        paymentMethods: [],
        companyAddresses: [],
        responseUrl: '',
        cancelUrl: '',
        redirectUrl: '',
    }, 
    errors: {},
    result: {}, 
    isSubmitting: false,
}

export const checkout = (state = defaultState, action) => {
    const { type, payload } = action;
    switch (type) {
        case CHECKOUT_SUBMIT_ERROR:
            return {
                ...state,
                errors: errorReducer(state.errors, action)
            }
        case CHECKOUT_SUBMIT:
            return {
                ...state,
                ...payload,
            };
        case CHECKOUT_SET_CUSTOMER_INFO:
            return {
                ...state,
                payload: {
                    ...state.payload,
                    [payload.key]: {
                        ...state.payload[payload.key],
                        ...payload.data,
                    }
                },
            }
        case CHECKOUT_SET_DELIVERY:
        case CHECKOUT_SET_PAYMENT:
        case CHECKOUT_SET_ORDER_NOTE:
        case CHECKOUT_SET_PAYMENT_WIDGET:
        case CHECKOUT_SET_PRIVATE_CUSTOMER:
        case CHECKOUT_SET_SIGN_UP:
        case CHECKOUT_SET_SELECTED_COMPANY_ADDRESS:
        case CHECKOUT_ACCEPT_TERMS_OF_CONDITION:
        case CHECKOUT_SET_CAMPAIGN_CODE:
        case CHECKOUT_SET_COUNTRY:
            return {
                ...state,
                payload: {
                    ...state.payload,
                    ...payload,
                }
            }
        default:
            return state;
    }
}