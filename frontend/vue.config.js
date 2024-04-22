const Dotenv = require('dotenv-webpack');

module.exports = {
    transpileDependencies: [
        'vuetify'
    ],
    lintOnSave: false,
    configureWebpack: {
        plugins: [
            new Dotenv(),
        ],
    },
};