import { combineReducers } from 'redux';
import { cart } from './Reducers/Cart.reducer';
import { quickSearch } from './Reducers/QuickSearch.reducer';
import { navigation } from './Reducers/Navigation.reducer';
import { facetedSearch } from "./Reducers/FacetedSearch.reducer";
import { myPage } from './Reducers/MyPage.reducer';
import { checkout } from "./Reducers/Checkout.reducer";
import { lightboxImages } from './Reducers/LightboxImages.reducer';

const app = combineReducers({
    cart,
    checkout,
    quickSearch,
    navigation,
    myPage,
    facetedSearch,
    lightboxImages,
});

export default app;