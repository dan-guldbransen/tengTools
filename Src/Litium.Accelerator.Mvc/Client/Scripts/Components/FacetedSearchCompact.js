import React, { Fragment } from 'react';
import { translate } from "../Services/translation";

const FacetedSearchCompact = ({facetFilters, visibleDropdownMenu, toggleVisibleDropdownMenu, onFacetChange, onSubmit}) => (
    <ul className="faceted-search faceted-search--compact row" >
        {facetFilters && facetFilters.map((group, groupIndex, array) => group.options && group.options.length > 0 && (
            <li className="columns small-6 large-3 faceted-search__group" key={`${group.label}-${groupIndex}`}>
                <div className={`faceted-search__group-header ${visibleDropdownMenu.includes(group.id)? 'faceted-search__group-header--show-compact':''}`} role="faceted-search-item-group" onClick={event => toggleVisibleDropdownMenu(group)}>
                    { group.label }
                </div>
                <ul className="faceted-search__sublist">
                    {group.options && group.options.map((item, itemIndex, array) => (
                        <li key={`${item.label}-${itemIndex}`} className="faceted-search__item" role="faceted-search-item">
                            <FacetedFilterCheckbox item={item} group={group} onFacetChange={onFacetChange}/>
                        </li>
                    ))}
                    <li className="faceted-search__item" >
                        <button className="filter__button" onClick={event => {toggleVisibleDropdownMenu(group);onSubmit(facetFilters)}}>{translate('general.select')}</button>
                    </li>
                </ul>
            </li>
        ))}
    </ul>
)

const FacetedFilterCheckbox = ({item, group, onFacetChange}) => (
    <label className="faceted-filter" >
        <input className="faceted-filter__input" type="checkbox" onChange={event => onFacetChange(group, item)} checked={group.selectedOptions.includes(item.id)} />
        <span className="faceted-filter__label">
            {item.label}
            {!isNaN(item.quantity) && item.quantity > 0 &&
                <span className="faceted-filter__quantity"> ({item.quantity})</span>
            }
        </span>
    </label>
)

export default FacetedSearchCompact;
