import React, { Component } from 'react';
import { connect } from 'react-redux';
import BuyButton from '../Components/BuyButton';
import { receive, loadError } from '../Actions/Cart.action';
import { catchError } from '../Actions/Error.action';
import { add as addToCartService} from '../Services/Cart.service';
import withReactiveStyleBuyButton from './withReactiveStyleBuyButton';

class BuyButtonContainer extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <BuyButton {...this.props}
                onClick={(articleNumber, quantityFieldId) => this.props.onClick({ articleNumber, quantityFieldId })} />
        );
    }
}

const mapStateToProps = state => {
    return { }
}

const mapDispatchToProps = dispatch => {
    return {
        onSuccess: (cart) => {
            dispatch(receive(cart));
        },
        onError: (ex) => {
            dispatch(catchError(ex, error => loadError(error)));
        }
    }
}

const onClick = ({ articleNumber, quantityFieldId }) => {
    const quantity = quantityFieldId ? document.getElementById(quantityFieldId).value : 1;
    return addToCartService({ articleNumber, quantity });
}
export default connect(mapStateToProps, mapDispatchToProps)(
            withReactiveStyleBuyButton(BuyButtonContainer, onClick, 'buy-button-container'));