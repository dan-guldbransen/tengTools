import UtilityMenu from '../Scripts/NoneReactComponents/utilitymenu';
import CookieNotificationMessage from '../Scripts/NoneReactComponents/cookieNotificationMessage';

window.addEventListener('DOMContentLoaded', (event) => {

    if (document.querySelector('.utilitymenu') !== undefined) {
        new UtilityMenu();
    }

    if (document.querySelector('.cookieNotification') !== undefined) {
        new CookieNotificationMessage();
    }
});