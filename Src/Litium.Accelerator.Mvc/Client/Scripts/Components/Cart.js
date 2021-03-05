import React, { Component, Fragment } from 'react';
import { translate } from '../Services/translation';

class Cart extends Component {
    constructor(props) {
        super(props);
        this.state = {
            editingQuantity: {},
            removingRow: {},
        };
    }

    onQuantityChange(rowSystemId, quantity) {
        this.setState(previousState => ({
            ...previousState,
            editingQuantity: {
                ...previousState.editingQuantity,
                [rowSystemId]: Math.abs(quantity), // The quantity should be a positive number
            }
        }));
    }

    onDeleteRequest(rowSystemId, showDeleteButton) {
        this.setState(previousState => ({
            ...previousState,
            removingRow: {
                ...previousState.removingRow,
                [rowSystemId]: showDeleteButton,
            }
        }));
    }

    handleUpdateClick(rowSystemId, currentQuantity, originalQuantity) {
        if (currentQuantity < 1 || currentQuantity === originalQuantity) {
            return;
        }
        this.props.updateOrderRowQuantity(rowSystemId, currentQuantity)
            .then(() => this.onQuantityChange(rowSystemId, undefined));
    }

    render() {
        const { editingQuantity, removingRow } = this.state;
        const productHeaderClass  = "columns small-12 medium-4 large-6"; /* = productContentClass */
        const priceHeaderClass    = "columns small-3 medium-2 large-2"; /* = priceContentClass */
        const quantityHeaderClass = "columns small-4 medium-3 large-2"; /* = quantityInputClass + quantityActionClass  */
        const totalHeaderClass    = "columns small-5 medium-3 large-2"; /* = totalContentClass + totalActionClass  */
        const productContentClass = "columns small-12 medium-4 large-6" + " checkout-cart__image-container";
        const priceContentClass   = "columns small-3 medium-2 large-2" + " simple-table__cell--no-break-word";
        const quantityInputClass  = "columns small-2 medium-2 large-1";
        const quantityActionClass = "columns small-2 medium-1 large-1";
        const totalContentClass   = "columns small-2 medium-1 large-1" + " simple-table__cell--no-break-word";
        const totalActionClass    =  "columns small-3 medium-2 large-1";
        return <div className="row checkout__container">
                <div className="small-12 simple-table">
                    <div className="row small-unstack no-margin">
                        <div className={productHeaderClass}></div>
                        <div className={priceHeaderClass}>{translate('checkout.cart.header.price')}</div>
                        <div className={quantityHeaderClass}>{translate('checkout.cart.header.quantity')}</div>
                        <div className={totalHeaderClass}>{translate('checkout.cart.header.total')}</div>
                    </div>
                    { this.props.orderRows && this.props.orderRows.map(order => (
                        <div className="row small-unstack no-margin checkout-cart__row" key={order.rowSystemId}>
                            <div className={productContentClass}>
                                <div className="checkout-cart__image-wrapper">
                                    <img className="checkout-cart__image" src={order.image} alt={order.name} />
                                </div>
                                <div className="checkout-cart__image-info">
                                    <a href={order.url}>{order.name}</a>
                                    <span className="checkout-cart__brand-name" >{order.brand}</span>
                                </div>
                            </div>
                            <div className={priceContentClass}>
                                {order.campaignPrice ? (
                                    <Fragment>
                                        <div className='checkout-cart__campaign-price'>{order.campaignPrice}</div>
                                        <div className='checkout-cart__original-price'> ({order.price})</div>
                                    </Fragment>
                                ) : order.price}
                            </div>
                            <div className={quantityInputClass}>
                                { order.isFreeGift ? (<div>{order.quantity}</div>) : (
                                        <input className="checkout-cart__input" type="number" min="1" maxLength={3}
                                        value={editingQuantity[order.rowSystemId] || order.quantity}
                                        onChange={event => this.handleUpdateClick(order.rowSystemId, event.target.value, editingQuantity[order.rowSystemId] || order.quantity)} />)
                                 }
                             </div>
                            
                            <div className={quantityActionClass}>
                            </div>
                            <div className={`checkout-cart__total-price ${totalContentClass}`}>
                                {order.totalPrice}
                            </div>
                            <div className={totalActionClass}>
                                {!order.isFreeGift && !removingRow[order.rowSystemId] && <a className="table__icon table__icon--delete" onClick={() => this.onDeleteRequest(order.rowSystemId, true)} title={translate('general.remove')}></a>}
                                {!order.isFreeGift && removingRow[order.rowSystemId] && (
                                    <Fragment>
                                        <a className="table__icon table__icon--accept" onClick={() => this.props.removeOrderRow(order.rowSystemId)} title={translate('general.ok')}></a>
                                        <a className="table__icon table__icon--cancel" onClick={() => this.onDeleteRequest(order.rowSystemId, false)} title={translate('general.cancel')}></a>
                                    </Fragment>
                                )}
                            </div>
                        </div>
                    ))}
                    <div className="row small-unstack no-margin checkout-cart__row">
                        <div className="columns">
                            <h3 className="text--right">{translate('checkout.cart.total')}: {this.props.orderTotal}</h3>
                        </div>
                    </div>
                </div>
            </div>;
    }
}

export default Cart;