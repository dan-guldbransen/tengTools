import CookieNotificationMessage from '../Scripts/NoneReactComponents/cookieNotificationMessage';
import Dealers from '../Scripts/NoneReactComponents/dealers';
import Favorites from '../Scripts/NoneReactComponents/favorites';

window.addEventListener('DOMContentLoaded', (event) => {
   
    if (document.querySelector('.cookieNotification') !== null ) {
        new CookieNotificationMessage();
    }

    if (document.querySelector('.dealer-container') !== null) {
        new Dealers();
    }

    if (document.querySelector('#favorites') !== null) {
        new Favorites();
    }

});