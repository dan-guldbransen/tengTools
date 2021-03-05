import React from 'react';
import { connect } from 'react-redux';
import Navigation from '../Components/Navigation';

const NavigationContainer = props => (
    <Navigation {...props} />
)

const mapStateToProps = state => {
    return {
        links: state.navigation.contentLinks,
    }
}

const mapDispatchToProps = dispatch => {
    return {
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(NavigationContainer);