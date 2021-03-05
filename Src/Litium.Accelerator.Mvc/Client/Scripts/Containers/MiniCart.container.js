import React from 'react';
import { connect } from 'react-redux';
import MiniCart from '../Components/MiniCart';
import { toggle } from '../Actions/Cart.action';

const MiniCartContainer = props => (
    <MiniCart {...props} />
)

const mapStateToProps = state => {
    const { cart } = state;
    return {
        ...cart,
    }
}

const mapDispatchToProps = dispatch => {
    return {
        toggle: () => dispatch(toggle()),
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(MiniCartContainer);