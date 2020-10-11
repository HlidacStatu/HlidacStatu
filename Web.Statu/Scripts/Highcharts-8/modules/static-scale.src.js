/**
 * @license Highcharts Gantt JS v8.2.0 (2020-08-20)
 *
 * StaticScale
 *
 * (c) 2016-2019 Torstein Honsi, Lars A. V. Cabrera
 *
 * License: www.highcharts.com/license
 */
'use strict';
(function (factory) {
    if (typeof module === 'object' && module.exports) {
        factory['default'] = factory;
        module.exports = factory;
    } else if (typeof define === 'function' && define.amd) {
        define('highcharts/modules/static-scale', ['highcharts'], function (Highcharts) {
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
    _registerModule(_modules, 'Extensions/StaticScale.js', [_modules['Core/Globals.js'], _modules['Core/Utilities.js']], function (H, U) {
        /* *
         *
         *  (c) 2016-2020 Torstein Honsi, Lars Cabrera
         *
         *  License: www.highcharts.com/license
         *
         *  !!!!!!! SOURCE GETS TRANSPILED BY TYPESCRIPT. EDIT TS FILE ONLY. !!!!!!!
         *
         * */
        var addEvent = U.addEvent,
            defined = U.defined,
            isNumber = U.isNumber,
            pick = U.pick;
        var Chart = H.Chart;
        /* eslint-disable no-invalid-this */
        /**
         * For vertical axes only. Setting the static scale ensures that each tick unit
         * is translated into a fixed pixel height. For example, setting the static
         * scale to 24 results in each Y axis category taking up 24 pixels, and the
         * height of the chart adjusts. Adding or removing items will make the chart
         * resize.
         *
         * @sample gantt/xrange-series/demo/
         *         X-range series with static scale
         *
         * @type      {number}
         * @default   50
         * @since     6.2.0
         * @product   gantt
         * @apioption yAxis.staticScale
         */
        addEvent(H.Axis, 'afterSetOptions', function () {
            var chartOptions = this.chart.options && this.chart.options.chart;
            if (!this.horiz &&
                isNumber(this.options.staticScale) &&
                (!chartOptions.height ||
                    (chartOptions.scrollablePlotArea &&
                        chartOptions.scrollablePlotArea.minHeight))) {
                this.staticScale = this.options.staticScale;
            }
        });
        Chart.prototype.adjustHeight = function () {
            if (this.redrawTrigger !== 'adjustHeight') {
                (this.axes || []).forEach(function (axis) {
                    var chart = axis.chart,
                        animate = !!chart.initiatedScale &&
                            chart.options.animation,
                        staticScale = axis.options.staticScale,
                        height,
                        diff;
                    if (axis.staticScale && defined(axis.min)) {
                        height = pick(axis.brokenAxis && axis.brokenAxis.unitLength, axis.max + axis.tickInterval - axis.min) * staticScale;
                        // Minimum height is 1 x staticScale.
                        height = Math.max(height, staticScale);
                        diff = height - chart.plotHeight;
                        if (Math.abs(diff) >= 1) {
                            chart.plotHeight = height;
                            chart.redrawTrigger = 'adjustHeight';
                            chart.setSize(void 0, chart.chartHeight + diff, animate);
                        }
                        // Make sure clip rects have the right height before initial
                        // animation.
                        axis.series.forEach(function (series) {
                            var clipRect = series.sharedClipKey &&
                                    chart[series.sharedClipKey];
                            if (clipRect) {
                                clipRect.attr({
                                    height: chart.plotHeight
                                });
                            }
                        });
                    }
                });
                this.initiatedScale = true;
            }
            this.redrawTrigger = null;
        };
        addEvent(Chart, 'render', Chart.prototype.adjustHeight);

    });
    _registerModule(_modules, 'masters/modules/static-scale.src.js', [], function () {


    });
}));