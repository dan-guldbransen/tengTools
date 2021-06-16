import React, { Fragment } from 'react';

const sameCategory = (item, index, array) => index > 0 && array[index - 1].category === array[index].category || item.showAll;

const QuickSearchResultMobile = ({ result, selectedItem, searchUrl, onClose }) => (
    <ul className="quick-search-result dropdown-menu show dropdown-menu-arrow p-4">
        <li className="quick-search-close">
            <div className="media" onClick={e => onClose()}></div>
        </li>
        {result && result.map((item, index, array) => (
            <Fragment key={`${item.name}-${index}`}>
                {item.category === 'NoHit' || sameCategory(item, index, array) ? null :
                    <li className="quick-search-result__item quick-search-result__group-header" ><h5> {item.category}</h5></li>
                }
                <li className={`quick-search-result__item dropdown-header ${selectedItem === index ? 'quick-search-result__item--selected' : ''}`} >
                    <a className={item.showAll ? 'quick-search-result__show-all dropdown-item d-flex align-items-center' : `d-flex align-items-center quick-search-result__link dropdown-item ${item.url ? '' : 'dropdown-item quick-search-result__link--disabled d-flex align-items-center'}`}
                        href={item.showAll ? searchUrl : item.url}>
                        {item.hasImage && item.imageSource && <img className="quick-search-result__image me-2" src={item.imageSource} />}
                        <div>{item.name}</div>
                    </a>
                </li>
                {item.category === 'NoHit' || sameCategory(item, index, array) ? null : <li><hr className="dropdown-divider" /></li>}
            </Fragment>
        ))}
    </ul>
)

export default QuickSearchResultMobile;