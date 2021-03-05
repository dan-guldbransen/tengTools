import React, { Component } from 'react';
import { connect } from 'react-redux';
import ReorderButton from '../Components/ReorderButton';
import { receive, loadError } from '../Actions/Cart.action';
import { catchError } from '../Actions/Error.action';
import { reorder as reorderService} from '../Services/Cart.service';
import withReactiveStyleBuyButton from './withReactiveStyleBuyButton';

class ReorderButtonContainer extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <ReorderButton {...this.props}
                onClick={(orderId) => this.props.onClick({ orderId })} />
        );
    }
}

const mapStateToProps = state => {
    return {}
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

export default connect(mapStateToProps, mapDispatchToProps)(
                withReactiveStyleBuyButton(ReorderButtonContainer, 
                    ({ orderId }) => reorderService(orderId), 'buy-button-container'));