/**
 * @license Highmaps JS v8.2.0 (2020-08-20)
 *
 * (c) 2009-2019 Torstein Honsi
 *
 * License: www.highcharts.com/license
 */
'use strict';
(function (factory) {
    if (typeof module === 'object' && module.exports) {
        factory['default'] = factory;
        module.exports = factory;
    } else if (typeof define === 'function' && define.amd) {
        define('highcharts/modules/heatmap', ['highcharts'], function (Highcharts) {
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
    _registerModule(_modules, 'Mixins/ColorSeries.js', [_modules['Core/Globals.js']], function (H) {
        /* *
         *
         *  (c) 2010-2020 Torstein Honsi
         *
         *  License: www.highcharts.com/license
         *
         *  !!!!!!! SOURCE GETS TRANSPILED BY TYPESCRIPT. EDIT TS FILE ONLY. !!!!!!!
         *
         * */
        /**
         * Mixin for maps and heatmaps
         *
         * @private
         * @mixin Highcharts.colorPointMixin
         */
        H.colorPointMixin = {
            /* eslint-disable valid-jsdoc */
            /**
             * Set the visibility of a single point
             * @private
             * @function Highcharts.colorPointMixin.setVisible
             * @param {boolean} visible
             * @return {void}
             */
            setVisible: function (vis) {
                var point = this,
                    method = vis ? 'show' : 'hide';
                point.visible = point.options.visible = Boolean(vis);
                // Show and hide associated elements
                ['graphic', 'dataLabel'].forEach(function (key) {
                    if (point[key]) {
                        point[key][method]();
                    }
                });
                this.series.buildKDTree(); // rebuild kdtree #13195
            }
            /* eslint-enable valid-jsdoc */
        };
        /**
         * @private
         * @mixin Highcharts.colorSeriesMixin
         */
        H.colorSeriesMixin = {
            optionalAxis: 'colorAxis',
            colorAxis: 0,
            /* eslint-disable valid-jsdoc */
            /**
             * In choropleth maps, the color is a result of the value, so this needs
             * translation too
             * @private
             * @function Highcharts.colorSeriesMixin.translateColors
             * @return {void}
             */
            translateColors: function () {
                var series = this,
                    points = this.data.length ? this.data : this.points,
                    nullColor = this.options.nullColor,
                    colorAxis = this.colorAxis,
                    colorKey = this.colorKey;
                points.forEach(function (point) {
                    var value = point.getNestedProperty(colorKey),
                        color;
                    color = point.options.color ||
                        (point.isNull || point.value === null ?
                            nullColor :
                            (colorAxis && typeof value !== 'undefined') ?
                                colorAxis.toColor(value, point) :
                                point.color || series.color);
                    if (color && point.color !== color) {
                        point.color = color;
                        if (series.options.legendType === 'point' && point.legendItem) {
                            series.chart.legend.colorizeItem(point, point.visible);
                        }
                    }
                });
            }
            /* eslint-enable valid-jsdoc */
        };

    });
    _registerModule(_modules, 'Core/Axis/ColorAxis.js', [_modules['Core/Axis/Axis.js'], _modules['Core/Chart/Chart.js'], _modules['Core/Color.js'], _modules['Core/Globals.js'], _modules['Core/Legend.js'], _modules['Mixins/LegendSymbol.js'], _modules['Core/Series/Point.js'], _modules['Core/Utilities.js']], function (Axis, Chart, Color, H, Legend, LegendSymbolMixin, Point, U) {
        /* *
         *
         *  (c) 2010-2020 Torstein Honsi
         *
         *  License: www.highcharts.com/license
         *
         *  !!!!!!! SOURCE GETS TRANSPILED BY TYPESCRIPT. EDIT TS FILE ONLY. !!!!!!!
         *
         * */
        var __extends = (this && this.__extends) || (function () {
                var extendStatics = function (d,
            b) {
                    extendStatics = Object.setPrototypeOf ||
                        ({ __proto__: [] } instanceof Array && function (d,
            b) { d.__proto__ = b; }) ||
                        function (d,
            b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
                return extendStatics(d, b);
            };
            return function (d, b) {
                extendStatics(d, b);
                function __() { this.constructor = d; }
                d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
            };
        })();
        var color = Color.parse;
        var noop = H.noop;
        var addEvent = U.addEvent,
            erase = U.erase,
            extend = U.extend,
            Fx = U.Fx,
            isNumber = U.isNumber,
            merge = U.merge,
            pick = U.pick,
            splat = U.splat;
        /**
         * Color axis types
         *
         * @typedef {"linear"|"logarithmic"} Highcharts.ColorAxisTypeValue
         */
        ''; // detach doclet above
        var Series = H.Series,
            colorPointMixin = H.colorPointMixin,
            colorSeriesMixin = H.colorSeriesMixin;
        extend(Series.prototype, colorSeriesMixin);
        extend(Point.prototype, colorPointMixin);
        Chart.prototype.collectionsWithUpdate.push('colorAxis');
        Chart.prototype.collectionsWithInit.colorAxis = [Chart.prototype.addColorAxis];
        /* eslint-disable no-invalid-this, valid-jsdoc */
        /**
         * The ColorAxis object for inclusion in gradient legends.
         *
         * @class
         * @name Highcharts.ColorAxis
         * @augments Highcharts.Axis
         *
         * @param {Highcharts.Chart} chart
         * The related chart of the color axis.
         *
         * @param {Highcharts.ColorAxisOptions} userOptions
         * The color axis options for initialization.
         */
        var ColorAxis = /** @class */ (function (_super) {
                __extends(ColorAxis, _super);
            /* *
             *
             *  Constructors
             *
             * */
            /**
             * @private
             */
            function ColorAxis(chart, userOptions) {
                var _this = _super.call(this,
                    chart,
                    userOptions) || this;
                _this.beforePadding = false; // Prevents unnecessary padding with `hc-more`
                _this.chart = void 0;
                _this.coll = 'colorAxis';
                _this.dataClasses = void 0;
                _this.legendItem = void 0;
                _this.legendItems = void 0;
                _this.name = ''; // Prevents 'undefined' in legend in IE8
                _this.options = void 0;
                _this.stops = void 0;
                _this.visible = true;
                _this.init(chart, userOptions);
                return _this;
            }
            /* *
             *
             *  Static Functions
             *
             * */
            /**
             * Build options to keep layout params on init and update.
             * @private
             */
            ColorAxis.buildOptions = function (chart, options, userOptions) {
                var legend = chart.options.legend || {},
                    horiz = userOptions.layout ?
                        userOptions.layout !== 'vertical' :
                        legend.layout !== 'vertical';
                return merge(options, {
                    side: horiz ? 2 : 1,
                    reversed: !horiz
                }, userOptions, {
                    opposite: !horiz,
                    showEmpty: false,
                    title: null,
                    visible: legend.enabled &&
                        (userOptions ? userOptions.visible !== false : true)
                });
            };
            /* *
             *
             *  Functions
             *
             * */
            /**
             * Initializes the color axis.
             *
             * @function Highcharts.ColorAxis#init
             *
             * @param {Highcharts.Chart} chart
             * The related chart of the color axis.
             *
             * @param {Highcharts.ColorAxisOptions} userOptions
             * The color axis options for initialization.
             */
            ColorAxis.prototype.init = function (chart, userOptions) {
                var axis = this;
                var options = ColorAxis.buildOptions(// Build the options
                    chart,
                    ColorAxis.defaultOptions,
                    userOptions);
                axis.coll = 'colorAxis';
                _super.prototype.init.call(this, chart, options);
                // Base init() pushes it to the xAxis array, now pop it again
                // chart[this.isXAxis ? 'xAxis' : 'yAxis'].pop();
                // Prepare data classes
                if (userOptions.dataClasses) {
                    axis.initDataClasses(userOptions);
                }
                axis.initStops();
                // Override original axis properties
                axis.horiz = !options.opposite;
                axis.zoomEnabled = false;
            };
            /**
             * @private
             */
            ColorAxis.prototype.initDataClasses = function (userOptions) {
                var axis = this;
                var chart = axis.chart,
                    dataClasses,
                    colorCounter = 0,
                    colorCount = chart.options.chart.colorCount,
                    options = axis.options,
                    len = userOptions.dataClasses.length;
                axis.dataClasses = dataClasses = [];
                axis.legendItems = [];
                userOptions.dataClasses.forEach(function (dataClass, i) {
                    var colors;
                    dataClass = merge(dataClass);
                    dataClasses.push(dataClass);
                    if (!chart.styledMode && dataClass.color) {
                        return;
                    }
                    if (options.dataClassColor === 'category') {
                        if (!chart.styledMode) {
                            colors = chart.options.colors;
                            colorCount = colors.length;
                            dataClass.color = colors[colorCounter];
                        }
                        dataClass.colorIndex = colorCounter;
                        // increase and loop back to zero
                        colorCounter++;
                        if (colorCounter === colorCount) {
                            colorCounter = 0;
                        }
                    }
                    else {
                        dataClass.color = color(options.minColor).tweenTo(color(options.maxColor), len < 2 ? 0.5 : i / (len - 1) // #3219
                        );
                    }
                });
            };
            /**
             * Returns true if the series has points at all.
             *
             * @function Highcharts.ColorAxis#hasData
             *
             * @return {boolean}
             * True, if the series has points, otherwise false.
             */
            ColorAxis.prototype.hasData = function () {
                return !!(this.tickPositions || []).length;
            };
            /**
             * Override so that ticks are not added in data class axes (#6914)
             * @private
             */
            ColorAxis.prototype.setTickPositions = function () {
                if (!this.dataClasses) {
                    return _super.prototype.setTickPositions.call(this);
                }
            };
            /**
             * @private
             */
            ColorAxis.prototype.initStops = function () {
                var axis = this;
                axis.stops = axis.options.stops || [
                    [0, axis.options.minColor],
                    [1, axis.options.maxColor]
                ];
                axis.stops.forEach(function (stop) {
                    stop.color = color(stop[1]);
                });
            };
            /**
             * Extend the setOptions method to process extreme colors and color stops.
             * @private
             */
            ColorAxis.prototype.setOptions = function (userOptions) {
                var axis = this;
                _super.prototype.setOptions.call(this, userOptions);
                axis.options.crosshair = axis.options.marker;
            };
            /**
             * @private
             */
            ColorAxis.prototype.setAxisSize = function () {
                var axis = this;
                var symbol = axis.legendSymbol;
                var chart = axis.chart;
                var legendOptions = chart.options.legend || {};
                var x,
                    y,
                    width,
                    height;
                if (symbol) {
                    this.left = x = symbol.attr('x');
                    this.top = y = symbol.attr('y');
                    this.width = width = symbol.attr('width');
                    this.height = height = symbol.attr('height');
                    this.right = chart.chartWidth - x - width;
                    this.bottom = chart.chartHeight - y - height;
                    this.len = this.horiz ? width : height;
                    this.pos = this.horiz ? x : y;
                }
                else {
                    // Fake length for disabled legend to avoid tick issues
                    // and such (#5205)
                    this.len = (this.horiz ?
                        legendOptions.symbolWidth :
                        legendOptions.symbolHeight) || ColorAxis.defaultLegendLength;
                }
            };
            /**
             * @private
             */
            ColorAxis.prototype.normalizedValue = function (value) {
                var axis = this;
                if (axis.logarithmic) {
                    value = axis.logarithmic.log2lin(value);
                }
                return 1 - ((axis.max - value) /
                    ((axis.max - axis.min) || 1));
            };
            /**
             * Translate from a value to a color.
             * @private
             */
            ColorAxis.prototype.toColor = function (value, point) {
                var axis = this;
                var dataClasses = axis.dataClasses;
                var stops = axis.stops;
                var pos,
                    from,
                    to,
                    color,
                    dataClass,
                    i;
                if (dataClasses) {
                    i = dataClasses.length;
                    while (i--) {
                        dataClass = dataClasses[i];
                        from = dataClass.from;
                        to = dataClass.to;
                        if ((typeof from === 'undefined' || value >= from) &&
                            (typeof to === 'undefined' || value <= to)) {
                            color = dataClass.color;
                            if (point) {
                                point.dataClass = i;
                                point.colorIndex = dataClass.colorIndex;
                            }
                            break;
                        }
                    }
                }
                else {
                    pos = axis.normalizedValue(value);
                    i = stops.length;
                    while (i--) {
                        if (pos > stops[i][0]) {
                            break;
                        }
                    }
                    from = stops[i] || stops[i + 1];
                    to = stops[i + 1] || from;
                    // The position within the gradient
                    pos = 1 - (to[0] - pos) / ((to[0] - from[0]) || 1);
                    color = from.color.tweenTo(to.color, pos);
                }
                return color;
            };
            /**
             * Override the getOffset method to add the whole axis groups inside the
             * legend.
             * @private
             */
            ColorAxis.prototype.getOffset = function () {
                var axis = this;
                var group = axis.legendGroup;
                var sideOffset = axis.chart.axisOffset[axis.side];
                if (group) {
                    // Hook for the getOffset method to add groups to this parent
                    // group
                    axis.axisParent = group;
                    // Call the base
                    _super.prototype.getOffset.call(this);
                    // First time only
                    if (!axis.added) {
                        axis.added = true;
                        axis.labelLeft = 0;
                        axis.labelRight = axis.width;
                    }
                    // Reset it to avoid color axis reserving space
                    axis.chart.axisOffset[axis.side] = sideOffset;
                }
            };
            /**
             * Create the color gradient.
             * @private
             */
            ColorAxis.prototype.setLegendColor = function () {
                var axis = this;
                var horiz = axis.horiz;
                var reversed = axis.reversed;
                var one = reversed ? 1 : 0;
                var zero = reversed ? 0 : 1;
                var grad = horiz ? [one, 0,
                    zero, 0] : [0,
                    zero, 0,
                    one]; // #3190
                    axis.legendColor = {
                        linearGradient: {
                            x1: grad[0],
                            y1: grad[1],
                            x2: grad[2],
                            y2: grad[3]
                        },
                        stops: axis.stops
                    };
            };
            /**
             * The color axis appears inside the legend and has its own legend symbol.
             * @private
             */
            ColorAxis.prototype.drawLegendSymbol = function (legend, item) {
                var axis = this;
                var padding = legend.padding;
                var legendOptions = legend.options;
                var horiz = axis.horiz;
                var width = pick(legendOptions.symbolWidth,
                    horiz ? ColorAxis.defaultLegendLength : 12);
                var height = pick(legendOptions.symbolHeight,
                    horiz ? 12 : ColorAxis.defaultLegendLength);
                var labelPadding = pick(legendOptions.labelPadding,
                    horiz ? 16 : 30);
                var itemDistance = pick(legendOptions.itemDistance, 10);
                this.setLegendColor();
                // Create the gradient
                item.legendSymbol = this.chart.renderer.rect(0, legend.baseline - 11, width, height).attr({
                    zIndex: 1
                }).add(item.legendGroup);
                // Set how much space this legend item takes up
                axis.legendItemWidth = width + padding + (horiz ? itemDistance : labelPadding);
                axis.legendItemHeight = height + padding + (horiz ? labelPadding : 0);
            };
            /**
             * Fool the legend.
             * @private
             */
            ColorAxis.prototype.setState = function (state) {
                this.series.forEach(function (series) {
                    series.setState(state);
                });
            };
            /**
             * @private
             */
            ColorAxis.prototype.setVisible = function () {
            };
            /**
             * @private
             */
            ColorAxis.prototype.getSeriesExtremes = function () {
                var axis = this;
                var series = axis.series;
                var colorValArray,
                    colorKey,
                    colorValIndex,
                    pointArrayMap,
                    calculatedExtremes,
                    cSeries,
                    i = series.length,
                    yData,
                    j;
                this.dataMin = Infinity;
                this.dataMax = -Infinity;
                while (i--) { // x, y, value, other
                    cSeries = series[i];
                    colorKey = cSeries.colorKey = pick(cSeries.options.colorKey, cSeries.colorKey, cSeries.pointValKey, cSeries.zoneAxis, 'y');
                    pointArrayMap = cSeries.pointArrayMap;
                    calculatedExtremes = cSeries[colorKey + 'Min'] &&
                        cSeries[colorKey + 'Max'];
                    if (cSeries[colorKey + 'Data']) {
                        colorValArray = cSeries[colorKey + 'Data'];
                    }
                    else {
                        if (!pointArrayMap) {
                            colorValArray = cSeries.yData;
                        }
                        else {
                            colorValArray = [];
                            colorValIndex = pointArrayMap.indexOf(colorKey);
                            yData = cSeries.yData;
                            if (colorValIndex >= 0 && yData) {
                                for (j = 0; j < yData.length; j++) {
                                    colorValArray.push(pick(yData[j][colorValIndex], yData[j]));
                                }
                            }
                        }
                    }
                    // If color key extremes are already calculated, use them.
                    if (calculatedExtremes) {
                        cSeries.minColorValue = cSeries[colorKey + 'Min'];
                        cSeries.maxColorValue = cSeries[colorKey + 'Max'];
                    }
                    else {
                        var cExtremes = Series.prototype.getExtremes.call(cSeries,
                            colorValArray);
                        cSeries.minColorValue = cExtremes.dataMin;
                        cSeries.maxColorValue = cExtremes.dataMax;
                    }
                    if (typeof cSeries.minColorValue !== 'undefined') {
                        this.dataMin =
                            Math.min(this.dataMin, cSeries.minColorValue);
                        this.dataMax =
                            Math.max(this.dataMax, cSeries.maxColorValue);
                    }
                    if (!calculatedExtremes) {
                        Series.prototype.applyExtremes.call(cSeries);
                    }
                }
            };
            /**
             * Internal function to draw a crosshair.
             *
             * @function Highcharts.ColorAxis#drawCrosshair
             *
             * @param {Highcharts.PointerEventObject} [e]
             *        The event arguments from the modified pointer event, extended with
             *        `chartX` and `chartY`
             *
             * @param {Highcharts.Point} [point]
             *        The Point object if the crosshair snaps to points.
             *
             * @fires Highcharts.ColorAxis#event:afterDrawCrosshair
             * @fires Highcharts.ColorAxis#event:drawCrosshair
             */
            ColorAxis.prototype.drawCrosshair = function (e, point) {
                var axis = this;
                var plotX = point && point.plotX;
                var plotY = point && point.plotY;
                var axisPos = axis.pos;
                var axisLen = axis.len;
                var crossPos;
                if (point) {
                    crossPos = axis.toPixels(point.getNestedProperty(point.series.colorKey));
                    if (crossPos < axisPos) {
                        crossPos = axisPos - 2;
                    }
                    else if (crossPos > axisPos + axisLen) {
                        crossPos = axisPos + axisLen + 2;
                    }
                    point.plotX = crossPos;
                    point.plotY = axis.len - crossPos;
                    _super.prototype.drawCrosshair.call(this, e, point);
                    point.plotX = plotX;
                    point.plotY = plotY;
                    if (axis.cross &&
                        !axis.cross.addedToColorAxis &&
                        axis.legendGroup) {
                        axis.cross
                            .addClass('highcharts-coloraxis-marker')
                            .add(axis.legendGroup);
                        axis.cross.addedToColorAxis = true;
                        if (!axis.chart.styledMode &&
                            axis.crosshair) {
                            axis.cross.attr({
                                fill: axis.crosshair.color
                            });
                        }
                    }
                }
            };
            /**
             * @private
             */
            ColorAxis.prototype.getPlotLinePath = function (options) {
                var axis = this,
                    left = axis.left,
                    pos = options.translatedValue,
                    top = axis.top;
                // crosshairs only
                return isNumber(pos) ? // pos can be 0 (#3969)
                    (axis.horiz ? [
                        ['M', pos - 4, top - 6],
                        ['L', pos + 4, top - 6],
                        ['L', pos, top],
                        ['Z']
                    ] : [
                        ['M', left, pos],
                        ['L', left - 6, pos + 6],
                        ['L', left - 6, pos - 6],
                        ['Z']
                    ]) :
                    _super.prototype.getPlotLinePath.call(this, options);
            };
            /**
             * Updates a color axis instance with a new set of options. The options are
             * merged with the existing options, so only new or altered options need to
             * be specified.
             *
             * @function Highcharts.ColorAxis#update
             *
             * @param {Highcharts.ColorAxisOptions} newOptions
             * The new options that will be merged in with existing options on the color
             * axis.
             *
             * @param {boolean} [redraw]
             * Whether to redraw the chart after the color axis is altered. If doing
             * more operations on the chart, it is a good idea to set redraw to `false`
             * and call {@link Highcharts.Chart#redraw} after.
             */
            ColorAxis.prototype.update = function (newOptions, redraw) {
                var axis = this,
                    chart = axis.chart,
                    legend = chart.legend,
                    updatedOptions = ColorAxis.buildOptions(chart, {},
                    newOptions);
                this.series.forEach(function (series) {
                    // Needed for Axis.update when choropleth colors change
                    series.isDirtyData = true;
                });
                // When updating data classes, destroy old items and make sure new
                // ones are created (#3207)
                if (newOptions.dataClasses && legend.allItems || axis.dataClasses) {
                    axis.destroyItems();
                }
                // Keep the options structure updated for export. Unlike xAxis and
                // yAxis, the colorAxis is not an array. (#3207)
                chart.options[axis.coll] =
                    merge(axis.userOptions, updatedOptions);
                _super.prototype.update.call(this, updatedOptions, redraw);
                if (axis.legendItem) {
                    axis.setLegendColor();
                    legend.colorizeItem(this, true);
                }
            };
            /**
             * Destroy color axis legend items.
             * @private
             */
            ColorAxis.prototype.destroyItems = function () {
                var axis = this;
                var chart = axis.chart;
                if (axis.legendItem) {
                    chart.legend.destroyItem(axis);
                }
                else if (axis.legendItems) {
                    axis.legendItems.forEach(function (item) {
                        chart.legend.destroyItem(item);
                    });
                }
                chart.isDirtyLegend = true;
            };
            /**
             * Removes the color axis and the related legend item.
             *
             * @function Highcharts.ColorAxis#remove
             *
             * @param {boolean} [redraw=true]
             *        Whether to redraw the chart following the remove.
             */
            ColorAxis.prototype.remove = function (redraw) {
                this.destroyItems();
                _super.prototype.remove.call(this, redraw);
            };
            /**
             * Get the legend item symbols for data classes.
             * @private
             */
            ColorAxis.prototype.getDataClassLegendSymbols = function () {
                var axis = this;
                var chart = axis.chart;
                var legendItems = axis.legendItems;
                var legendOptions = chart.options.legend;
                var valueDecimals = legendOptions.valueDecimals;
                var valueSuffix = legendOptions.valueSuffix || '';
                var name;
                if (!legendItems.length) {
                    axis.dataClasses.forEach(function (dataClass, i) {
                        var vis = true,
                            from = dataClass.from,
                            to = dataClass.to;
                        var numberFormatter = chart.numberFormatter;
                        // Assemble the default name. This can be overridden
                        // by legend.options.labelFormatter
                        name = '';
                        if (typeof from === 'undefined') {
                            name = '< ';
                        }
                        else if (typeof to === 'undefined') {
                            name = '> ';
                        }
                        if (typeof from !== 'undefined') {
                            name += numberFormatter(from, valueDecimals) + valueSuffix;
                        }
                        if (typeof from !== 'undefined' && typeof to !== 'undefined') {
                            name += ' - ';
                        }
                        if (typeof to !== 'undefined') {
                            name += numberFormatter(to, valueDecimals) + valueSuffix;
                        }
                        // Add a mock object to the legend items
                        legendItems.push(extend({
                            chart: chart,
                            name: name,
                            options: {},
                            drawLegendSymbol: LegendSymbolMixin.drawRectangle,
                            visible: true,
                            setState: noop,
                            isDataClass: true,
                            setVisible: function () {
                                vis = axis.visible = !vis;
                                axis.series.forEach(function (series) {
                                    series.points.forEach(function (point) {
                                        if (point.dataClass === i) {
                                            point.setVisible(vis);
                                        }
                                    });
                                });
                                chart.legend.colorizeItem(this, vis);
                            }
                        }, dataClass));
                    });
                }
                return legendItems;
            };
            /* *
             *
             *  Static Functions
             *
             * */
            ColorAxis.defaultLegendLength = 200;
            /**
             * A color axis for series. Visually, the color
             * axis will appear as a gradient or as separate items inside the
             * legend, depending on whether the axis is scalar or based on data
             * classes.
             *
             * For supported color formats, see the
             * [docs article about colors](https://www.highcharts.com/docs/chart-design-and-style/colors).
             *
             * A scalar color axis is represented by a gradient. The colors either
             * range between the [minColor](#colorAxis.minColor) and the
             * [maxColor](#colorAxis.maxColor), or for more fine grained control the
             * colors can be defined in [stops](#colorAxis.stops). Often times, the
             * color axis needs to be adjusted to get the right color spread for the
             * data. In addition to stops, consider using a logarithmic
             * [axis type](#colorAxis.type), or setting [min](#colorAxis.min) and
             * [max](#colorAxis.max) to avoid the colors being determined by
             * outliers.
             *
             * When [dataClasses](#colorAxis.dataClasses) are used, the ranges are
             * subdivided into separate classes like categories based on their
             * values. This can be used for ranges between two values, but also for
             * a true category. However, when your data is categorized, it may be as
             * convenient to add each category to a separate series.
             *
             * Color axis does not work with: `sankey`, `sunburst`, `dependencywheel`,
             * `networkgraph`, `wordcloud`, `venn`, `gauge` and `solidgauge` series
             * types.
             *
             * Since v7.2.0 `colorAxis` can also be an array of options objects.
             *
             * See [the Axis object](/class-reference/Highcharts.Axis) for
             * programmatic access to the axis.
             *
             * @sample       {highcharts} highcharts/coloraxis/custom-color-key
             *               Column chart with color axis
             * @sample       {highcharts} highcharts/coloraxis/horizontal-layout
             *               Horizontal layout
             * @sample       {highmaps} maps/coloraxis/dataclasscolor
             *               With data classes
             * @sample       {highmaps} maps/coloraxis/mincolor-maxcolor
             *               Min color and max color
             *
             * @extends      xAxis
             * @excluding    alignTicks, allowDecimals, alternateGridColor, breaks,
             *               categories, crosshair, dateTimeLabelFormats, height, left,
             *               lineWidth, linkedTo, maxZoom, minRange, minTickInterval,
             *               offset, opposite, pane, plotBands, plotLines,
             *               reversedStacks, showEmpty, title, top, width, zoomEnabled
             * @product      highcharts highstock highmaps
             * @type         {*|Array<*>}
             * @optionparent colorAxis
             * @ignore
             */
            ColorAxis.defaultOptions = {
                /**
                 * Whether to allow decimals on the color axis.
                 * @type      {boolean}
                 * @default   true
                 * @product   highcharts highstock highmaps
                 * @apioption colorAxis.allowDecimals
                 */
                /**
                 * Determines how to set each data class' color if no individual
                 * color is set. The default value, `tween`, computes intermediate
                 * colors between `minColor` and `maxColor`. The other possible
                 * value, `category`, pulls colors from the global or chart specific
                 * [colors](#colors) array.
                 *
                 * @sample {highmaps} maps/coloraxis/dataclasscolor/
                 *         Category colors
                 *
                 * @type       {string}
                 * @default    tween
                 * @product    highcharts highstock highmaps
                 * @validvalue ["tween", "category"]
                 * @apioption  colorAxis.dataClassColor
                 */
                /**
                 * An array of data classes or ranges for the choropleth map. If
                 * none given, the color axis is scalar and values are distributed
                 * as a gradient between the minimum and maximum colors.
                 *
                 * @sample {highmaps} maps/demo/data-class-ranges/
                 *         Multiple ranges
                 *
                 * @sample {highmaps} maps/demo/data-class-two-ranges/
                 *         Two ranges
                 *
                 * @type      {Array<*>}
                 * @product   highcharts highstock highmaps
                 * @apioption colorAxis.dataClasses
                 */
                /**
                 * The layout of the color axis. Can be `'horizontal'` or `'vertical'`.
                 * If none given, the color axis has the same layout as the legend.
                 *
                 * @sample highcharts/coloraxis/horizontal-layout/
                 *         Horizontal color axis layout with vertical legend
                 *
                 * @type      {string|undefined}
                 * @since     7.2.0
                 * @product   highcharts highstock highmaps
                 * @apioption colorAxis.layout
                 */
                /**
                 * The color of each data class. If not set, the color is pulled
                 * from the global or chart-specific [colors](#colors) array. In
                 * styled mode, this option is ignored. Instead, use colors defined
                 * in CSS.
                 *
                 * @sample {highmaps} maps/demo/data-class-two-ranges/
                 *         Explicit colors
                 *
                 * @type      {Highcharts.ColorString|Highcharts.GradientColorObject|Highcharts.PatternObject}
                 * @product   highcharts highstock highmaps
                 * @apioption colorAxis.dataClasses.color
                 */
                /**
                 * The start of the value range that the data class represents,
                 * relating to the point value.
                 *
                 * The range of each `dataClass` is closed in both ends, but can be
                 * overridden by the next `dataClass`.
                 *
                 * @type      {number}
                 * @product   highcharts highstock highmaps
                 * @apioption colorAxis.dataClasses.from
                 */
                /**
                 * The name of the data class as it appears in the legend.
                 * If no name is given, it is automatically created based on the
                 * `from` and `to` values. For full programmatic control,
                 * [legend.labelFormatter](#legend.labelFormatter) can be used.
                 * In the formatter, `this.from` and `this.to` can be accessed.
                 *
                 * @sample {highmaps} maps/coloraxis/dataclasses-name/
                 *         Named data classes
                 *
                 * @sample {highmaps} maps/coloraxis/dataclasses-labelformatter/
                 *         Formatted data classes
                 *
                 * @type      {string}
                 * @product   highcharts highstock highmaps
                 * @apioption colorAxis.dataClasses.name
                 */
                /**
                 * The end of the value range that the data class represents,
                 * relating to the point value.
                 *
                 * The range of each `dataClass` is closed in both ends, but can be
                 * overridden by the next `dataClass`.
                 *
                 * @type      {number}
                 * @product   highcharts highstock highmaps
                 * @apioption colorAxis.dataClasses.to
                 */
                /** @ignore-option */
                lineWidth: 0,
                /**
                 * Padding of the min value relative to the length of the axis. A
                 * padding of 0.05 will make a 100px axis 5px longer.
                 *
                 * @product highcharts highstock highmaps
                 */
                minPadding: 0,
                /**
                 * The maximum value of the axis in terms of map point values. If
                 * `null`, the max value is automatically calculated. If the
                 * `endOnTick` option is true, the max value might be rounded up.
                 *
                 * @sample {highmaps} maps/coloraxis/gridlines/
                 *         Explicit min and max to reduce the effect of outliers
                 *
                 * @type      {number}
                 * @product   highcharts highstock highmaps
                 * @apioption colorAxis.max
                 */
                /**
                 * The minimum value of the axis in terms of map point values. If
                 * `null`, the min value is automatically calculated. If the
                 * `startOnTick` option is true, the min value might be rounded
                 * down.
                 *
                 * @sample {highmaps} maps/coloraxis/gridlines/
                 *         Explicit min and max to reduce the effect of outliers
                 *
                 * @type      {number}
                 * @product   highcharts highstock highmaps
                 * @apioption colorAxis.min
                 */
                /**
                 * Padding of the max value relative to the length of the axis. A
                 * padding of 0.05 will make a 100px axis 5px longer.
                 *
                 * @product highcharts highstock highmaps
                 */
                maxPadding: 0,
                /**
                 * Color of the grid lines extending from the axis across the
                 * gradient.
                 *
                 * @sample {highmaps} maps/coloraxis/gridlines/
                 *         Grid lines demonstrated
                 *
                 * @type      {Highcharts.ColorString|Highcharts.GradientColorObject|Highcharts.PatternObject}
                 * @default   #e6e6e6
                 * @product   highcharts highstock highmaps
                 * @apioption colorAxis.gridLineColor
                 */
                /**
                 * The width of the grid lines extending from the axis across the
                 * gradient of a scalar color axis.
                 *
                 * @sample {highmaps} maps/coloraxis/gridlines/
                 *         Grid lines demonstrated
                 *
                 * @product highcharts highstock highmaps
                 */
                gridLineWidth: 1,
                /**
                 * The interval of the tick marks in axis units. When `null`, the
                 * tick interval is computed to approximately follow the
                 * `tickPixelInterval`.
                 *
                 * @type      {number}
                 * @product   highcharts highstock highmaps
                 * @apioption colorAxis.tickInterval
                 */
                /**
                 * If [tickInterval](#colorAxis.tickInterval) is `null` this option
                 * sets the approximate pixel interval of the tick marks.
                 *
                 * @product highcharts highstock highmaps
                 */
                tickPixelInterval: 72,
                /**
                 * Whether to force the axis to start on a tick. Use this option
                 * with the `maxPadding` option to control the axis start.
                 *
                 * @product highcharts highstock highmaps
                 */
                startOnTick: true,
                /**
                 * Whether to force the axis to end on a tick. Use this option with
                 * the [maxPadding](#colorAxis.maxPadding) option to control the
                 * axis end.
                 *
                 * @product highcharts highstock highmaps
                 */
                endOnTick: true,
                /** @ignore */
                offset: 0,
                /**
                 * The triangular marker on a scalar color axis that points to the
                 * value of the hovered area. To disable the marker, set
                 * `marker: null`.
                 *
                 * @sample {highmaps} maps/coloraxis/marker/
                 *         Black marker
                 *
                 * @declare Highcharts.PointMarkerOptionsObject
                 * @product highcharts highstock highmaps
                 */
                marker: {
                    /**
                     * Animation for the marker as it moves between values. Set to
                     * `false` to disable animation. Defaults to `{ duration: 50 }`.
                     *
                     * @type    {boolean|Partial<Highcharts.AnimationOptionsObject>}
                     * @product highcharts highstock highmaps
                     */
                    animation: {
                        /** @internal */
                        duration: 50
                    },
                    /** @internal */
                    width: 0.01,
                    /**
                     * The color of the marker.
                     *
                     * @type    {Highcharts.ColorString|Highcharts.GradientColorObject|Highcharts.PatternObject}
                     * @product highcharts highstock highmaps
                     */
                    color: '#999999'
                },
                /**
                 * The axis labels show the number for each tick.
                 *
                 * For more live examples on label options, see [xAxis.labels in the
                 * Highcharts API.](/highcharts#xAxis.labels)
                 *
                 * @extends xAxis.labels
                 * @product highcharts highstock highmaps
                 */
                labels: {
                    /**
                     * How to handle overflowing labels on horizontal color axis. If set
                     * to `"allow"`, it will not be aligned at all. By default it
                     * `"justify"` labels inside the chart area. If there is room to
                     * move it, it will be aligned to the edge, else it will be removed.
                     *
                     * @validvalue ["allow", "justify"]
                     * @product    highcharts highstock highmaps
                     */
                    overflow: 'justify',
                    rotation: 0
                },
                /**
                 * The color to represent the minimum of the color axis. Unless
                 * [dataClasses](#colorAxis.dataClasses) or
                 * [stops](#colorAxis.stops) are set, the gradient starts at this
                 * value.
                 *
                 * If dataClasses are set, the color is based on minColor and
                 * maxColor unless a color is set for each data class, or the
                 * [dataClassColor](#colorAxis.dataClassColor) is set.
                 *
                 * @sample {highmaps} maps/coloraxis/mincolor-maxcolor/
                 *         Min and max colors on scalar (gradient) axis
                 * @sample {highmaps} maps/coloraxis/mincolor-maxcolor-dataclasses/
                 *         On data classes
                 *
                 * @type    {Highcharts.ColorString|Highcharts.GradientColorObject|Highcharts.PatternObject}
                 * @product highcharts highstock highmaps
                 */
                minColor: '#e6ebf5',
                /**
                 * The color to represent the maximum of the color axis. Unless
                 * [dataClasses](#colorAxis.dataClasses) or
                 * [stops](#colorAxis.stops) are set, the gradient ends at this
                 * value.
                 *
                 * If dataClasses are set, the color is based on minColor and
                 * maxColor unless a color is set for each data class, or the
                 * [dataClassColor](#colorAxis.dataClassColor) is set.
                 *
                 * @sample {highmaps} maps/coloraxis/mincolor-maxcolor/
                 *         Min and max colors on scalar (gradient) axis
                 * @sample {highmaps} maps/coloraxis/mincolor-maxcolor-dataclasses/
                 *         On data classes
                 *
                 * @type    {Highcharts.ColorString|Highcharts.GradientColorObject|Highcharts.PatternObject}
                 * @product highcharts highstock highmaps
                 */
                maxColor: '#003399',
                /**
                 * Color stops for the gradient of a scalar color axis. Use this in
                 * cases where a linear gradient between a `minColor` and `maxColor`
                 * is not sufficient. The stops is an array of tuples, where the
                 * first item is a float between 0 and 1 assigning the relative
                 * position in the gradient, and the second item is the color.
                 *
                 * @sample {highmaps} maps/demo/heatmap/
                 *         Heatmap with three color stops
                 *
                 * @type      {Array<Array<number,Highcharts.ColorString>>}
                 * @product   highcharts highstock highmaps
                 * @apioption colorAxis.stops
                 */
                /**
                 * The pixel length of the main tick marks on the color axis.
                 */
                tickLength: 5,
                /**
                 * The type of interpolation to use for the color axis. Can be
                 * `linear` or `logarithmic`.
                 *
                 * @sample highcharts/coloraxis/logarithmic-with-emulate-negative-values/
                 *         Logarithmic color axis with extension to emulate negative
                 *         values
                 *
                 * @type      {Highcharts.ColorAxisTypeValue}
                 * @default   linear
                 * @product   highcharts highstock highmaps
                 * @apioption colorAxis.type
                 */
                /**
                 * Whether to reverse the axis so that the highest number is closest
                 * to the origin. Defaults to `false` in a horizontal legend and
                 * `true` in a vertical legend, where the smallest value starts on
                 * top.
                 *
                 * @type      {boolean}
                 * @product   highcharts highstock highmaps
                 * @apioption colorAxis.reversed
                 */
                /**
                 * @product   highcharts highstock highmaps
                 * @excluding afterBreaks, pointBreak, pointInBreak
                 * @apioption colorAxis.events
                 */
                /**
                 * Fires when the legend item belonging to the colorAxis is clicked.
                 * One parameter, `event`, is passed to the function.
                 *
                 * @type      {Function}
                 * @product   highcharts highstock highmaps
                 * @apioption colorAxis.events.legendItemClick
                 */
                /**
                 * Whether to display the colorAxis in the legend.
                 *
                 * @sample highcharts/coloraxis/hidden-coloraxis-with-3d-chart/
                 *         Hidden color axis with 3d chart
                 *
                 * @see [heatmap.showInLegend](#series.heatmap.showInLegend)
                 *
                 * @since   4.2.7
                 * @product highcharts highstock highmaps
                 */
                showInLegend: true
            };
            /**
             * @private
             */
            ColorAxis.keepProps = [
                'legendGroup',
                'legendItemHeight',
                'legendItemWidth',
                'legendItem',
                'legendSymbol'
            ];
            return ColorAxis;
        }(Axis));
        // Properties to preserve after destroy, for Axis.update (#5881, #6025).
        Array.prototype.push.apply(Axis.keepProps, ColorAxis.keepProps);
        H.ColorAxis = ColorAxis;
        /**
         * Handle animation of the color attributes directly
         *
         * @private
         * @function Highcharts.Fx#fillSetter
         */ /**
        * Handle animation of the color attributes directly
        *
        * @private
        * @function Highcharts.Fx#strokeSetter
        */
        ['fill', 'stroke'].forEach(function (prop) {
            Fx.prototype[prop + 'Setter'] = function () {
                this.elem.attr(prop, color(this.start).tweenTo(color(this.end), this.pos), null, true);
            };
        });
        // Extend the chart getAxes method to also get the color axis
        addEvent(Chart, 'afterGetAxes', function () {
            var chart = this,
                options = chart.options;
            this.colorAxis = [];
            if (options.colorAxis) {
                options.colorAxis = splat(options.colorAxis);
                options.colorAxis.forEach(function (axisOptions, i) {
                    axisOptions.index = i;
                    new ColorAxis(chart, axisOptions); // eslint-disable-line no-new
                });
            }
        });
        // Add colorAxis to series axisTypes
        addEvent(Series, 'bindAxes', function () {
            var axisTypes = this.axisTypes;
            if (!axisTypes) {
                this.axisTypes = ['colorAxis'];
            }
            else if (axisTypes.indexOf('colorAxis') === -1) {
                axisTypes.push('colorAxis');
            }
        });
        // Add the color axis. This also removes the axis' own series to prevent
        // them from showing up individually.
        addEvent(Legend, 'afterGetAllItems', function (e) {
            var colorAxisItems = [],
                colorAxes = this.chart.colorAxis || [],
                options,
                i;
            colorAxes.forEach(function (colorAxis) {
                options = colorAxis.options;
                if (options && options.showInLegend) {
                    // Data classes
                    if (options.dataClasses && options.visible) {
                        colorAxisItems = colorAxisItems.concat(colorAxis.getDataClassLegendSymbols());
                        // Gradient legend
                    }
                    else if (options.visible) {
                        // Add this axis on top
                        colorAxisItems.push(colorAxis);
                    }
                    // If dataClasses are defined or showInLegend option is not set to
                    // true, do not add color axis' series to legend.
                    colorAxis.series.forEach(function (series) {
                        if (!series.options.showInLegend || options.dataClasses) {
                            if (series.options.legendType === 'point') {
                                series.points.forEach(function (point) {
                                    erase(e.allItems, point);
                                });
                            }
                            else {
                                erase(e.allItems, series);
                            }
                        }
                    });
                }
            });
            i = colorAxisItems.length;
            while (i--) {
                e.allItems.unshift(colorAxisItems[i]);
            }
        });
        addEvent(Legend, 'afterColorizeItem', function (e) {
            if (e.visible && e.item.legendColor) {
                e.item.legendSymbol.attr({
                    fill: e.item.legendColor
                });
            }
        });
        // Updates in the legend need to be reflected in the color axis (6888)
        addEvent(Legend, 'afterUpdate', function () {
            var colorAxes = this.chart.colorAxis;
            if (colorAxes) {
                colorAxes.forEach(function (colorAxis) {
                    colorAxis.update({}, arguments[2]);
                });
            }
        });
        // Calculate and set colors for points
        addEvent(Series, 'afterTranslate', function () {
            if (this.chart.colorAxis &&
                this.chart.colorAxis.length ||
                this.colorAttribs) {
                this.translateColors();
            }
        });

        return ColorAxis;
    });
    _registerModule(_modules, 'Mixins/ColorMapSeries.js', [_modules['Core/Globals.js'], _modules['Core/Series/Point.js'], _modules['Core/Utilities.js']], function (H, Point, U) {
        /* *
         *
         *  (c) 2010-2020 Torstein Honsi
         *
         *  License: www.highcharts.com/license
         *
         *  !!!!!!! SOURCE GETS TRANSPILED BY TYPESCRIPT. EDIT TS FILE ONLY. !!!!!!!
         *
         * */
        var defined = U.defined;
        var noop = H.noop,
            seriesTypes = H.seriesTypes;
        /**
         * Mixin for maps and heatmaps
         *
         * @private
         * @mixin Highcharts.colorMapPointMixin
         */
        H.colorMapPointMixin = {
            dataLabelOnNull: true,
            /* eslint-disable valid-jsdoc */
            /**
             * Color points have a value option that determines whether or not it is
             * a null point
             * @private
             * @function Highcharts.colorMapPointMixin.isValid
             * @return {boolean}
             */
            isValid: function () {
                // undefined is allowed
                return (this.value !== null &&
                    this.value !== Infinity &&
                    this.value !== -Infinity);
            },
            /**
             * @private
             * @function Highcharts.colorMapPointMixin.setState
             * @param {string} state
             * @return {void}
             */
            setState: function (state) {
                Point.prototype.setState.call(this, state);
                if (this.graphic) {
                    this.graphic.attr({
                        zIndex: state === 'hover' ? 1 : 0
                    });
                }
            }
            /* eslint-enable valid-jsdoc */
        };
        /**
         * @private
         * @mixin Highcharts.colorMapSeriesMixin
         */
        H.colorMapSeriesMixin = {
            pointArrayMap: ['value'],
            axisTypes: ['xAxis', 'yAxis', 'colorAxis'],
            trackerGroups: ['group', 'markerGroup', 'dataLabelsGroup'],
            getSymbol: noop,
            parallelArrays: ['x', 'y', 'value'],
            colorKey: 'value',
            pointAttribs: seriesTypes.column.prototype.pointAttribs,
            /* eslint-disable valid-jsdoc */
            /**
             * Get the color attibutes to apply on the graphic
             * @private
             * @function Highcharts.colorMapSeriesMixin.colorAttribs
             * @param {Highcharts.Point} point
             * @return {Highcharts.SVGAttributes}
             */
            colorAttribs: function (point) {
                var ret = {};
                if (defined(point.color)) {
                    ret[this.colorProp || 'fill'] = point.color;
                }
                return ret;
            }
        };

    });
    _registerModule(_modules, 'Series/HeatmapSeries.js', [_modules['Core/Globals.js'], _modules['Mixins/LegendSymbol.js'], _modules['Core/Renderer/SVG/SVGRenderer.js'], _modules['Core/Utilities.js']], function (H, LegendSymbolMixin, SVGRenderer, U) {
        /* *
         *
         *  (c) 2010-2020 Torstein Honsi
         *
         *  License: www.highcharts.com/license
         *
         *  !!!!!!! SOURCE GETS TRANSPILED BY TYPESCRIPT. EDIT TS FILE ONLY. !!!!!!!
         *
         * */
        var clamp = U.clamp,
            extend = U.extend,
            fireEvent = U.fireEvent,
            isNumber = U.isNumber,
            merge = U.merge,
            pick = U.pick,
            seriesType = U.seriesType;
        /* *
         * @interface Highcharts.PointOptionsObject in parts/Point.ts
         */ /**
        * Heatmap series only. Point padding for a single point.
        * @name Highcharts.PointOptionsObject#pointPadding
        * @type {number|undefined}
        */ /**
        * Heatmap series only. The value of the point, resulting in a color controled
        * by options as set in the colorAxis configuration.
        * @name Highcharts.PointOptionsObject#value
        * @type {number|null|undefined}
        */
        ''; // detach doclets above
        var colorMapPointMixin = H.colorMapPointMixin,
            colorMapSeriesMixin = H.colorMapSeriesMixin,
            noop = H.noop,
            Series = H.Series,
            seriesTypes = H.seriesTypes,
            symbols = SVGRenderer.prototype.symbols;
        /**
         * @private
         * @class
         * @name Highcharts.seriesTypes.heatmap
         *
         * @augments Highcharts.Series
         */
        seriesType('heatmap', 'scatter', 
        /**
         * A heatmap is a graphical representation of data where the individual
         * values contained in a matrix are represented as colors.
         *
         * @productdesc {highcharts}
         * Requires `modules/heatmap`.
         *
         * @sample highcharts/demo/heatmap/
         *         Simple heatmap
         * @sample highcharts/demo/heatmap-canvas/
         *         Heavy heatmap
         *
         * @extends      plotOptions.scatter
         * @excluding    animationLimit, connectEnds, connectNulls, cropThreshold,
         *               dashStyle, findNearestPointBy, getExtremesFromAll, jitter,
         *               linecap, lineWidth, pointInterval, pointIntervalUnit,
         *               pointRange, pointStart, shadow, softThreshold, stacking,
         *               step, threshold, cluster
         * @product      highcharts highmaps
         * @optionparent plotOptions.heatmap
         */
        {
            /**
             * Animation is disabled by default on the heatmap series.
             */
            animation: false,
            /**
             * The border width for each heat map item.
             */
            borderWidth: 0,
            /**
             * Padding between the points in the heatmap.
             *
             * @type      {number}
             * @default   0
             * @since     6.0
             * @apioption plotOptions.heatmap.pointPadding
             */
            /**
             * @default   value
             * @apioption plotOptions.heatmap.colorKey
             */
            /**
             * The main color of the series. In heat maps this color is rarely used,
             * as we mostly use the color to denote the value of each point. Unless
             * options are set in the [colorAxis](#colorAxis), the default value
             * is pulled from the [options.colors](#colors) array.
             *
             * @type      {Highcharts.ColorString|Highcharts.GradientColorObject|Highcharts.PatternObject}
             * @since     4.0
             * @product   highcharts
             * @apioption plotOptions.heatmap.color
             */
            /**
             * The column size - how many X axis units each column in the heatmap
             * should span.
             *
             * @sample {highcharts} maps/demo/heatmap/
             *         One day
             * @sample {highmaps} maps/demo/heatmap/
             *         One day
             *
             * @type      {number}
             * @default   1
             * @since     4.0
             * @product   highcharts highmaps
             * @apioption plotOptions.heatmap.colsize
             */
            /**
             * The row size - how many Y axis units each heatmap row should span.
             *
             * @sample {highcharts} maps/demo/heatmap/
             *         1 by default
             * @sample {highmaps} maps/demo/heatmap/
             *         1 by default
             *
             * @type      {number}
             * @default   1
             * @since     4.0
             * @product   highcharts highmaps
             * @apioption plotOptions.heatmap.rowsize
             */
            /**
             * The color applied to null points. In styled mode, a general CSS class
             * is applied instead.
             *
             * @type {Highcharts.ColorString|Highcharts.GradientColorObject|Highcharts.PatternObject}
             */
            nullColor: '#f7f7f7',
            dataLabels: {
                formatter: function () {
                    return this.point.value;
                },
                inside: true,
                verticalAlign: 'middle',
                crop: false,
                overflow: false,
                padding: 0 // #3837
            },
            /**
             * @excluding radius, enabledThreshold
             * @since     8.1
             */
            marker: {
                /**
                 * A predefined shape or symbol for the marker. When undefined, the
                 * symbol is pulled from options.symbols. Other possible values are
                 * `'circle'`, `'square'`,`'diamond'`, `'triangle'`,
                 * `'triangle-down'`, `'rect'`, and `'ellipse'`.
                 *
                 * Additionally, the URL to a graphic can be given on this form:
                 * `'url(graphic.png)'`. Note that for the image to be applied to
                 * exported charts, its URL needs to be accessible by the export
                 * server.
                 *
                 * Custom callbacks for symbol path generation can also be added to
                 * `Highcharts.SVGRenderer.prototype.symbols`. The callback is then
                 * used by its method name, as shown in the demo.
                 *
                 * @sample {highcharts} highcharts/plotoptions/series-marker-symbol/
                 *         Predefined, graphic and custom markers
                 * @sample {highstock} highcharts/plotoptions/series-marker-symbol/
                 *         Predefined, graphic and custom markers
                 */
                symbol: 'rect',
                /** @ignore-option */
                radius: 0,
                lineColor: void 0,
                states: {
                    /**
                     * @excluding radius, radiusPlus
                     */
                    hover: {
                        /**
                         * Set the marker's fixed width on hover state.
                         *
                         * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-width
                         *         70px fixed marker's width and height on hover
                         *
                         * @type      {number|undefined}
                         * @default   undefined
                         * @product   highcharts highmaps
                         * @apioption plotOptions.heatmap.marker.states.hover.width
                         */
                        /**
                         * Set the marker's fixed height on hover state.
                         *
                         * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-width
                         *         70px fixed marker's width and height on hover
                         *
                         * @type      {number|undefined}
                         * @default   undefined
                         * @product   highcharts highmaps
                         * @apioption plotOptions.heatmap.marker.states.hover.height
                         */
                        /**
                         * The number of pixels to increase the width of the
                         * selected point.
                         *
                         * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-widthplus
                         *         20px greater width and height on hover
                         *
                         * @type      {number|undefined}
                         * @default   undefined
                         * @product   highcharts highmaps
                         * @apioption plotOptions.heatmap.marker.states.hover.widthPlus
                         */
                        /**
                         * The number of pixels to increase the height of the
                         * selected point.
                         *
                         * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-widthplus
                        *          20px greater width and height on hover
                         *
                         * @type      {number|undefined}
                         * @default   undefined
                         * @product   highcharts highmaps
                         * @apioption plotOptions.heatmap.marker.states.hover.heightPlus
                         */
                        /**
                         * The additional line width for a hovered point.
                         *
                         * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-linewidthplus
                         *         5 pixels wider lineWidth on hover
                         * @sample {highmaps} maps/plotoptions/heatmap-marker-states-hover-linewidthplus
                         *         5 pixels wider lineWidth on hover
                         */
                        lineWidthPlus: 0
                    },
                    /**
                     * @excluding radius
                     */
                    select: {
                    /**
                     * Set the marker's fixed width on select state.
                     *
                     * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-width
                     *         70px fixed marker's width and height on hover
                     *
                     * @type      {number|undefined}
                     * @default   undefined
                     * @product   highcharts highmaps
                     * @apioption plotOptions.heatmap.marker.states.select.width
                     */
                    /**
                     * Set the marker's fixed height on select state.
                     *
                     * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-width
                     *         70px fixed marker's width and height on hover
                     *
                     * @type      {number|undefined}
                     * @default   undefined
                     * @product   highcharts highmaps
                     * @apioption plotOptions.heatmap.marker.states.select.height
                     */
                    /**
                     * The number of pixels to increase the width of the
                     * selected point.
                     *
                     * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-widthplus
                     *         20px greater width and height on hover
                     *
                     * @type      {number|undefined}
                     * @default   undefined
                     * @product   highcharts highmaps
                     * @apioption plotOptions.heatmap.marker.states.select.widthPlus
                     */
                    /**
                     * The number of pixels to increase the height of the
                     * selected point.
                     *
                     * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-widthplus
                     *         20px greater width and height on hover
                     *
                     * @type      {number|undefined}
                     * @default   undefined
                     * @product   highcharts highmaps
                     * @apioption plotOptions.heatmap.marker.states.select.heightPlus
                     */
                    }
                }
            },
            clip: true,
            /** @ignore-option */
            pointRange: null,
            tooltip: {
                pointFormat: '{point.x}, {point.y}: {point.value}<br/>'
            },
            states: {
                hover: {
                    /** @ignore-option */
                    halo: false,
                    /**
                     * How much to brighten the point on interaction. Requires the
                     * main color to be defined in hex or rgb(a) format.
                     *
                     * In styled mode, the hover brightening is by default replaced
                     * with a fill-opacity set in the `.highcharts-point:hover`
                     * rule.
                     */
                    brightness: 0.2
                }
            }
        }, merge(colorMapSeriesMixin, {
            pointArrayMap: ['y', 'value'],
            hasPointSpecificOptions: true,
            getExtremesFromAll: true,
            directTouch: true,
            /* eslint-disable valid-jsdoc */
            /**
             * Override the init method to add point ranges on both axes.
             *
             * @private
             * @function Highcharts.seriesTypes.heatmap#init
             * @return {void}
             */
            init: function () {
                var options;
                Series.prototype.init.apply(this, arguments);
                options = this.options;
                // #3758, prevent resetting in setData
                options.pointRange = pick(options.pointRange, options.colsize || 1);
                // general point range
                this.yAxis.axisPointRange = options.rowsize || 1;
                // Bind new symbol names
                extend(symbols, {
                    ellipse: symbols.circle,
                    rect: symbols.square
                });
            },
            getSymbol: Series.prototype.getSymbol,
            /**
             * @private
             * @function Highcharts.seriesTypes.heatmap#setClip
             * @return {void}
             */
            setClip: function (animation) {
                var series = this,
                    chart = series.chart;
                Series.prototype.setClip.apply(series, arguments);
                if (series.options.clip !== false || animation) {
                    series.markerGroup
                        .clip((animation || series.clipBox) && series.sharedClipKey ?
                        chart[series.sharedClipKey] :
                        chart.clipRect);
                }
            },
            /**
             * @private
             * @function Highcharts.seriesTypes.heatmap#translate
             * @return {void}
             */
            translate: function () {
                var series = this, options = series.options, symbol = options.marker && options.marker.symbol || '', shape = symbols[symbol] ? symbol : 'rect', options = series.options, hasRegularShape = ['circle', 'square'].indexOf(shape) !== -1;
                series.generatePoints();
                series.points.forEach(function (point) {
                    var pointAttr,
                        sizeDiff,
                        hasImage,
                        cellAttr = point.getCellAttributes(),
                        shapeArgs = {
                            x: Math.min(cellAttr.x1,
                        cellAttr.x2),
                            y: Math.min(cellAttr.y1,
                        cellAttr.y2),
                            width: Math.max(Math.abs(cellAttr.x2 - cellAttr.x1), 0),
                            height: Math.max(Math.abs(cellAttr.y2 - cellAttr.y1), 0)
                        };
                    hasImage = point.hasImage =
                        (point.marker && point.marker.symbol || symbol || '')
                            .indexOf('url') === 0;
                    // If marker shape is regular (symetric), find shorter
                    // cell's side.
                    if (hasRegularShape) {
                        sizeDiff = Math.abs(shapeArgs.width - shapeArgs.height);
                        shapeArgs.x = Math.min(cellAttr.x1, cellAttr.x2) +
                            (shapeArgs.width < shapeArgs.height ? 0 : sizeDiff / 2);
                        shapeArgs.y = Math.min(cellAttr.y1, cellAttr.y2) +
                            (shapeArgs.width < shapeArgs.height ? sizeDiff / 2 : 0);
                        shapeArgs.width = shapeArgs.height =
                            Math.min(shapeArgs.width, shapeArgs.height);
                    }
                    pointAttr = {
                        plotX: (cellAttr.x1 + cellAttr.x2) / 2,
                        plotY: (cellAttr.y1 + cellAttr.y2) / 2,
                        clientX: (cellAttr.x1 + cellAttr.x2) / 2,
                        shapeType: 'path',
                        shapeArgs: merge(true, shapeArgs, {
                            d: symbols[shape](shapeArgs.x, shapeArgs.y, shapeArgs.width, shapeArgs.height)
                        })
                    };
                    if (hasImage) {
                        point.marker = {
                            width: shapeArgs.width,
                            height: shapeArgs.height
                        };
                    }
                    extend(point, pointAttr);
                });
                fireEvent(series, 'afterTranslate');
            },
            /**
             * @private
             * @function Highcharts.seriesTypes.heatmap#pointAttribs
             * @param {Highcharts.HeatmapPoint} point
             * @param {string} state
             * @return {Highcharts.SVGAttributes}
             */
            pointAttribs: function (point, state) {
                var series = this,
                    attr = Series.prototype.pointAttribs
                        .call(series,
                    point,
                    state),
                    seriesOptions = series.options || {},
                    plotOptions = series.chart.options.plotOptions || {},
                    seriesPlotOptions = plotOptions.series || {},
                    heatmapPlotOptions = plotOptions.heatmap || {},
                    stateOptions,
                    brightness, 
                    // Get old properties in order to keep backward compatibility
                    borderColor = seriesOptions.borderColor ||
                        heatmapPlotOptions.borderColor ||
                        seriesPlotOptions.borderColor,
                    borderWidth = seriesOptions.borderWidth ||
                        heatmapPlotOptions.borderWidth ||
                        seriesPlotOptions.borderWidth ||
                        attr['stroke-width'];
                // Apply lineColor, or set it to default series color.
                attr.stroke = ((point && point.marker && point.marker.lineColor) ||
                    (seriesOptions.marker && seriesOptions.marker.lineColor) ||
                    borderColor ||
                    this.color);
                // Apply old borderWidth property if exists.
                attr['stroke-width'] = borderWidth;
                if (state) {
                    stateOptions =
                        merge(seriesOptions.states[state], seriesOptions.marker &&
                            seriesOptions.marker.states[state], point.options.states &&
                            point.options.states[state] || {});
                    brightness = stateOptions.brightness;
                    attr.fill =
                        stateOptions.color ||
                            H.color(attr.fill).brighten(brightness || 0).get();
                    attr.stroke = stateOptions.lineColor;
                }
                return attr;
            },
            /**
             * @private
             * @function Highcharts.seriesTypes.heatmap#markerAttribs
             * @param {Highcharts.HeatmapPoint} point
             * @return {Highcharts.SVGAttributes}
             */
            markerAttribs: function (point, state) {
                var pointMarkerOptions = point.marker || {},
                    seriesMarkerOptions = this.options.marker || {},
                    seriesStateOptions,
                    pointStateOptions,
                    shapeArgs = point.shapeArgs || {},
                    hasImage = point.hasImage,
                    attribs = {};
                if (hasImage) {
                    return {
                        x: point.plotX,
                        y: point.plotY
                    };
                }
                // Setting width and height attributes on image does not affect
                // on its dimensions.
                if (state) {
                    seriesStateOptions = seriesMarkerOptions.states[state] || {};
                    pointStateOptions = pointMarkerOptions.states &&
                        pointMarkerOptions.states[state] || {};
                    [['width', 'x'], ['height', 'y']].forEach(function (dimension) {
                        // Set new width and height basing on state options.
                        attribs[dimension[0]] = (pointStateOptions[dimension[0]] ||
                            seriesStateOptions[dimension[0]] ||
                            shapeArgs[dimension[0]]) + (pointStateOptions[dimension[0] + 'Plus'] ||
                            seriesStateOptions[dimension[0] + 'Plus'] || 0);
                        // Align marker by a new size.
                        attribs[dimension[1]] = shapeArgs[dimension[1]] +
                            (shapeArgs[dimension[0]] - attribs[dimension[0]]) / 2;
                    });
                }
                return state ? attribs : shapeArgs;
            },
            /**
             * @private
             * @function Highcharts.seriesTypes.heatmap#drawPoints
             * @return {void}
             */
            drawPoints: function () {
                var _this = this;
                // In styled mode, use CSS, otherwise the fill used in the style
                // sheet will take precedence over the fill attribute.
                var seriesMarkerOptions = this.options.marker || {};
                if (seriesMarkerOptions.enabled || this._hasPointMarkers) {
                    Series.prototype.drawPoints.call(this);
                    this.points.forEach(function (point) {
                        point.graphic &&
                            point.graphic[_this.chart.styledMode ? 'css' : 'animate'](_this.colorAttribs(point));
                    });
                }
            },
            // Define hasData function for non-cartesian series.
            // Returns true if the series has points at all.
            hasData: function () {
                return !!this.processedXData.length; // != 0
            },
            // Override to also allow null points, used when building the k-d-tree
            // for tooltips in boost mode.
            getValidPoints: function (points, insideOnly) {
                return Series.prototype.getValidPoints.call(this, points, insideOnly, true);
            },
            /**
             * @ignore
             * @deprecated
             * @function Highcharts.seriesTypes.heatmap#getBox
             */
            getBox: noop,
            /**
             * @private
             * @borrows Highcharts.LegendSymbolMixin.drawRectangle as Highcharts.seriesTypes.heatmap#drawLegendSymbol
             */
            drawLegendSymbol: LegendSymbolMixin.drawRectangle,
            /**
             * @private
             * @borrows Highcharts.seriesTypes.column#alignDataLabel as Highcharts.seriesTypes.heatmap#alignDataLabel
             */
            alignDataLabel: seriesTypes.column.prototype.alignDataLabel,
            /**
             * @private
             * @function Highcharts.seriesTypes.heatmap#getExtremes
             * @return {void}
             */
            getExtremes: function () {
                // Get the extremes from the value data
                var _a = Series.prototype.getExtremes
                        .call(this,
                    this.valueData),
                    dataMin = _a.dataMin,
                    dataMax = _a.dataMax;
                if (isNumber(dataMin)) {
                    this.valueMin = dataMin;
                }
                if (isNumber(dataMax)) {
                    this.valueMax = dataMax;
                }
                // Get the extremes from the y data
                return Series.prototype.getExtremes.call(this);
            }
            /* eslint-enable valid-jsdoc */
        }), merge(colorMapPointMixin, {
            /**
             * Heatmap series only. Padding between the points in the heatmap.
             * @name Highcharts.Point#pointPadding
             * @type {number|undefined}
             */
            /**
             * Heatmap series only. The value of the point, resulting in a color
             * controled by options as set in the colorAxis configuration.
             * @name Highcharts.Point#value
             * @type {number|null|undefined}
             */
            /* eslint-disable valid-jsdoc */
            /**
             * @private
             * @function Highcharts.Point#applyOptions
             * @param {Highcharts.HeatmapPointOptions} options
             * @param {number} x
             * @return {Highcharts.SVGPathArray}
             */
            applyOptions: function (options, x) {
                var point = H.Point.prototype
                        .applyOptions.call(this,
                    options,
                    x);
                point.formatPrefix =
                    point.isNull || point.value === null ?
                        'null' : 'point';
                return point;
            },
            /**
             * Color points have a value option that determines whether or not it is
             * a null point
             * @private
             * @function Highcharts.HeatmapPoint.isValid
             * @return {boolean}
             */
            isValid: function () {
                // undefined is allowed
                return (this.value !== Infinity &&
                    this.value !== -Infinity);
            },
            /**
             * @private
             * @function Highcharts.Point#haloPath
             * @param {number} size
             * @return {Highcharts.SVGPathArray}
             */
            haloPath: function (size) {
                if (!size) {
                    return [];
                }
                var rect = this.shapeArgs;
                return [
                    'M',
                    rect.x - size,
                    rect.y - size,
                    'L',
                    rect.x - size,
                    rect.y + rect.height + size,
                    rect.x + rect.width + size,
                    rect.y + rect.height + size,
                    rect.x + rect.width + size,
                    rect.y - size,
                    'Z'
                ];
            },
            getCellAttributes: function () {
                var point = this,
                    series = point.series,
                    seriesOptions = series.options,
                    xPad = (seriesOptions.colsize || 1) / 2,
                    yPad = (seriesOptions.rowsize || 1) / 2,
                    xAxis = series.xAxis,
                    yAxis = series.yAxis,
                    markerOptions = point.options.marker || series.options.marker,
                    pointPlacement = series.pointPlacementToXValue(), // #7860
                    pointPadding = pick(point.pointPadding,
                    seriesOptions.pointPadding, 0),
                    cellAttr = {
                        x1: clamp(Math.round(xAxis.len -
                            (xAxis.translate(point.x - xPad,
                    false,
                    true,
                    false,
                    true, -pointPlacement) || 0)), -xAxis.len, 2 * xAxis.len),
                        x2: clamp(Math.round(xAxis.len -
                            (xAxis.translate(point.x + xPad,
                    false,
                    true,
                    false,
                    true, -pointPlacement) || 0)), -xAxis.len, 2 * xAxis.len),
                        y1: clamp(Math.round((yAxis.translate(point.y - yPad,
                    false,
                    true,
                    false,
                    true) || 0)), -yAxis.len, 2 * yAxis.len),
                        y2: clamp(Math.round((yAxis.translate(point.y + yPad,
                    false,
                    true,
                    false,
                    true) || 0)), -yAxis.len, 2 * yAxis.len)
                    };
                // Handle marker's fixed width, and height values including border
                // and pointPadding while calculating cell attributes.
                [['width', 'x'], ['height', 'y']].forEach(function (dimension) {
                    var prop = dimension[0],
                        direction = dimension[1];
                    var start = direction + '1', end = direction + '2';
                    var side = Math.abs(cellAttr[start] - cellAttr[end]),
                        borderWidth = markerOptions &&
                            markerOptions.lineWidth || 0,
                        plotPos = Math.abs(cellAttr[start] + cellAttr[end]) / 2;
                    if (markerOptions[prop] &&
                        markerOptions[prop] < side) {
                        cellAttr[start] = plotPos - (markerOptions[prop] / 2) -
                            (borderWidth / 2);
                        cellAttr[end] = plotPos + (markerOptions[prop] / 2) +
                            (borderWidth / 2);
                    }
                    // Handle pointPadding
                    if (pointPadding) {
                        if (direction === 'y') {
                            start = end;
                            end = direction + '1';
                        }
                        cellAttr[start] += pointPadding;
                        cellAttr[end] -= pointPadding;
                    }
                });
                return cellAttr;
            }
            /* eslint-enable valid-jsdoc */
        }));
        /**
         * A `heatmap` series. If the [type](#series.heatmap.type) option is
         * not specified, it is inherited from [chart.type](#chart.type).
         *
         * @productdesc {highcharts}
         * Requires `modules/heatmap`.
         *
         * @extends   series,plotOptions.heatmap
         * @excluding cropThreshold, dataParser, dataURL, pointRange, stack,
         * @product   highcharts highmaps
         * @apioption series.heatmap
         */
        /**
         * An array of data points for the series. For the `heatmap` series
         * type, points can be given in the following ways:
         *
         * 1.  An array of arrays with 3 or 2 values. In this case, the values
         * correspond to `x,y,value`. If the first value is a string, it is
         * applied as the name of the point, and the `x` value is inferred.
         * The `x` value can also be omitted, in which case the inner arrays
         * should be of length 2\. Then the `x` value is automatically calculated,
         * either starting at 0 and incremented by 1, or from `pointStart`
         * and `pointInterval` given in the series options.
         *
         *  ```js
         *     data: [
         *         [0, 9, 7],
         *         [1, 10, 4],
         *         [2, 6, 3]
         *     ]
         *  ```
         *
         * 2.  An array of objects with named values. The following snippet shows only a
         * few settings, see the complete options set below. If the total number of data
         * points exceeds the series' [turboThreshold](#series.heatmap.turboThreshold),
         * this option is not available.
         *
         *  ```js
         *     data: [{
         *         x: 1,
         *         y: 3,
         *         value: 10,
         *         name: "Point2",
         *         color: "#00FF00"
         *     }, {
         *         x: 1,
         *         y: 7,
         *         value: 10,
         *         name: "Point1",
         *         color: "#FF00FF"
         *     }]
         *  ```
         *
         * @sample {highcharts} highcharts/chart/reflow-true/
         *         Numerical values
         * @sample {highcharts} highcharts/series/data-array-of-arrays/
         *         Arrays of numeric x and y
         * @sample {highcharts} highcharts/series/data-array-of-arrays-datetime/
         *         Arrays of datetime x and y
         * @sample {highcharts} highcharts/series/data-array-of-name-value/
         *         Arrays of point.name and y
         * @sample {highcharts} highcharts/series/data-array-of-objects/
         *         Config objects
         *
         * @type      {Array<Array<number>|*>}
         * @extends   series.line.data
         * @product   highcharts highmaps
         * @apioption series.heatmap.data
         */
        /**
         * The color of the point. In heat maps the point color is rarely set
         * explicitly, as we use the color to denote the `value`. Options for
         * this are set in the [colorAxis](#colorAxis) configuration.
         *
         * @type      {Highcharts.ColorString|Highcharts.GradientColorObject|Highcharts.PatternObject}
         * @product   highcharts highmaps
         * @apioption series.heatmap.data.color
         */
        /**
         * The value of the point, resulting in a color controled by options
         * as set in the [colorAxis](#colorAxis) configuration.
         *
         * @type      {number}
         * @product   highcharts highmaps
         * @apioption series.heatmap.data.value
         */
        /**
         * The x value of the point. For datetime axes,
         * the X value is the timestamp in milliseconds since 1970.
         *
         * @type      {number}
         * @product   highcharts highmaps
         * @apioption series.heatmap.data.x
         */
        /**
         * The y value of the point.
         *
         * @type      {number}
         * @product   highcharts highmaps
         * @apioption series.heatmap.data.y
         */
        /**
         * Point padding for a single point.
         *
         * @sample maps/plotoptions/tilemap-pointpadding
         *         Point padding on tiles
         *
         * @type      {number}
         * @product   highcharts highmaps
         * @apioption series.heatmap.data.pointPadding
         */
        /**
         * @excluding radius, enabledThreshold
         * @product   highcharts highmaps
         * @since     8.1
         * @apioption series.heatmap.data.marker
         */
        /**
         * @excluding radius, enabledThreshold
         * @product   highcharts highmaps
         * @since     8.1
         * @apioption series.heatmap.marker
         */
        /**
         * @excluding radius, radiusPlus
         * @product   highcharts highmaps
         * @apioption series.heatmap.marker.states.hover
         */
        /**
         * @excluding radius
         * @product   highcharts highmaps
         * @apioption series.heatmap.marker.states.select
         */
        /**
         * @excluding radius, radiusPlus
         * @product   highcharts highmaps
         * @apioption series.heatmap.data.marker.states.hover
         */
        /**
         * @excluding radius
         * @product   highcharts highmaps
         * @apioption series.heatmap.data.marker.states.select
         */
        /**
        * Set the marker's fixed width on hover state.
        *
        * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-linewidthplus
        *         5 pixels wider lineWidth on hover
        *
        * @type      {number|undefined}
        * @default   0
        * @product   highcharts highmaps
        * @apioption series.heatmap.marker.states.hover.lineWidthPlus
        */
        /**
        * Set the marker's fixed width on hover state.
        *
        * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-width
        *         70px fixed marker's width and height on hover
        *
        * @type      {number|undefined}
        * @default   undefined
        * @product   highcharts highmaps
        * @apioption series.heatmap.marker.states.hover.width
        */
        /**
         * Set the marker's fixed height on hover state.
         *
         * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-width
         *         70px fixed marker's width and height on hover
         *
         * @type      {number|undefined}
         * @default   undefined
         * @product   highcharts highmaps
         * @apioption series.heatmap.marker.states.hover.height
         */
        /**
        * The number of pixels to increase the width of the
        * hovered point.
        *
        * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-widthplus
        *         One day
        *
        * @type      {number|undefined}
        * @default   undefined
        * @product   highcharts highmaps
        * @apioption series.heatmap.marker.states.hover.widthPlus
        */
        /**
         * The number of pixels to increase the height of the
         * hovered point.
         *
         * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-widthplus
         *         One day
         *
         * @type      {number|undefined}
         * @default   undefined
         * @product   highcharts highmaps
         * @apioption series.heatmap.marker.states.hover.heightPlus
         */
        /**
         * The number of pixels to increase the width of the
         * hovered point.
         *
         * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-widthplus
         *         One day
         *
         * @type      {number|undefined}
         * @default   undefined
         * @product   highcharts highmaps
         * @apioption series.heatmap.marker.states.select.widthPlus
         */
        /**
         * The number of pixels to increase the height of the
         * hovered point.
         *
         * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-widthplus
         *         One day
         *
         * @type      {number|undefined}
         * @default   undefined
         * @product   highcharts highmaps
         * @apioption series.heatmap.marker.states.select.heightPlus
         */
        /**
        * Set the marker's fixed width on hover state.
        *
        * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-linewidthplus
        *         5 pixels wider lineWidth on hover
        *
        * @type      {number|undefined}
        * @default   0
        * @product   highcharts highmaps
        * @apioption series.heatmap.data.marker.states.hover.lineWidthPlus
        */
        /**
         * Set the marker's fixed width on hover state.
         *
         * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-width
         *         70px fixed marker's width and height on hover
         *
         * @type      {number|undefined}
         * @default   undefined
         * @product   highcharts highmaps
         * @apioption series.heatmap.data.marker.states.hover.width
         */
        /**
         * Set the marker's fixed height on hover state.
         *
         * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-width
         *         70px fixed marker's width and height on hover
         *
         * @type      {number|undefined}
         * @default   undefined
         * @product   highcharts highmaps
         * @apioption series.heatmap.data.marker.states.hover.height
         */
        /**
         * The number of pixels to increase the width of the
         * hovered point.
         *
         * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-widthplus
         *         One day
         *
         * @type      {number|undefined}
         * @default   undefined
         * @product   highcharts highstock
         * @apioption series.heatmap.data.marker.states.hover.widthPlus
         */
        /**
         * The number of pixels to increase the height of the
         * hovered point.
         *
         * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-widthplus
         *         One day
         *
         * @type      {number|undefined}
         * @default   undefined
         * @product   highcharts highstock
         * @apioption series.heatmap.data.marker.states.hover.heightPlus
         */
        /**
        * Set the marker's fixed width on select state.
        *
        * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-width
        *         70px fixed marker's width and height on hover
        *
        * @type      {number|undefined}
        * @default   undefined
        * @product   highcharts highmaps
        * @apioption series.heatmap.data.marker.states.select.width
        */
        /**
         * Set the marker's fixed height on select state.
         *
         * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-width
         *         70px fixed marker's width and height on hover
         *
         * @type      {number|undefined}
         * @default   undefined
         * @product   highcharts highmaps
         * @apioption series.heatmap.data.marker.states.select.height
         */
        /**
         * The number of pixels to increase the width of the
         * hovered point.
         *
         * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-widthplus
         *         One day
         *
         * @type      {number|undefined}
         * @default   undefined
         * @product   highcharts highstock
         * @apioption series.heatmap.data.marker.states.select.widthPlus
         */
        /**
         * The number of pixels to increase the height of the
         * hovered point.
         *
         * @sample {highcharts} maps/plotoptions/heatmap-marker-states-hover-widthplus
         *         One day
         *
         * @type      {number|undefined}
         * @default   undefined
         * @product   highcharts highstock
         * @apioption series.heatmap.data.marker.states.select.heightPlus
         */
        ''; // adds doclets above to transpiled file

    });
    _registerModule(_modules, 'masters/modules/heatmap.src.js', [], function () {


    });
}));