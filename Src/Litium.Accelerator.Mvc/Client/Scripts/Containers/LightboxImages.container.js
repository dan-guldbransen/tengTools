import React from 'react';
import { connect } from 'react-redux';
import LightboxImages from '../Components/LightboxImages';
import { setCurrentIndex, show, previous, next } from '../Actions/LightboxImages.action';

const LightboxImagesContainer = props => <LightboxImages {...props} />;

const mapStateToProps = (state) => {
    return {
        isOpen: state.lightboxImages.visible,
        currentImage: state.lightboxImages.index,
    }
}

const mapDispatchToProps = dispatch => {
    return {
        onClose: () => dispatch(show(false)),
        onClickNext: () => dispatch(next()),
        onClickPrev: () => dispatch(previous()),
        onClickThumbnail: (index) => {
            dispatch(show(true));
            dispatch(setCurrentIndex(index));
        },
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(LightboxImagesContainer);