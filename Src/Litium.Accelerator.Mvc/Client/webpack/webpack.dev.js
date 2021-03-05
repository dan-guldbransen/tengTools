const webpack = require('webpack');
const merge = require('webpack-merge');
const common = require('./webpack.common.js');

const ENV = process.env.ENV = 'development';

module.exports = merge(common, {
    mode: ENV,
    plugins: [
        new webpack.DefinePlugin({
            'process.env.NODE_ENV': JSON.stringify(ENV)
        })
    ],
});