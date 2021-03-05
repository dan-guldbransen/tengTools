import { PERSON_RECEIVE, PERSON_ERROR, PERSON_CHANGE_MODE } from '../Actions/Person.action';
import { ADDRESS_RECEIVE, ADDRESS_ERROR, ADDRESS_CHANGE_MODE } from '../Actions/Address.action';
import { person as personReducer } from './Person.reducer';
import { address as addressReducer } from './Address.reducer';

const defaultState = {
    persons: {},
    addresses: {},
}

export const myPage = (state = defaultState, action) => {
    const { type, payload } = action;
    switch (type) {
        case PERSON_RECEIVE:
        case PERSON_ERROR:
        case PERSON_CHANGE_MODE:
            return {
                ...state,
                persons: personReducer(state.persons, action),
            }
        case ADDRESS_RECEIVE:
        case ADDRESS_ERROR:
        case ADDRESS_CHANGE_MODE:
            return {
                ...state,
                addresses: addressReducer(state.addresses, action),
            }
        default:
            return state;
    }
}