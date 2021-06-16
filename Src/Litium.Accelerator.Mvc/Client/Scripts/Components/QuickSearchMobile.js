import React, { Component } from 'react';
import QuickSearchResultMobile from './QuickSearchResultMobile';
import { translate } from '../Services/translation';

class QuickSearchMobile extends Component {
    focusOnInput() {
        setTimeout(() => {
            this.searchInput && this.searchInput.focus();
        }, 0);
    }

    render() {
        const { query, result, showResult, showFullForm, onSearch, onBlur, onKeyDown, toggleShowFullForm, selectedItem, searchUrl, onClose } = this.props;
        return (
            <div className="quick-search row my-2 mx-3 p-2" role="search">
                <a className="quick-search__link--block text-right p-0 col-1" onClick={e => { toggleShowFullForm(); this.focusOnInput() }}>
                    <span className="quick-search__title">{translate('general.search')}:</span>
                </a>
                <div className={`quick-search__form p-0 col-11 d-flex align-items-center justify-content-between ${showFullForm ? 'quick-search__form--force-show' : ''}`} role="search">
                    <div className={`dropdown w-100 ps-2 ${showResult ? 'quick-search__arrow' : ''}'`}>
                        <input className="quick-search__input" type="search"
                            autoComplete="off" value={decodeURIComponent(query)} onChange={event => onSearch(encodeURIComponent(event.target.value))}
                            onKeyDown={event => onKeyDown(event, { searchUrl })} ref={(input) => { this.searchInput = input; }}
                            onBlur={() => onBlur()} />
                        {showResult && <QuickSearchResultMobile result={result} selectedItem={selectedItem} searchUrl={searchUrl} onClose={onClose} />}
                    </div>
                    <button className="quick-search__submit-button" type="submit">
                        <i className="quick-search__submit-icon bi bi-search"></i>
                    </button>
                </div>
            </div>
        )
    }
}

export default QuickSearchMobile;