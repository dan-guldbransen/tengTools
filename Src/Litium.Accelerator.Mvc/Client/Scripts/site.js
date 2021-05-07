import CookieNotificationMessage from '../Scripts/NoneReactComponents/cookieNotificationMessage';

window.addEventListener('DOMContentLoaded', (event) => {
   
    if (document.querySelector('.cookieNotification') !== null ) {
        new CookieNotificationMessage();
    }

});