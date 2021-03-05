import { get } from '../Services/http';
import { catchError } from './Error.action';

export const NAVIGATION_LOAD = 'NAVIGATION_LOAD';
export const NAVIGATION_LOAD_ERROR = 'NAVIGATION_LOAD_ERROR';
export const NAVIGATION_RECEIVE = 'NAVIGATION_RECEIVE';
export const NAVIGATION_TOGGLE = 'NAVIGATION_TOGGLE';

export const load = () => (dispatch, getState) => {
    return get('/api/navigation')
        .then(response => response.json())
        .then(data => dispatch(receive(data)))
        .catch(ex => dispatch(catchError(ex, error => loadError(error))))
}

export const loadError = error => ({
    type: NAVIGATION_LOAD_ERROR,
    payload: {
        error,
    }
})

export const receive = data => ({
    type: NAVIGATION_RECEIVE,
    payload: {
        contentLinks : data.contentLinks,
    },
})
