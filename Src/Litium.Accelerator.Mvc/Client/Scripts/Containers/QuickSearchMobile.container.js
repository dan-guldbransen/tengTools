import React, { Component } from 'react';
import * as debounce from 'lodash.debounce';
import { connect } from 'react-redux';
import QuickSearchMobile from '../Components/QuickSearchMobile';
import { query, setSearchQuery, toggleShowFullForm, handleKeyDown } from '../Actions/QuickSearch.action';
import { show } from '../Actions/LightboxImages.action';

class QuickSearchContainerMobile extends Component {
    constructor(props) {
        super(props);
        this.state = { ...props };
        this.clickHandler = this.clickHandler.bind(this);
        this.lastClickedNode = null;
    }

    componentDidMount() {
        document.addEventListener('mousedown', this.clickHandler);
        var urlParams = new URLSearchParams(window.location.search);
        if (urlParams.has('q')) {
            const query = urlParams.get('q');
            const { setSearchQuery } = this.props;
            setSearchQuery(query);
        }
    }

    componentWillUnmount() {
        document.removeEventListener('mousedown', this.clickHandler);
    }

    componentWillUpdate({ showFullForm } = props) {
        if (this.state.showFullForm !== showFullForm) {
            this.setState({ ...this.state, showFullForm });
        }
    }

    clickHandler(event) {
        this.lastClickedNode = event.target;
    }

    onBlur() {
        const { showFullForm, toggleShowFullForm } = this.props;
        if (this.searchContainer && !this.searchContainer.contains(this.lastClickedNode)) {
            showFullForm && toggleShowFullForm();
        }
    }

    onClose() {
        const { onSearch } = this.props;
        onSearch("");
    }

    componentDidUpdate(prevProps) {
        if (this.props.selectedItem !== prevProps.selectedItem) {
            document.querySelector('.quick-search-result__item--selected').scrollIntoView({ behavior: 'smooth', block: 'end', inline: 'nearest' });
        }
    }

    render() {
        const { query } = this.props;
        const searchUrl = window.__litium.quickSearchUrl + (query.length > 0 ? `?q=${query}` : '');
        return (
            <div ref={(elem) => this.searchContainer = elem} >
                <QuickSearchMobile {...{ ...this.props, searchUrl, onBlur: () => this.onBlur(), onClose: () => this.onClose() }} />
            </div>
        )
    }
}

const mapStateToProps = state => {
    return {
        query: state.quickSearch.query,
        result: state.quickSearch.result,
        showResult: state.quickSearch.showResult,
        showFullForm: state.quickSearch.showFullForm,
        selectedItem: state.quickSearch.selectedItem,
    }
}

// debouncing function to 200ms so we don't send query request on every key stroke
const debouncedQuery = debounce((dispatch, text) => dispatch(query(text)), 200);

const mapDispatchToProps = dispatch => {
    return {
        onSearch: (text) => {
            dispatch(setSearchQuery(text));
            debouncedQuery(dispatch, text);
        },
        toggleShowFullForm: () => dispatch(toggleShowFullForm()),
        setSearchQuery: (text) => dispatch(setSearchQuery(text)),
        onKeyDown: (event, opt) => dispatch(handleKeyDown(event, opt)),
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(QuickSearchContainerMobile);