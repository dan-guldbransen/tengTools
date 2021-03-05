import { post } from './http';

export const add = async ({ articleNumber, quantity = 1 }) => {
    if (!quantity || isNaN(quantity) || parseInt(quantity) <= 0) {
        throw "Invalid quantity";
    }

    const response = await post('/api/cart/add', { articleNumber, quantity: parseInt(quantity) });
    return response.json();
}

export const reorder = async (orderId) => {
    const response = await post('/api/cart/reorder', { orderId });
    return response.json();
}