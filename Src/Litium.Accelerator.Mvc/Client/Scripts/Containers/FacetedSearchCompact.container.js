import React, { Component, Fragment } from 'react';
import { connect } from 'react-redux';
import FacetedSearchCompact from '../Components/FacetedSearchCompact';
import {AccordionPanel, Accordion} from './Accordion.container';
import { query, submitSearchFacet, toggleVisibleDropdownMenu, searchFacetChange } from '../Actions/FacetedSearch.action';
import SubNav from '../Components/SubNavCompact';
import SortCriteriaCompact from '../Components/SortCriteriaCompact';
import FilterTag from '../Components/FilterTags';
import { translate } from '../Services/translation';
import { PRODUCT_VIEW_CACHED, updateFilterOption }  from '../Actions/FacetedSearch.action';

class FacetedSearchCompactContainer extends Component {
    constructor(props) {
        super(props);
        const {facetFilters} = props;
        this.state = {
            facetFilters,
        };
    }

    componentDidMount() {
        this.props.query(window.location.search.substr(1) || '', false);
    }

    onFacetChange (filter, option) {
        this.setState((prevState, props) => {
            const { facetFilters } = this.state;
            const newFilters = updateFilterOption(facetFilters, filter, option);
            return {
                ...prevState,
                facetFilters: newFilters,
            };
        });
    }

    static getDerivedStateFromProps(nextProp, prevState) {
        if (nextProp.productsViewCachedId !== prevState.productsViewCachedId) {
            return {
                ...prevState,
                ...nextProp,
            }
        }
        return prevState;
    }

    onSearchResultDataChange(dom) {
        if ( [null, undefined].includes(dom) ) {
            return;
        }
        const container = document.createElement('div');
        container.innerHTML = dom;
        const existingResult = document.querySelector("#search-result");
        const tempResult = container.querySelector("#search-result");
        const existingFilter = existingResult.querySelector('#facetedSearchCompact');
        const tempFilter = tempResult.querySelector('#facetedSearchCompact');
        const replace = (node, newNode) => node.parentNode.replaceChild(newNode, node);
        // move existingFilter from existingResult to tempResult
        replace(tempFilter, existingFilter);
        // replace existingResult with tempResult ( newResult )
        replace(existingResult, tempResult);
        // bootstrap react components if any exists in the search result
        window.__litium.bootstrapComponents();
    }

    componentDidUpdate() {
        const productViewCached = window.__litium.cache ? window.__litium.cache[PRODUCT_VIEW_CACHED] || {} : {};
        if (!productViewCached.used) {
            productViewCached.used = true;
            const dom = productViewCached.productsView;
            dom && this.onSearchResultDataChange(dom);
        }
    }

    render() {
        const {
            subNavigation,
            sortCriteria,
            navigationTheme='',
            ...facetProps
        } = this.props;
        const { facetFilters } = this.state;
        const facetState = {
            ...facetProps,
            facetFilters,
            onFacetChange: (filter, option) => this.onFacetChange(filter, option),
        };
        const empty = array => !(array && array.length);
        const subNavigations = !subNavigation ? null : [subNavigation];
        const sortCriterias = !sortCriteria || !sortCriteria.sortItems ? null : sortCriteria.sortItems;
        const hidden = [subNavigations, facetFilters, sortCriterias].every(arr => empty(arr));
        return ( hidden ? null :
            <Fragment>
                <FilterTag {...{...facetProps, navigationTheme}} />
                <Accordion className='compact-filter hide-for-large'>
                    {!empty(subNavigations) &&
                    <AccordionPanel header={translate('facet.header.categories')}>
                        <SubNav {...{subNavigation: subNavigations}}/>
                    </AccordionPanel>
                    }
                    {!empty(facetFilters) &&
                    <AccordionPanel header={translate('facet.header.filter')}>
                        <FacetedSearchCompact {...facetState} />
                    </AccordionPanel>
                    }
                    {!empty(sortCriterias) &&
                    <AccordionPanel header={translate('facet.header.sortCriteria')}>
                        <SortCriteriaCompact {...{sortCriteria: sortCriterias}}/>
                    </AccordionPanel>
                    }
                </Accordion>
                {navigationTheme==='category' && (
                    <div className='compact-filter category-theme show-for-large'>
                        <FacetedSearchCompact {...facetState}/>
                    </div>
                )}
            </Fragment>
        );
    }
}

const mapStateToProps = ({facetedSearch:{subNavigation, sortCriteria, facetFilters, visibleDropdownMenu, navigationTheme, productsViewCachedId}}) => {
    return {
        subNavigation,
        sortCriteria,
        facetFilters,
        visibleDropdownMenu,
        navigationTheme,
        productsViewCachedId,
    }
}

const mapDispatchToProps = dispatch => {
    return {
        query: (q, withHtmlResult) => dispatch(query(q, withHtmlResult)),
        onSubmit: (allFilters) => dispatch(submitSearchFacet(allFilters)),
        toggleVisibleDropdownMenu: (group) => dispatch(toggleVisibleDropdownMenu(group)),
        searchFacetChange: (group, item) => dispatch(searchFacetChange(group, item)),
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(FacetedSearchCompactContainer);