import React, { Fragment, Component } from 'react';
import { translate } from '../Services/translation';

class FacetedSearchGroup extends Component {
    constructor(props) {
        super(props);
        this.state = {
            collapsed: false,
            showLessItemCount : 5
        };
        this.toggleShowMore = this.toggleShowMore.bind(this);
    }

    componentDidMount() {
        const visible = this.refs.list.getBoundingClientRect().height !== 0;
        const tooMuchItem = this.props.group.options.length > this.state.showLessItemCount;
        visible && tooMuchItem && this.toggleShowMore();
    }

    toggleShowMore() {
        const topPos = this.refs.list.getBoundingClientRect().top;
        const lessBottomPos = this.refs.showLess.getBoundingClientRect().bottom;
        const moreBottomPos = this.refs.showMore.getBoundingClientRect().bottom;
        const setHeight = () => this.refs.list.style.height = `${ (this.state.collapsed ? lessBottomPos : moreBottomPos ) - topPos }px`;
        const toggleCollapse = () => {
            this.setState((prevState, props) => ({
                collapsed: !prevState.collapsed
            }),()=> {
                setHeight();
            });
        }
        if( !this.refs.list.style.height ) {
            setHeight();
            setTimeout(()=> {toggleCollapse()}, 0);
        }
        else {
            toggleCollapse();
        }
    }

    render() {
        const { group, searchFacetChange } = this.props;
        return (
            <Fragment>
                <ul className="faceted-search__group" ref="list">
                    <div className="faceted-search__group-header" role="faceted-search-item-group">{ group.label }</div>
                    {group.options && group.options.map((item, itemIndex, array) => (
                        <li key={`${item.label}-${itemIndex}`} className="faceted-search__item" role="faceted-search-item" ref={itemIndex===this.state.showLessItemCount-1 ? "showLess" : itemIndex===array.length-1 ? "showMore" : null}>
                            <FacetedFilterCheckbox item={item} group={group} searchFacetChange={searchFacetChange}/>
                        </li>
                    ))}
                </ul>
                {group.options.length > this.state.showLessItemCount &&
                    <span className="faceted-search__show-more" onClick={this.toggleShowMore}>
                        {`${ this.state.collapsed ? translate('filter.showmore') : translate('filter.showless')}`}
                    </span>
                }
            </Fragment>
        )
    }
}

const FacetedSearch = ({facetFilters, searchFacetChange, navigationTheme}) => navigationTheme !== 'category' && (
    <ul className="faceted-search">
        {facetFilters && facetFilters.map((group, groupIndex, array) => (
            <FacetedSearchGroup key={`${group.label}-${groupIndex}`} group={group} searchFacetChange={searchFacetChange}/>
        ))}
    </ul>
)

const FacetedFilterCheckbox = ({item, group, searchFacetChange}) => (
    <label className="faceted-filter" >
        <input className="faceted-filter__input" type="checkbox" onChange={event => searchFacetChange(group, item)} checked={group.selectedOptions != null && group.selectedOptions.includes(item.id)} />
        <span className="faceted-filter__label">
            {item.label}
            {!isNaN(item.quantity) && (item.quantity != null) && 
                <span className="faceted-filter__quantity">&nbsp;({item.quantity})</span>
            }
        </span>
    </label>
)

export default FacetedSearch;
