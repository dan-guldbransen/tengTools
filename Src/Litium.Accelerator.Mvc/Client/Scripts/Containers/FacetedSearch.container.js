import React, { Component } from 'react';
import { connect } from 'react-redux';
import FacetedSearch from '../Components/FacetedSearch';
import { query, searchFacetChange } from '../Actions/FacetedSearch.action';

class FacetedSearchContainer extends Component {
    componentDidMount() {
        // listen to history events (back, forward) of window
        window.onpopstate = window.onpopstate || ((event) => {
            this.props.query(window.location.search.substr(1) || '');
        })
    }

    render() {
        return <FacetedSearch {...this.props} />;
    }
}
const mapStateToProps = ({facetedSearch:{facetFilters, navigationTheme}}) => {
    return {
        facetFilters,
        navigationTheme,
    }
}

const mapDispatchToProps = dispatch => {
    return {
        query: (q, replaceResult) => dispatch(query(q, replaceResult)),
        searchFacetChange: (group, item) => dispatch(searchFacetChange(group, item)),
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(FacetedSearchContainer);