import React from 'react';

const BuyButton = ({ label, articleNumber, quantityFieldId, href, cssClass, onClick }) => (
    articleNumber ? 
    <a className={cssClass} onClick={() => onClick(articleNumber, quantityFieldId)}>
        {label}
    </a>
    : <a className={cssClass} href={href}>
        {label}
    </a>
)

export default BuyButton;