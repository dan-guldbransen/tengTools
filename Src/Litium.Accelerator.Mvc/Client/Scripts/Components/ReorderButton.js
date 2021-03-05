import React from 'react';

const ReorderButton = ({ label, title, cssClass, orderId, onClick }) => (
    <button className={cssClass} type="button" title={title} 
        onClick={() => onClick(orderId)} >{label}</button>
)

export default ReorderButton;