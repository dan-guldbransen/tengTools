

export const error = (state = {}, action) => {
    const { error } = action.payload;
    if (!error) {
        return state;
    }
    if (error.modelState) {
        return error.modelState;
    }
    if (error.name === 'ValidationError') {
        return {
            [error.path]: error.errors,
        };
    }
    return state;
}