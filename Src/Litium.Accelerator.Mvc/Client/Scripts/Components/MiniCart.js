import React from 'react';
import { translate } from '../Services/translation';

const MiniCart = ({ quantity, orderTotal, checkoutUrl, showInfo, toggle }) => (
    <div className="cart cart--mini">
        <a href="#" className="cart__link--block" onClick={() => toggle()}>
            <i className="cart__icon">
                <span className="cart__quantity">{ quantity }</span>
            </i>
            <span className="cart__title">{ translate('minicart.checkout') }</span>
        </a>
        <div className={ `cart__info ${!showInfo ? 'cart__info--hidden' : ''}` }>
            <span className="cart__close-button" onClick={() => toggle()}>
            </span>
            <p className="cart__info-row">{ quantity } { translate('minicart.numberofproduct') }</p>
            <p className="cart__info-row"><b>{ translate('minicart.total') }</b> { orderTotal }</p>
            <a href={ checkoutUrl } className="cart__checkout-button">{ translate('minicart.checkout') }</a>
        </div>
    </div>
)

export default MiniCart;