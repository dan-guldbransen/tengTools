export default class UtilityMenu {
    constructor() {
        this.initMarketSelector();
    }

    initMarketSelector() {
        const utilityMenu = document.querySelector('.utilitymenu');

        utilityMenu.addEventListener('click', function () {
            document.querySelector('.utilitymenu_language_container').classList.toggle('active');
        });
    }
}