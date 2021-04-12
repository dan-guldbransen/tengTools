import UtilityMenu from '../Scripts/NoneReactComponents/utilitymenu';

window.addEventListener('DOMContentLoaded', (event) => {

    if (document.querySelector('.utilitymenu') !== undefined) {
        new UtilityMenu();
    }

});