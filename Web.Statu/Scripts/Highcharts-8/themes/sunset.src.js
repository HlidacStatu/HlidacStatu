/**
 * @license Highcharts JS v8.2.0 (2020-08-20)
 *
 * (c) 2009-2019 Highsoft AS
 *
 * License: www.highcharts.com/license
 */
'use strict';
(function (factory) {
    if (typeof module === 'object' && module.exports) {
        factory['default'] = factory;
        module.exports = factory;
    } else if (typeof define === 'function' && define.amd) {
        define('highcharts/themes/sunset', ['highcharts'], function (Highcharts) {
            factory(Highcharts);
            factory.Highcharts = Highcharts;
            return factory;
        });
    } else {
        factory(typeof Highcharts !== 'undefined' ? Highcharts : undefined);
    }
}(function (Highcharts) {
    var _modules = Highcharts ? Highcharts._modules : {};
    function _registerModule(obj, path, args, fn) {
        if (!obj.hasOwnProperty(path)) {
            obj[path] = fn.apply(null, args);
        }
    }
    _registerModule(_modules, 'Extensions/Themes/Sunset.js', [_modules['Core/Globals.js'], _modules['Core/Utilities.js']], function (Highcharts, U) {
        /* *
         *
         *  (c) 2010-2020 Highsoft AS
         *
         *  Author: Øystein Moseng
         *
         *  License: www.highcharts.com/license
         *
         *  Accessible high-contrast theme for Highcharts. Considers colorblindness and
         *  monochrome rendering.
         *
         *  !!!!!!! SOURCE GETS TRANSPILED BY TYPESCRIPT. EDIT TS FILE ONLY. !!!!!!!
         *
         * */
        var setOptions = U.setOptions;
        Highcharts.theme = {
            colors: ['#FDD089', '#FF7F79', '#A0446E', '#251535'],
            colorAxis: {
                maxColor: '#60042E',
                minColor: '#FDD089'
            },
            plotOptions: {
                map: {
                    nullColor: '#fefefc'
                }
            },
            navigator: {
                series: {
                    color: '#FF7F79',
                    lineColor: '#A0446E'
                }
            }
        };
        // Apply the theme
        setOptions(Highcharts.theme);

    });
    _registerModule(_modules, 'masters/themes/sunset.src.js', [], function () {


    });
}));