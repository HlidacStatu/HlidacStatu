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
        define('highcharts/themes/avocado', ['highcharts'], function (Highcharts) {
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
    _registerModule(_modules, 'Extensions/Themes/Avocado.js', [_modules['Core/Globals.js'], _modules['Core/Utilities.js']], function (Highcharts, U) {
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
            colors: ['#F3E796', '#95C471', '#35729E', '#251735'],
            colorAxis: {
                maxColor: '#05426E',
                minColor: '#F3E796'
            },
            plotOptions: {
                map: {
                    nullColor: '#FCFEFE'
                }
            },
            navigator: {
                maskFill: 'rgba(170, 205, 170, 0.5)',
                series: {
                    color: '#95C471',
                    lineColor: '#35729E'
                }
            }
        };
        // Apply the theme
        setOptions(Highcharts.theme);

    });
    _registerModule(_modules, 'masters/themes/avocado.src.js', [], function () {


    });
}));