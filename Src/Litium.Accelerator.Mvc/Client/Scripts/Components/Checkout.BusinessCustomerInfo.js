import React from 'react';
import { translate } from '../Services/translation';
import constants from '../constants';

const CheckoutBusinessCustomerInfo = (props) => {
    const { companyAddresses = [], companyName, authenticated, selectedCompanyAddressId, onChange, setSelectedCompanyAddress, errors = {} } = props;
    const input = (cssClass, stateKey, id, autoComplete = null, placeholder = null, type = 'text') => (
        <div className={cssClass}>
            <label className="form__label" htmlFor={`${stateKey}-${id}`}>{translate(`checkout.customerinfo.${id.toLowerCase()}`)}&#8203;</label>
            <input className="form__input" disabled={!authenticated} id={`${stateKey}-${id}`} name={`${stateKey}-${id}`} type={type}
                value={(props[stateKey] || {})[id] || ''} placeholder={placeholder} autoComplete={autoComplete}
                onChange={(event) => onChange(stateKey, id, event.target.value)}/>
            {errors[`${stateKey}-${id}`] &&
                <span className="form__validator--error form__validator--top-narrow" data-error-for={`${stateKey}-${id}`}>{errors[`${stateKey}-${id}`][0]}</span>
            }
        </div>
    );
    const selectedAddress = selectedCompanyAddressId && companyAddresses ? companyAddresses.find(address => address.systemId === selectedCompanyAddressId) : null;
    const getCountry = (address) => {
        const addressCountry = constants.countries ? constants.countries.find(country => country.value === address.country) : null;
        return addressCountry ? addressCountry.text : address.country;
    };
    return (
        <div className="row checkout-info__container">
            <div className="small-12 medium-6 columns">
                <div className="row-inner">
                    <div className="small-12 columns">{translate('checkout.customerinfo.reference')}</div>
                </div>
                <div className="row-inner">
                    {input('small-6 columns', 'customerDetails', 'firstName', 'billing given-name')}
                    {input('small-6 columns', 'customerDetails', 'lastName', 'billing family-name')}
                </div>
                <div className="row-inner">
                    {input('small-12 columns', 'customerDetails', 'phoneNumber', 'billing tel')}
                </div>
                <div className="row-inner">
                    {input('small-6 columns', 'customerDetails', 'email', 'email')}
                </div>
            </div>
            <div className="small-12 medium-6 columns">
                <div className="row-inner">
                    <div className="small-12 columns">{translate('checkout.customerinfo.address')}</div>
                    <div className="small-12 columns">
                        <label className="form__label" htmlFor="address">&#8203;</label>
                        <select className="form__input" value={selectedCompanyAddressId || ''} disabled={!authenticated}
                            onChange={(event) => setSelectedCompanyAddress(event.target.value, companyAddresses.find(address => address.systemId === event.target.value).country)}>
                            <option value='' disabled>{translate('checkout.customerinfo.companyaddress.placeholder')}</option>
                            {companyAddresses && companyAddresses.map((address) =>
                                <option value={address.systemId} key={`companyAddress-${address.systemId}`}>{`${address.address}, ${address.zipCode}, ${address.city}, ${getCountry(address)}`}</option>
                            )}
                        </select>
                        {errors['selectedCompanyAddressId'] &&
                            <span className="form__validator--error form__validator--top-narrow" data-error-for='selectedCompanyAddressId'>{errors['selectedCompanyAddressId'][0]}</span>
                        }
                    </div>
                </div>
                { selectedAddress &&
                    <div className="row-inner">
                        <div className="small-12 columns">{companyName}</div>
                        <div className="small-12 columns">{selectedAddress.address}</div>
                        <div className="small-12 columns">
                            <span>{selectedAddress.zipCode}</span>&nbsp;
                            <span>{selectedAddress.city}</span>
                        </div>
                        <div className="small-12 columns">{getCountry(selectedAddress)}</div>
                    </div>
                }
            </div>
        </div>
    )
}

export default CheckoutBusinessCustomerInfo;