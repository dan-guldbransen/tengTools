import React, { Fragment } from 'react';
import { translate } from '../Services/translation';
import constants from '../constants';

const AddressForm = ({ address = {}, errors = {}, onDismiss, onChange, onSubmit }) => (
    <div>
        <h2>{translate(address.systemId ? 'mypage.address.edittitle' : 'mypage.address.addtitle')}</h2>

        <div className="row">
            <div className="columns small-12 medium-8">
                <label className="form__label" htmlFor="address">{translate('mypage.address.address')}</label>
                <input className="form__input" id="address" name="address" type="text" autoComplete="address-line1" value={address.address || ''} onChange={(event) => onChange('address', event.target.value)}/>
                {errors['address'] &&
                    <span className="form__validator--error form__validator--top-narrow">{errors['address'][0]}</span>
                }

                <input className="form__input" id="address2" name="address2" type="text" autoComplete="address-line2" value={address.address2 || ''} onChange={(event) => onChange('address2', event.target.value)}/>
                {errors['address2'] &&
                    <span className="form__validator--error form__validator--top-narrow">{errors['address2'][0]}</span>
                }
            </div>

            <div className="columns small-12 medium-8">
                <label className="form__label" htmlFor="zipCode">{translate('mypage.address.postnumber')}</label>
                <input className="form__input" id="zipCode" name="zipCode" type="text" autoComplete="postal-code" value={address.zipCode || ''} onChange={(event) => onChange('zipCode', event.target.value)}/>
                {errors['zipCode'] &&
                    <span className="form__validator--error form__validator--top-narrow">{errors['zipCode'][0]}</span>
                }
            </div>

            <div className="columns small-12 medium-8">
                <label className="form__label" htmlFor="city">{translate('mypage.address.city')}</label>
                <input className="form__input" id="city" name="city" type="text" autoComplete="on" value={address.city || ''} onChange={(event) => onChange('city', event.target.value)}/>
                {errors['city'] &&
                    <span className="form__validator--error form__validator--top-narrow">{errors['city'][0]}</span>
                }
            </div>

            <div className="columns small-12 medium-8">
                <label className="form__label" htmlFor="country">{translate('mypage.address.country')}</label>
                <select className="form__input" autoComplete = "country-name" value={address.country || ''} onChange={(event) => onChange('country', event.target.value)}>
                    {constants.countries.map(country => <option key={country.value} value={country.value}>{country.text}</option>)}
                </select>
                {errors['country'] &&
                    <span className="form__validator--error form__validator--top-narrow">{errors['country'][0]}</span>
                }
            </div>

            <div className="columns small-12 medium-8">
                <label className="form__label" htmlFor="phoneNumber">{translate('mypage.address.phonenumber')}</label>
                <input className="form__input" id="phoneNumber" name="phoneNumber" type="tel" autoComplete="tel" value={address.phoneNumber || ''} onChange={(event) => onChange('phoneNumber', event.target.value)}/>
                {errors['phoneNumber'] &&
                    <span className="form__validator--error form__validator--top-narrow">{errors['phoneNumber'][0]}</span>
                }
            </div>
        </div>

        {errors['general'] && <div>{errors['general'][0]}</div>}
        <button className="form__button" onClick={() => onDismiss()}>{translate('general.cancel')}</button>
        <span className="form__space"></span>
        <button className="form__button" onClick={() => onSubmit(address)}>{translate('general.save')}</button>
    </div>
);

export default AddressForm;