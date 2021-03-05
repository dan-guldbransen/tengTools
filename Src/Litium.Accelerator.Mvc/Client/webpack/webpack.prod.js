const webpack = require('webpack');
const merge = require('webpack-merge');
const common = require('./webpack.common.js');
const OptimizeCSSAssetsPlugin = require("optimize-css-assets-webpack-plugin");
const TerserPlugin = require('terser-webpack-plugin');

const ENV = process.env.ENV = 'production';

module.exports = merge(common, {
    mode: ENV,
    output: {
        filename: '[name].[contenthash].js',
    },
    optimization: {
        minimizer: [
            new TerserPlugin({
              cache: true,
              parallel: true,
              sourceMap: true // set to true if you want JS source maps
            }),
            new OptimizeCSSAssetsPlugin({})
        ]
    },
    plugins: [
        new webpack.DefinePlugin({
            'process.env.NODE_ENV': JSON.stringify(ENV)
        }),
    ],
});