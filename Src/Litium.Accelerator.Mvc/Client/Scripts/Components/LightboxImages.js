import React, { Fragment } from 'react';
import Lightbox from 'react-images';

const LightboxImages = props => (
    !props.images || props.images.length < 1 ? <Fragment /> :
    <Fragment>
        <div className="row product-images">
            <div className="col-12">
                <figure className="product-detail__image-container">
                    <a data-src={props.images[0].src} itemProp="url" onClick={() => props.onClickThumbnail(0)} className="product-image-main">
                        <img className="rounded" itemProp="image" src={props.thumbnails[0].src} />
                    </a>
                </figure>
            </div>
            <div className="col-12">
                <div className="d-flex align-items-center justify-content-start">
                    {props.images.map((image, index) => (
                        index > 0 &&
                        <div className="product-detail__image-container me-3" key={index}>
                            <a data-src={image.src} itemProp="url" onClick={() => props.onClickThumbnail(index)} className="product-image cursor-hover">
                                <img className="product-detail__image--alter " itemProp="image" src={props.thumbnails[index].src} />
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