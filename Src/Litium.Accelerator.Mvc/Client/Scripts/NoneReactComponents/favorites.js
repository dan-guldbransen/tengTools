import axios from 'axios';
import Cookies from 'js-cookie';


export default class Favorites {
    constructor() {
        var addbtns = document.querySelectorAll('.addfavorites-btn');
        addbtns.forEach(btn => btn.addEventListener('click', this.addVariant));

        var deletebtns = document.querySelectorAll('.deletefavorites-btn');
        deletebtns.forEach(btn => btn.addEventListener('click', this.removeVariant));
    }

    addVariant(e) {

        // add variat to cookie

        var cookie = Cookies.get('favorites.variantsystemids')
        console.log(cookie);
        if (cookie === null) {
            // create value
        }

        // set value

        // update ui ajax-req
    }

    removeVariant(e) {
        var cookie = Cookies.get('favorites.variantsystemids')
        // remove from cookie value

        // set value

        // delete from ui
    }
}