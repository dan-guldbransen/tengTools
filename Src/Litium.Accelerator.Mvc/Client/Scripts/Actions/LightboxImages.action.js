export const LIGHTBOX_IMAGES_SET_CURRENT_IMAGE = 'LIGHTBOX_IMAGES_SET_CURRENT_IMAGE';
export const LIGHTBOX_IMAGES_SHOW = 'LIGHTBOX_IMAGES_SHOW';
export const LIGHTBOX_IMAGES_NEXT = 'LIGHTBOX_IMAGES_NEXT';
export const LIGHTBOX_IMAGES_PREVIOUS = 'LIGHTBOX_IMAGES_PREVIOUS';

export const setCurrentIndex = (index) => ({
    type: LIGHTBOX_IMAGES_SET_CURRENT_IMAGE,
    payload: {
        index,
    }
})

export const show = (visible) => ({
    type: LIGHTBOX_IMAGES_SHOW,
    payload: {
        visible,
    }
})

export const next = () => ({
    type: LIGHTBOX_IMAGES_NEXT,
})

export const previous = () => ({
    type: LIGHTBOX_IMAGES_PREVIOUS,
})