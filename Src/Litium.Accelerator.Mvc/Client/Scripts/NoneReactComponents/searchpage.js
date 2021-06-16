export default class SearchPage {
    constructor() {
        var btns = document.querySelectorAll('.searchtoggler');
        btns.forEach(btn => btn.addEventListener('click', this.toggle));
    }

    toggle(e) {
        if (e.target.classList.contains('btn-secondary'))
            return;

        var btns = document.querySelectorAll('.searchtoggler');
        var className = e.target.dataset.target;

        var produtlist = document.querySelector('.product-list');
        var otherhits = document.querySelector('.other-hits');

        if (className == 'product-list') {
            otherhits.classList.add('d-none');
            produtlist.classList.remove('d-none');
        }
        else if (className == 'other-hits') {
            otherhits.classList.remove('d-none');
            produtlist.classList.add('d-none');
        }

        btns.forEach(btn => {
            if (e.target === btn) {
                btn.classList.remove('btn-outline-dark');
                btn.classList.add('btn-secondary');
            }
            else {
                btn.classList.add('btn-outline-dark');
                btn.classList.remove('btn-secondary');
            }
        });
    }
}