import CookieNotificationMessage from '../Scripts/NoneReactComponents/cookieNotificationMessage';

window.addEventListener('DOMContentLoaded', (event) => {
   
    if (document.querySelector('.cookieNotification') !== null ) {
        new CookieNotificationMessage();
    }

    if (document.querySelector('.mobile-menu') !== null) {
        new Mmenu('.mobile-menu', {
            offCanvas: false,
            counters: true
        });
    }

    if (document.querySelector('#mobile-menu-toggle') !== null) {
        const menu = new Mmenu("#mobile-menu");
        const api = menu.API;

        document.querySelector('#mobile-menu-toggle').addEventListener('click', function (e) {
            e.preventDefault();
            api.open();
        });
    }
});