import Cookies from 'js-cookie';
import toastr from 'toastr';

export default class Favorites {
    constructor() {
        var addbtns = document.querySelectorAll('.addfavorites-btn');
        addbtns.forEach(btn => btn.addEventListener('click', this.addVariant));

        var deletebtns = document.querySelectorAll('.deletefavorites-btn');
        deletebtns.forEach(btn => btn.addEventListener('click', this.removeVariant));
    }

    addVariant(e) {
        // add variat to cookie
        var id = e.target.dataset.variantid;
        var name = e.target.dataset.variantname;
        var url = e.target.dataset.varianturl;

        var cookie = Cookies.get('favorites.variantids')

        toastr.options.closeButton = true;
        toastr.options.closeDuration = 300;

        if (cookie === undefined) {
            // create value
            Cookies.set('favorites.variantids', id, { expires: 365 });
            toastr.success('Added to favorites');

            var li = document.createElement("li");
            li.classList.add("list-group-item", "border-0", "d-flex", "align-items-center", "justify-content-between");

            var anchor = document.createElement("a");
            anchor.setAttribute("href", url);

            var p = document.createElement("p");
            p.classList.add("mb-0")
            p.innerText = name;

            var deleteBtn = document.createElement("a");
            deleteBtn.classList.add("deletefavorites-btn", "cursor-hover");
            deleteBtn.setAttribute('onclick', "removeVariant");

            var icon = document.createElement("i");
            icon.classList.add("bi", "bi-trash");
            icon.setAttribute("data-variantid", id);

            deleteBtn.appendChild(icon);
            anchor.appendChild(p);
            li.appendChild(anchor);
            li.appendChild(deleteBtn);

            document.querySelector('.favorites-list').appendChild(li);
        }
        else if (!cookie.includes(id)) {
            var newvalue = cookie + "," + id;
            Cookies.set('favorites.variantids', newvalue, { expires: 365 });
            toastr.success('Added to favorites');

            var li = document.createElement("li");
            li.classList.add("list-group-item", "border-0", "d-flex", "align-items-center", "justify-content-between");

            var anchor = document.createElement("a");
            anchor.setAttribute("href", url);

            var p = document.createElement("p");
            p.classList.add("mb-0")
            p.innerText = name;

            var deleteBtn = document.createElement("a");
            deleteBtn.classList.add("deletefavorites-btn", "cursor-hover");
            deleteBtn.setAttribute('onclick', "removeVariant");

            var icon = document.createElement("i");
            icon.classList.add("bi", "bi-trash");
            icon.setAttribute("data-variantid", id);

            deleteBtn.appendChild(icon);
            anchor.appendChild(p);
            li.appendChild(anchor);
            li.appendChild(deleteBtn);

            document.querySelector('.favorites-list').appendChild(li);
        }
        else {
            toastr.warning('Favorites already contains this product');
        }
    }

    removeVariant(e) {
        var id = e.target.dataset.variantid;
        var cookie = Cookies.get('favorites.variantids')

        toastr.options.closeButton = true;
        toastr.options.closeDuration = 300;

        if (cookie !== undefined) {
            var arr = cookie.split(',');

            for (var i = 0; i < arr.length; i++) {
                if (arr[i] == id) {

                    arr.splice(i, 1);
                }
            }

            var newvalue;
            for (var i = 0; i < arr.length; i++) {
                if (i == 0) {
                    newvalue = arr[i];
                }
                else {
                    newvalue = newvalue + "," + arr[i];
                }
            }

            Cookies.set('favorites.variantids', newvalue, { expires: 365 })
            var toremove = document.querySelector('.favorites-list').querySelector('[data-variantid="'+id+'"]').parentElement.parentElement
            toremove.remove();
            toastr.error('Removed from favorites');
        }
    }
}