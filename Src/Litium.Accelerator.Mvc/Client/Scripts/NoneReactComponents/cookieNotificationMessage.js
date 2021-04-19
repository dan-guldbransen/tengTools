export default class CoookieNotificationMessage {
    constructor() {
        this.init();
    }

    init() {
        const cookieNotificationMessage = document.getElementById('cookieNotificationMessage');
        const acceptButton = cookieNotificationMessage.getElementById('accept-cookies');
        const rejectButton = cookieNotificationMessage.getElementById('reject-cookies');

        acceptButton.addEventListener('click', sendConsentCookies(true));
        rejectButton.addEventListener('click', sendConsentCookies(false));
    }

    
    function sendConsentCookies(consent) {

        $.ajax({
            type: "POST"
            url: "Layout/CookieNotificationMessage",
            data: JSON.stringify(consent)
        });

        this.cookieNotificationMessage.style.display = "none";

    }
}