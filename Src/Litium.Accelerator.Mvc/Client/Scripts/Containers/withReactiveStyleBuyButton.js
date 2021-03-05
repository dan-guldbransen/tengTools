import React, { Component } from 'react';

const StateStyles = {
    LOADING: "--loading",
    SUCCESS: "--success",
    ERROR: "--error",
};

/**
 * Represents a HOC which wraps a BuyButton or ReorderButton and applies diffrent styles to it
 * depending on its click state.
 * @param {*} WrappedComponent The button component.
 * @param {*} onClick The button's onClick event.
 * @param {*} stylePrefix The style prefix to append state's modifiers. For
 * example, 'button' will result as 'button--loading'.
 * 
 * Some available props that the HOC component supports:
 * autoReset : it is true by default. Not its value, but the behaviour is like that. Unless people set it as false, by default, button is always reset to neutral state after the request is completed.
 * resetTimeout: Number milisecond after the complete state, the style of button will be reset. If don't set, it is 2000
 * minimumLoadingTime: Mininum milisecond to display the loading state. If don't set, it is 1000
 * onSuccess: callback if onClick method returns data
 * onError: callback if onClick method throws an exception
 */
export default function withReactiveStyleBuyButton(WrappedComponent, onClick, stylePrefix) {
    return class extends Component {
        constructor(props) {
            super(props);
            this.state = {
                stateClass: '',
                startTime: 0,
            }
        }

        onNeutralState() {
            this.setState({
                stateClass: '',
                startTime: 0,
            });
        }
    
        onLoadingState() {
            this.setState({
                stateClass: `${stylePrefix}${StateStyles.LOADING}`,
                startTime: Date.now(),
            });
        }
    
        onCompleteState(success) {
            const changeState = () => {
                this.setState({
                    stateClass: `${stylePrefix}${success ? StateStyles.SUCCESS : StateStyles.ERROR}`,
                });
                // if `autoReset` is true, which is default, the style will be changed
                // to neutral after a `resetTimeout` amount of time (2 seconds by default).
                this.props.autoReset !== false && setTimeout(() => {
                    this.onNeutralState();
                }, this.props.resetTimeout || 2000);
            }

            const loadingDuration = Date.now() - this.state.startTime;
            const minimumLoadingTime = this.props.minimumLoadingTime || 1000;
            // ensure the loading indicator is displayed at least a `minimumLoadingTime`
            // amount of time before changing it to Success or Error.
            if (loadingDuration >= minimumLoadingTime) {
                changeState();
            } else {
                setTimeout(() => {
                    changeState();
                }, minimumLoadingTime - loadingDuration);
            }
        }

        async onButtonClick(params) {
            try {
                this.onLoadingState();
                const data = await onClick(params);
                if(data) {
                    this.onCompleteState(true);
                    this.props.onSuccess(data);
                } else {
                    this.onNeutralState();
                }
            } catch(err) {
                this.onCompleteState(false);
                this.props.onError(err);
            }
        }
    
        render() {
            return (
                <span className={this.state.stateClass}>
                    <WrappedComponent onClick={(params) => this.onButtonClick(params)} {...this.props} />
                </span>
            );
        }
    }
}