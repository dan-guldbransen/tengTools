import React from 'react';
import { translate } from '../Services/translation';

const CheckoutOrderNote = ({ orderNote, onChange }) => (
    <div className="columns small-12 medium-6 checkout-info__summary--full-height">
        {translate('checkout.order.message')}
        <textarea className="form__input checkout-info__messages" value={orderNote} onChange={(event) => onChange(event.target.value)}></textarea>
    </div>
)

export default CheckoutOrderNote;