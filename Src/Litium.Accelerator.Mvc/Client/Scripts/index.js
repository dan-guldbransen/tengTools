import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import { applyMiddleware, createStore } from 'redux';
import { composeWithDevTools } from 'redux-devtools-extension/developmentOnly';
import thunk from 'redux-thunk';
import app from './reducers';
import { historyMiddleware } from './Middlewares/History.middleware';
import MiniCartContainer from './Containers/MiniCart.container';
import QuickSearchContainer from './Containers/QuickSearch.container';
import NavigationContainer from './Containers/Navigation.container';
//import FacetedSearchContainer from './Containers/FacetedSearch.container';
import FacetedSearchCompactContainer from './Containers/FacetedSearchCompact.container';
import DynamicComponent from './Components/DynamicComponent';

window.__litium = window.__litium || {};
const preloadState = window.__litium.preloadState || {};
// use the parent page's store if possible, so the iframe will share the same store with the parent context.
// this to makes for example the Reorder button in My Page page to update the state and reload the mini cart
// when reorder action happens.
const store = window.__litium.store = window.__litium.store || (window.parent.__litium && window.parent.__litium.store)
    || createStore(app, preloadState, composeWithDevTools(applyMiddleware(thunk, historyMiddleware)));
window.__litium = {
    ...window.__litium,
    bootstrapComponents: () => {
        // bootstrap React components, in case the HTML response we receive from the server
        // has React components. ReactDOM.render performs only an update on previous rendered
        // components and only mutate the DOM as necessary to reflect latest React element.
        bootstrapComponents();
    },
    cache: {}, // for storing cache data for current request
};

const registeredContainers = [];
const renderReact = (element, container, callback) => {
    registeredContainers.push(container);
    ReactDOM.render(element, container, callback);
}

window.onunload = () => {
    // make sure components are unmounted, redux's listener are unsubscribed
    // to fix issue with iframe navigation in IE
    registeredContainers.forEach(container => {
        ReactDOM.unmountComponentAtNode(container);
    });
}

const bootstrapComponents = () => {
    //if (document.getElementById('miniCart')) {
    //    renderReact(
    //        <Provider store={store}>
    //            <MiniCartContainer />
    //        </Provider>,
    //        document.getElementById('miniCart')
    //    );
    //}
    if (document.getElementById('quickSearch')) {
        renderReact(
            <Provider store={store}>
                <QuickSearchContainer />
            </Provider>,
            document.getElementById('quickSearch')
        );
    }
    if (document.getElementById('navbar')) {
        renderReact(
            <Provider store={store}>
                <NavigationContainer />
            </Provider>,
            document.getElementById('navbar')
        );
    }
    if (document.getElementById('facetedSearch')) {
        renderReact(
            <Provider store={store}>
                <FacetedSearchContainer />
            </Provider>,
            document.getElementById('facetedSearch')
        );
    }
    if (document.getElementById('facetedSearchCompact')) {
        renderReact(
            <Provider store={store}>
                <FacetedSearchCompactContainer />
            </Provider>,
            document.getElementById('facetedSearchCompact')
        );
    }
    if (document.getElementById('myPagePersons')) {
        const PersonList = DynamicComponent({
            loader: () => import('./Containers/PersonList.container')
        });
        renderReact(
            <Provider store={store}>
                <PersonList />
            </Provider>,
            document.getElementById('myPagePersons')
        );
    }
    if (document.getElementById('myPageAddresses')) {
        const AddressList = DynamicComponent({
            loader: () => import('./Containers/AddressList.container')
        });
        renderReact(
            <Provider store={store}>
                <AddressList />
            </Provider>,
            document.getElementById('myPageAddresses')
        );
    }
    if (document.getElementById('checkout')) {
        const Checkout = DynamicComponent({
            loader: () => import('./Containers/Checkout.container')
        });
        renderReact(
            <Provider store={store}>
                <Checkout />
            </Provider>,
            document.getElementById('checkout')
        );
    }
    if (document.getElementById('lightBoxImages')) {
        const LightboxImages = DynamicComponent({
            loader: () => import('./Containers/LightboxImages.container')
        });
        const rootElement = document.getElementById('lightBoxImages');
        const images = Array.from(rootElement.querySelectorAll('a')).map(img => ({ src: img.dataset.src }));
        const thumbnails = Array.from(rootElement.querySelectorAll('a img')).map(img => ({ src: img.src }));
        renderReact(
            <Provider store={store}>
                <LightboxImages images={images} thumbnails={thumbnails} />
            </Provider>,
            document.getElementById('lightBoxImages')
        );
    }

    if (document.querySelectorAll('.slider').length > 0) {
        const Slider = DynamicComponent({
            loader: () => import('./Components/Slider')
        });
        Array.from(document.querySelectorAll('.slider')).forEach((slider, index) => {
            const values = [...slider.querySelectorAll('.slider__block')].map(block => {
                return ({
                    image: block.dataset.image,
                    url: block.dataset.url,
                    text: block.dataset.text,
                })
            });
            if (values.length > 0) {
                renderReact(
                    <Slider values={values} />,
                    slider
                );
            }
        });
    }

    if (document.querySelectorAll('buy-button').length > 0) {
        const BuyButton = DynamicComponent({
            loader: () => import('./Containers/BuyButton.Container')
        });
        Array.from(document.querySelectorAll('buy-button')).forEach((button) => {
            const { articleNumber, quantityFieldId, href, cssClass, label } = button.dataset;
            renderReact(
                <Provider store={store}>
                    <BuyButton {...{ label, articleNumber, quantityFieldId, href, cssClass }} />
                </Provider>,
                button
            );
        });
    }

    if (document.querySelectorAll('reorder-button').length > 0) {
        const ReorderButton = DynamicComponent({
            loader: () => import('./Containers/ReorderButton.Container')
        });
        Array.from(document.querySelectorAll('reorder-button')).forEach((button) => {
            const { cssClass, orderId, title } = button.dataset;
            const label = button.innerText;
            renderReact(
                <Provider store={store}>
                    <ReorderButton {...{ label, title, cssClass, orderId }} />
                </Provider>,
                button
            );
        });
    }
}

bootstrapComponents();