import React from 'react';
import 'react-responsive-carousel/lib/styles/carousel.min.css';
import { Carousel } from 'react-responsive-carousel';

const CarouselSettings = {
    showStatus: false,
    showThumbs: false,
    infiniteLoop: true,
};

const Slider = ({ values }) =>
    <Carousel {...CarouselSettings}>
        {values.map((value, index) =>
            <a className="slider__link" href={value.url} key={`figure${index}`}>
                <img className="slider__image" src={value.image}/>
                <h2 className="slider__text">{value.text}</h2>
            </a>
        )}
    </Carousel>

export default Slider;