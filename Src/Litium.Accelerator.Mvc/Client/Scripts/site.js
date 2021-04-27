import CookieNotificationMessage from '../Scripts/NoneReactComponents/cookieNotificationMessage';

window.addEventListener('DOMContentLoaded', (event) => {
   
    if (document.querySelector('.cookieNotification') !== null ) {
        new CookieNotificationMessage();
    }

    if (document.querySelector('.mobile-menu') !== null) {
        new Mmenu('.mobile-menu', {
            "counters": true
        });
    } 
    
});