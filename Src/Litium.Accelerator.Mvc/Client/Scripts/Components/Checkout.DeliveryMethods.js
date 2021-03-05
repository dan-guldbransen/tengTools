import React from 'react';

const CheckoutDeliveryMethods = ({ deliveryMethods, selectedId, onChange }) => (
    <section className="row checkout-info__container">
        <div className="columns small-12">
            { deliveryMethods && deliveryMethods.map(method => (
                <label className="row no-margin" key={method.id}>
                    <input type="radio" name="deliveryMethods" className="checkout-info__checkbox-radio"
                        value={method.id} checked={method.id === selectedId} onChange={() => onChange(method.id)} />
                    <span className="columns">
                        <b> {method.name} </b> - {method.formattedPrice}
                    </span>
                </label>
            ))}
        </div>
    </section>
)

export default CheckoutDeliveryMethods;