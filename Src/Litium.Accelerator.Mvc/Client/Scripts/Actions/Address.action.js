import { get, post, put, remove as httpRemove } from '../Services/http';
import { catchError } from './Error.action';

export const ADDRESS_RECEIVE = 'ADDRESS_RECEIVE';
export const ADDRESS_ERROR = 'ADDRESS_ERROR';
export const ADDRESS_CHANGE_MODE = 'ADDRESS_CHANGE_MODE';

const rootRoute = '/api/mypageaddress';

export const changeMode = mode => ({
    type: ADDRESS_CHANGE_MODE,
    payload: {
        mode,
    }
})

export const query = (mode = 'list') => (dispatch) => {
    return get(rootRoute)
        .then(response => response.json())
        .then(result => {
            dispatch(receive(result, mode));
        })
        .catch(ex => dispatch(catchError(ex, error => setError(error))))
        ;
}

export const remove = (addressSystemId) => (dispatch) => 
    httpRemove(rootRoute, addressSystemId)
        .then(() => dispatch(query()))
        .catch(ex => dispatch(catchError(ex, error => setError(error))))
        ;

export const add = (address) => (dispatch) =>
    post(rootRoute, address)
        .then(() => dispatch(query()))
        .catch(ex => dispatch(catchError(ex, error => setError(error))))
        ;

export const edit = (address) => (dispatch) =>
    put(rootRoute, address)
        .then(() => dispatch(query()))
        .catch(ex => dispatch(catchError(ex, error => setError(error))))
        ;

const receive = (list, mode) => ({
    type: ADDRESS_RECEIVE,
    payload: {
        list,
        mode,
    },
})

export const setError = error => ({
    type: ADDRESS_ERROR,
    payload: {
        error,
    },
})