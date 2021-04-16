export default class CoookieNotificationMessage {
    constructor() {
        this.init();
    }

    init() {
        const cookieNotificationMessage = document.getElementById('cookieNotificationMessage');
        const acceptButton = cookieNotificationMessage.getElementById('accept-cookies');
        const rejectButton = cookieNotificationMessage.getElementById('reject-cookies');
        const closeButton = cookieNotificationMessage.getElementById('close-cookies');

        acceptButton.addEventListener('click', accept);
        rejectButton.addEventListener('click', reject);
        closeButton.addEventListener('click', close);
    }

    function accept() {
        console.log('accept');

        $.ajax({
            type: "POST",
            url: "Layout/CookieNotificationMessage",
            data: JSON.stringify(true)
        });

        this.cookieNotificationMessage.style.display = "none";
    }

    function reject() {
        console.log('reject');

        $.ajax({
            type: "POST",
            url: "Layout/CookieNotificationMessage",
            data: JSON.stringify(false)
        });

        this.cookieNotificationMessage.style.display = "none";
    }

    function close() {
        $.ajax({
            type: "POST",
            url: "Layout/CookieNotificationMessage",
            data: JSON.stringify(null)
        });

        this.cookieNotificationMessage.style.display = "none";
    }
}