import { 
    LIGHTBOX_IMAGES_SET_CURRENT_IMAGE,
    LIGHTBOX_IMAGES_SHOW,
    LIGHTBOX_IMAGES_NEXT,
    LIGHTBOX_IMAGES_PREVIOUS,
} from '../Actions/LightboxImages.action';

const defaultState = { 
    index: 0,
    visible: false,
}

export const lightboxImages = (state = defaultState, action) => {
    const { type, payload } = action;
    switch (type) {
        case LIGHTBOX_IMAGES_SET_CURRENT_IMAGE:
        case LIGHTBOX_IMAGES_SHOW:
            return {
                ...state,
                ...payload,
            };
        case LIGHTBOX_IMAGES_NEXT:
            return {
                ...state,
                index: state.index + 1,
            }
        case LIGHTBOX_IMAGES_PREVIOUS:
            return {
                ...state,
                index: state.index - 1,
            }
        default:
            return state;
    }
}