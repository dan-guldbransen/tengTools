import axios from 'axios';
import Cookies from 'js-cookie'

export default class CoookieNotificationMessage {
    constructor() {
        const btns = document.querySelectorAll('.cookieNotificationBtn');
        btns.forEach(btn => btn.addEventListener('click', this.setCookie));

        const myModal = new bootstrap.Modal(document.querySelector('.cookieNotification'), {
            backdrop: 'static',
            keyboard: false
        });

        myModal.show();
    }

    setCookie(e) {
        const url = window.location.origin + "/api/common/cookieinfo";
        var accepted = e.target.dataset.accept;

        axios.get(url)
            .then(resp => {
                Cookies.set(resp.data.name, accepted, { expires: parseInt(resp.data.expires) });
            }).catch(ex => {
                console.log(ex);
            })
    }
}