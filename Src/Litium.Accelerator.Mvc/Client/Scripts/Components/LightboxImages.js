import React, { Fragment } from 'react';
import Lightbox from 'react-images';

const LightboxImages = props => (
    !props.images || props.images.length < 1 ? <Fragment /> :
    <Fragment>
        <div className="row product-images">
            <div className="small-12 large-9 columns large-order-1">
                <figure className="product-detail__image-container">
                    <a data-src={props.images[0].src} itemProp="url" onClick={() => props.onClickThumbnail(0)} className="product-image">
                        <img className="shadow" itemProp="image" src={props.thumbnails[0].src} />
                    </a>
                </figure>
            </div>
            <div className="small-12 large-3 columns medium-flex-dir-column">
                <div className="row">
                    {props.images.map((image, index) => (
                        index > 0 &&
                        <div className="product-detail__image-container columns large-12" key={index}>
                            <a data-src={image.src} itemProp="url" onClick={() => props.onClickThumbnail(index)} className="product-image">
                                <img className="product-detail__image--alter" itemProp="image" src={props.thumbnails[index].src} />
                            </a>
                        </div>
                    ))}
                </div>
            </div>
        </div>
        <Lightbox 
            showThumbnails={true}
            showImageCount={false}
            {...props}
        />
    </Fragment>
)

export default LightboxImages;