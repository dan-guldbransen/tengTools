var CustomModuleIdsPlugin = require('custom-module-ids-webpack-plugin');
const TerserPlugin = require('terser-webpack-plugin');
const commonVendorsTest = require('litium-ui/common-vendors-module.js');

module.exports = {
    optimization: {
        chunkIds: 'named',
        minimizer: [new TerserPlugin({
            sourceMap: true,
            parallel: true,
            cache: true,
            terserOptions: {
                keep_fnames: true, // to keep components names
            }
        })],
        // set runtimeChunk to true to generate the runtime Accelerator file, to move webpackBootstrap
        // to runtime file, keeping Accelerator.js to clean, to not containing webpackBootstrap.
        // The dynamic component loading would not work if Accelerator.js contains webpackBootstrap
        runtimeChunk: {
            // name it as manifest then we will re-use the Litium Web's runtime
            name: 'manifest'
        },
        usedExports: false, // to keep named exports from libraries
        splitChunks: {
            cacheGroups: {
                commons: {
                    test: commonVendorsTest,
                    name: "vendor",
                    chunks: "all"
                }
            }
        },
    },
    plugins: [
        new CustomModuleIdsPlugin({
            idFunction: function(libIdent, module) {
                if (libIdent.startsWith('../../Litium.Client.UI/dist/litium-ui/')) {
                    return libIdent.replace('../../Litium.Client.UI/dist/litium-ui/', '../../node_modules/litium-ui/');
                }
                return libIdent;
            }
        })
    ]
}
