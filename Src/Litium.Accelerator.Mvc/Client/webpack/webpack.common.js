const path = require('path');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;

const ROOT = path.resolve(__dirname, '../');
const BUILD_DIR = path.resolve(ROOT, '../ui');
const JS_DIR = path.resolve(ROOT, 'Scripts');
const CSS_DIR = path.resolve(ROOT, 'Styles');

module.exports = {
    context: ROOT,
    target: 'web',
    entry: {
        app : [
            '@babel/polyfill',
            'whatwg-fetch',
            path.resolve(JS_DIR, 'index.js'),
            path.resolve(CSS_DIR, 'site.scss'),
            path.resolve(JS_DIR, 'site.js'),
        ],
    },
    output: {
        path: BUILD_DIR,
        publicPath: '/ui/',
        filename: '[name].js',
    },
    devtool: 'source-map',
    module : {
        rules : [
            {
                test : /\.js(x?)$/,
                include : JS_DIR,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader'
                }
            },
            {
                test: /\.css$/,  
                include: /node_modules/,  
                loaders: ['style-loader?sourceMap=true', 'css-loader?sourceMap=true'],
            },
            {
                test: /\.scss$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    "css-loader?sourceMap=true", // translates CSS into CommonJS
                    "resolve-url-loader", // resolve relative url of assets in scss files if they are imported from node_modules
                    "sass-loader?sourceMap=true" // compiles Sass to CSS, using Node Sass by default
                ]
            },
            {
                test: /\.svg$/,
                exclude: /(\/fonts)/,
                loader: 'svg-url-loader'
            },
            {
                test: /\.(png|jpg|jpeg|gif|ico)/i,
                loader: 'file-loader',
                exclude: /(\/fonts)/,
                options: {
                    name: '[name].[ext]',
                    outputPath: 'images',
                    publicPath: '../images'
                }
            },
            {
                test: /\.(woff(2)?|ttf|eot|svg)/i,
                loader: 'file-loader',
                include: [
                    /fonts/,
                    /node_modules/
                ],
                options: {
                    name: '[name].[ext]',
                    outputPath: 'fonts',
                    publicPath: '../fonts'
                }
            }
        ]
    },
    plugins: [
        new CleanWebpackPlugin({
            cleanOnceBeforeBuildPatterns: [BUILD_DIR + '/*.*'],
            verbose: false, //todo: fix better solution
        }),
        new MiniCssExtractPlugin({
            filename: 'css/site.min.css'
        }),
        new CopyWebpackPlugin([
            { from: path.resolve(ROOT, 'images'), to: path.resolve(BUILD_DIR, 'images') },
            //{ from: path.resolve(ROOT, 'bootstrap'), to: path.resolve(BUILD_DIR, 'bootstrap') },
            //{ from: path.resolve(ROOT, 'mmenu'), to: path.resolve(BUILD_DIR, 'mmenu') }
        ]),
        // new BundleAnalyzerPlugin(),
    ],
    resolve: {
        extensions: [ '.tsx', '.ts', '.js' ]
    }
};