import React, { Component } from 'react';

export const AccordionPanel = (props) => props;

export class Accordion extends Component {
    constructor(props) {
        super(props);
        const {index = -1} = props;
        this.state = {
            index,
        };
    }
    handleClick(e, index) {
        this.setState((prevState, props) => ({
            ...prevState,
            index: index===prevState.index ? -1 : index,
        }));
    }
    render() {
        const accordions = React.Children.toArray(this.props.children);
        const activeClass = index => this.state.index===index ? 'active' : '';
        const headers = accordions.map((accordion, index) =>
            <div className="columns" key={`accordion__header-${index}`}>
                <div className={`accordion__header ${activeClass(index)} ${accordion.props.icon||''}`} onClick = {e => this.handleClick(e, index)}>
                    {accordion.props.header||''}
                </div>
            </div>
        );
        const panels = accordions.map((accordion, index) =>
            <div className={`accordion__panel ${activeClass(index)}`} key={`accordion__panel-${index}`} >
                {accordion.props.children}
            </div>
        );
        return (
            <div className={this.props.className}>
                <nav className={`accordion__header-container`}>
                    {headers}
                </nav>
                {panels}
            </div>
        );
    }
}