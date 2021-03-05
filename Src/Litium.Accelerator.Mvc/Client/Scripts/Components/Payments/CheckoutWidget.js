import React, { Component } from 'react';

class CheckoutWidget extends Component {
    constructor(props) {
        super(props);
        this.state = props.extractScripts(props.paymentSession);
    }

    componentDidMount() {
        this.state.scripts && this.state.scripts.forEach(script => this.props.executeScript('checkout-widget', script));
        this.state.scriptFiles && this.state.scriptFiles.forEach(url => this.props.includeScript('checkout-widget', url));
    }

    render() {
        return <div id="checkout-widget" dangerouslySetInnerHTML={{ __html: this.state.html }} />;
    }
}

export default CheckoutWidget;