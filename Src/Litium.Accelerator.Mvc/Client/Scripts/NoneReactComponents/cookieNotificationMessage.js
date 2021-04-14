export default class CoookieNotificationMessage {
    constructor() {
        this.init();
    }

    init() {
        const cookieNotificationMessage = document.getElementById('cookieNotificationMessage');
        const acceptButton = cookieNotificationMessage.getElementById('accept-cookies');
        const rejectButton = cookieNotificationMessage.getElementById('reject-cookies');

        acceptButton.addEventListener('click', this.accept);
        rejectButton.addEventListener('click', this.reject);
    }

    accept() {
        console.log('accept');
    }

    reject() {
        console.log('reject');
    }
}