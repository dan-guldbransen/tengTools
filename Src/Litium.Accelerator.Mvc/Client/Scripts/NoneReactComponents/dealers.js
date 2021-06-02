import { isEmptyObject } from "jquery";

export default class Dealers {
    constructor() {
        if (document.querySelector('.dealer-search') !== null) {
            document.querySelector('.dealer-search').addEventListener('keyup', this.filter);
        }

        if (document.querySelectorAll('.dealer-check').length > 0) {
            document.querySelectorAll('.dealer-check').forEach(el => el.addEventListener('click', this.filter));
        }
    }

    filter() {
        var showEcom, showPhysical;

        showPhysical = document.getElementById('physicalCheckbox').checked;
        showEcom = document.getElementById('ecomCheckbox').checked;

        var query = document.getElementById('query').value;
        var items = document.querySelectorAll('.accordion-item');

        items.forEach(item => {
            var isEcom = item.dataset.isecom;
            var shouldShow;

            if (isEcom == "True" && !showEcom) {
                shouldShow = false;
            }
            else if (isEcom == "False" && !showPhysical) {
                shouldShow = false;
            }
            else {
                shouldShow = true;
            }

            if (query.length > 0) {
                if (!item.dataset.value.toLowerCase().includes(query.toLowerCase())) {
                    shouldShow = false;
                }
            }

            if (shouldShow && item.classList.contains("d-none")) {
                item.classList.remove("d-none");
            }
            else if (!shouldShow && !item.classList.contains("d-none")) {
                item.classList.add("d-none");
            }

        });

        var headers = document.querySelectorAll('.accordion');

        headers.forEach(head => {
            var items = head.querySelectorAll('.accordion-item:not(.d-none)');

            var shouldShow = false;
            if (items.length > 0) {
                shouldShow = true;
            }

            var headline = head.previousElementSibling;

            if (shouldShow && headline.classList.contains('d-none')) {
                headline.classList.remove('d-none');
            }
            else if (!shouldShow && !headline.classList.contains('d-none')) {
                headline.classList.add('d-none');
            }
        });
    }
}