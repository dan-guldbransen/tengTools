import axios from 'axios';
import Cookies from 'js-cookie'

export default class CoookieNotificationMessage {
    constructor() {
        const btns = document.querySelectorAll('.cookieNotificationBtn');
        btns.forEach(btn => btn.addEventListener('click', this.setCookie));
    }

    setCookie(e) {
        const url = window.location.origin + "/api/common/cookieinfo";
        var accepted = e.target.dataset.accept;
        console.log(accepted);

        axios.get(url)
            .then(resp => {
                Cookies.set(resp.data.name, accepted, { expires: parseInt(resp.data.expires) });
                document.querySelector('.cookieNotification').remove();
            }).catch(ex => {
                console.log(ex);
            })
    }
}