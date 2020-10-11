/**
 * @license Highcharts Gantt JS v8.2.0 (2020-08-20)
 *
 * Tree Grid
 *
 * (c) 2016-2019 Jon Arild Nygard
 *
 * License: www.highcharts.com/license
 */
'use strict';
(function (factory) {
    if (typeof module === 'object' && module.exports) {
        factory['default'] = factory;
        module.exports = factory;
    } else if (typeof define === 'function' && define.amd) {
        define('highcharts/modules/treegrid', ['highcharts'], function (Highcharts) {
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
    _registerModule(_modules, 'Gantt/Tree.js', [_modules['Core/Utilities.js']], function (U) {
        /* *
         *
         *  (c) 2016-2020 Highsoft AS
         *
         *  Authors: Jon Arild Nygard
         *
         *  License: www.highcharts.com/license
         *
         *  !!!!!!! SOURCE GETS TRANSPILED BY TYPESCRIPT. EDIT TS FILE ONLY. !!!!!!!
         *
         * */
        /* eslint no-console: 0 */
        var extend = U.extend,
            isNumber = U.isNumber,
            pick = U.pick;
        /**
         * Creates an object map from parent id to childrens index.
         *
         * @private
         * @function Highcharts.Tree#getListOfParents
         *
         * @param {Array<*>} data
         *        List of points set in options. `Array.parent` is parent id of point.
         *
         * @param {Array<string>} ids
         *        List of all point ids.
         *
         * @return {Highcharts.Dictionary<Array<*>>}
         *         Map from parent id to children index in data
         */
        var getListOfParents = function (data,
            ids) {
                var listOfParents = data.reduce(function (prev,
            curr) {
                    var parent = pick(curr.parent, '');
                if (typeof prev[parent] === 'undefined') {
                    prev[parent] = [];
                }
                prev[parent].push(curr);
                return prev;
            }, {}), parents = Object.keys(listOfParents);
            // If parent does not exist, hoist parent to root of tree.
            parents.forEach(function (parent, list) {
                var children = listOfParents[parent];
                if ((parent !== '') && (ids.indexOf(parent) === -1)) {
                    children.forEach(function (child) {
                        list[''].push(child);
                    });
                    delete list[parent];
                }
            });
            return listOfParents;
        };
        var getNode = function (id,
            parent,
            level,
            data,
            mapOfIdToChildren,
            options) {
                var descendants = 0,
            height = 0,
            after = options && options.after,
            before = options && options.before,
            node = {
                    data: data,
                    depth: level - 1,
                    id: id,
                    level: level,
                    parent: parent
                },
            start,
            end,
            children;
            // Allow custom logic before the children has been created.
            if (typeof before === 'function') {
                before(node, options);
            }
            // Call getNode recursively on the children. Calulate the height of the
            // node, and the number of descendants.
            children = ((mapOfIdToChildren[id] || [])).map(function (child) {
                var node = getNode(child.id,
                    id, (level + 1),
                    child,
                    mapOfIdToChildren,
                    options),
                    childStart = child.start,
                    childEnd = (child.milestone === true ?
                        childStart :
                        child.end);
                // Start should be the lowest child.start.
                start = ((!isNumber(start) || childStart < start) ?
                    childStart :
                    start);
                // End should be the largest child.end.
                // If child is milestone, then use start as end.
                end = ((!isNumber(end) || childEnd > end) ?
                    childEnd :
                    end);
                descendants = descendants + 1 + node.descendants;
                height = Math.max(node.height + 1, height);
                return node;
            });
            // Calculate start and end for point if it is not already explicitly set.
            if (data) {
                data.start = pick(data.start, start);
                data.end = pick(data.end, end);
            }
            extend(node, {
                children: children,
                descendants: descendants,
                height: height
            });
            // Allow custom logic after the children has been created.
            if (typeof after === 'function') {
                after(node, options);
            }
            return node;
        };
        var getTree = function (data,
            options) {
                var ids = data.map(function (d) {
                    return d.id;
            }), mapOfIdToChildren = getListOfParents(data, ids);
            return getNode('', null, 1, null, mapOfIdToChildren, options);
        };
        var Tree = {
                getListOfParents: getListOfParents,
                getNode: getNode,
                getTree: getTree
            };

        return Tree;
    });
    _registerModule(_modules, 'Core/Axis/TreeGridTick.js', [_modules['Core/Utilities.js']], function (U) {
        /* *
         *
         *  (c) 2016 Highsoft AS
         *  Authors: Jon Arild Nygard
         *
         *  License: www.highcharts.com/license
         *
         *  !!!!!!! SOURCE GETS TRANSPILED BY TYPESCRIPT. EDIT TS FILE ONLY. !!!!!!!
         *
         * */
        var addEvent = U.addEvent,
            defined = U.defined,
            isObject = U.isObject,
            isNumber = U.isNumber,
            pick = U.pick,
            wrap = U.wrap;
        /**
         * @private
         */
        var TreeGridTick;
        (function (TreeGridTick) {
            /* *
             *
             *  Interfaces
             *
             * */
            /* *
             *
             *  Variables
             *
             * */
            var applied = false;
            /* *
             *
             *  Functions
             *
             * */
            /**
             * @private
             */
            function compose(TickClass) {
                if (!applied) {
                    addEvent(TickClass, 'init', onInit);
                    wrap(TickClass.prototype, 'getLabelPosition', wrapGetLabelPosition);
                    wrap(TickClass.prototype, 'renderLabel', wrapRenderLabel);
                    // backwards compatibility
                    TickClass.prototype.collapse = function (redraw) {
                        this.treeGrid.collapse(redraw);
                    };
                    TickClass.prototype.expand = function (redraw) {
                        this.treeGrid.expand(redraw);
                    };
                    TickClass.prototype.toggleCollapse = function (redraw) {
                        this.treeGrid.toggleCollapse(redraw);
                    };
                    applied = true;
                }
            }
            TreeGridTick.compose = compose;
            /**
             * @private
             */
            function onInit() {
                var tick = this;
                if (!tick.treeGrid) {
                    tick.treeGrid = new Additions(tick);
                }
            }
            /**
             * @private
             */
            function onTickHover(label) {
                label.addClass('highcharts-treegrid-node-active');
                if (!label.renderer.styledMode) {
                    label.css({
                        textDecoration: 'underline'
                    });
                }
            }
            /**
             * @private
             */
            function onTickHoverExit(label, options) {
                var css = defined(options.style) ? options.style : {};
                label.removeClass('highcharts-treegrid-node-active');
                if (!label.renderer.styledMode) {
                    label.css({ textDecoration: css.textDecoration });
                }
            }
            /**
             * @private
             */
            function renderLabelIcon(tick, params) {
                var treeGrid = tick.treeGrid,
                    isNew = !treeGrid.labelIcon,
                    renderer = params.renderer,
                    labelBox = params.xy,
                    options = params.options,
                    width = options.width,
                    height = options.height,
                    iconCenter = {
                        x: labelBox.x - (width / 2) - options.padding,
                        y: labelBox.y - (height / 2)
                    },
                    rotation = params.collapsed ? 90 : 180,
                    shouldRender = params.show && isNumber(iconCenter.y);
                var icon = treeGrid.labelIcon;
                if (!icon) {
                    treeGrid.labelIcon = icon = renderer
                        .path(renderer.symbols[options.type](options.x, options.y, width, height))
                        .addClass('highcharts-label-icon')
                        .add(params.group);
                }
                // Set the new position, and show or hide
                if (!shouldRender) {
                    icon.attr({ y: -9999 }); // #1338
                }
                // Presentational attributes
                if (!renderer.styledMode) {
                    icon
                        .attr({
                        'stroke-width': 1,
                        'fill': pick(params.color, '#666666')
                    })
                        .css({
                        cursor: 'pointer',
                        stroke: options.lineColor,
                        strokeWidth: options.lineWidth
                    });
                }
                // Update the icon positions
                icon[isNew ? 'attr' : 'animate']({
                    translateX: iconCenter.x,
                    translateY: iconCenter.y,
                    rotation: rotation
                });
            }
            /**
             * @private
             */
            function wrapGetLabelPosition(proceed, x, y, label, horiz, labelOptions, tickmarkOffset, index, step) {
                var tick = this,
                    lbOptions = pick(tick.options && tick.options.labels,
                    labelOptions),
                    pos = tick.pos,
                    axis = tick.axis,
                    options = axis.options,
                    isTreeGrid = options.type === 'treegrid',
                    result = proceed.apply(tick,
                    [x,
                    y,
                    label,
                    horiz,
                    lbOptions,
                    tickmarkOffset,
                    index,
                    step]);
                var symbolOptions,
                    indentation,
                    mapOfPosToGridNode,
                    node,
                    level;
                if (isTreeGrid) {
                    symbolOptions = (lbOptions && isObject(lbOptions.symbol, true) ?
                        lbOptions.symbol :
                        {});
                    indentation = (lbOptions && isNumber(lbOptions.indentation) ?
                        lbOptions.indentation :
                        0);
                    mapOfPosToGridNode = axis.treeGrid.mapOfPosToGridNode;
                    node = mapOfPosToGridNode && mapOfPosToGridNode[pos];
                    level = (node && node.depth) || 1;
                    result.x += (
                    // Add space for symbols
                    ((symbolOptions.width) + (symbolOptions.padding * 2)) +
                        // Apply indentation
                        ((level - 1) * indentation));
                }
                return result;
            }
            /**
             * @private
             */
            function wrapRenderLabel(proceed) {
                var tick = this, pos = tick.pos, axis = tick.axis, label = tick.label, mapOfPosToGridNode = axis.treeGrid.mapOfPosToGridNode, options = axis.options, labelOptions = pick(tick.options && tick.options.labels, options && options.labels), symbolOptions = (labelOptions && isObject(labelOptions.symbol, true) ?
                        labelOptions.symbol :
                        {}), node = mapOfPosToGridNode && mapOfPosToGridNode[pos], level = node && node.depth, isTreeGrid = options.type === 'treegrid', shouldRender = axis.tickPositions.indexOf(pos) > -1, prefixClassName = 'highcharts-treegrid-node-', styledMode = axis.chart.styledMode;
                var collapsed,
                    addClassName,
                    removeClassName;
                if (isTreeGrid && node) {
                    // Add class name for hierarchical styling.
                    if (label &&
                        label.element) {
                        label.addClass(prefixClassName + 'level-' + level);
                    }
                }
                proceed.apply(tick, Array.prototype.slice.call(arguments, 1));
                if (isTreeGrid &&
                    label &&
                    label.element &&
                    node &&
                    node.descendants &&
                    node.descendants > 0) {
                    collapsed = axis.treeGrid.isCollapsed(node);
                    renderLabelIcon(tick, {
                        color: !styledMode && label.styles && label.styles.color || '',
                        collapsed: collapsed,
                        group: label.parentGroup,
                        options: symbolOptions,
                        renderer: label.renderer,
                        show: shouldRender,
                        xy: label.xy
                    });
                    // Add class name for the node.
                    addClassName = prefixClassName +
                        (collapsed ? 'collapsed' : 'expanded');
                    removeClassName = prefixClassName +
                        (collapsed ? 'expanded' : 'collapsed');
                    label
                        .addClass(addClassName)
                        .removeClass(removeClassName);
                    if (!styledMode) {
                        label.css({
                            cursor: 'pointer'
                        });
                    }
                    // Add events to both label text and icon
                    [label, tick.treeGrid.labelIcon].forEach(function (object) {
                        if (object && !object.attachedTreeGridEvents) {
                            // On hover
                            addEvent(object.element, 'mouseover', function () {
                                onTickHover(label);
                            });
                            // On hover out
                            addEvent(object.element, 'mouseout', function () {
                                onTickHoverExit(label, labelOptions);
                            });
                            addEvent(object.element, 'click', function () {
                                tick.treeGrid.toggleCollapse();
                            });
                            object.attachedTreeGridEvents = true;
                        }
                    });
                }
            }
            /* *
             *
             *  Classes
             *
             * */
            /**
             * @private
             * @class
             */
            var Additions = /** @class */ (function () {
                    /* *
                     *
                     *  Constructors
                     *
                     * */
                    /**
                     * @private
                     */
                    function Additions(tick) {
                        this.tick = tick;
                }
                /* *
                 *
                 *  Functions
                 *
                 * */
                /**
                 * Collapse the grid cell. Used when axis is of type treegrid.
                 *
                 * @see gantt/treegrid-axis/collapsed-dynamically/demo.js
                 *
                 * @private
                 * @function Highcharts.Tick#collapse
                 *
                 * @param {boolean} [redraw=true]
                 * Whether to redraw the chart or wait for an explicit call to
                 * {@link Highcharts.Chart#redraw}
                 */
                Additions.prototype.collapse = function (redraw) {
                    var tick = this.tick,
                        axis = tick.axis,
                        brokenAxis = axis.brokenAxis;
                    if (brokenAxis &&
                        axis.treeGrid.mapOfPosToGridNode) {
                        var pos = tick.pos,
                            node = axis.treeGrid.mapOfPosToGridNode[pos],
                            breaks = axis.treeGrid.collapse(node);
                        brokenAxis.setBreaks(breaks, pick(redraw, true));
                    }
                };
                /**
                 * Expand the grid cell. Used when axis is of type treegrid.
                 *
                 * @see gantt/treegrid-axis/collapsed-dynamically/demo.js
                 *
                 * @private
                 * @function Highcharts.Tick#expand
                 *
                 * @param {boolean} [redraw=true]
                 * Whether to redraw the chart or wait for an explicit call to
                 * {@link Highcharts.Chart#redraw}
                 */
                Additions.prototype.expand = function (redraw) {
                    var tick = this.tick,
                        axis = tick.axis,
                        brokenAxis = axis.brokenAxis;
                    if (brokenAxis &&
                        axis.treeGrid.mapOfPosToGridNode) {
                        var pos = tick.pos,
                            node = axis.treeGrid.mapOfPosToGridNode[pos],
                            breaks = axis.treeGrid.expand(node);
                        brokenAxis.setBreaks(breaks, pick(redraw, true));
                    }
                };
                /**
                 * Toggle the collapse/expand state of the grid cell. Used when axis is
                 * of type treegrid.
                 *
                 * @see gantt/treegrid-axis/collapsed-dynamically/demo.js
                 *
                 * @private
                 * @function Highcharts.Tick#toggleCollapse
                 *
                 * @param {boolean} [redraw=true]
                 * Whether to redraw the chart or wait for an explicit call to
                 * {@link Highcharts.Chart#redraw}
                 */
                Additions.prototype.toggleCollapse = function (redraw) {
                    var tick = this.tick,
                        axis = tick.axis,
                        brokenAxis = axis.brokenAxis;
                    if (brokenAxis &&
                        axis.treeGrid.mapOfPosToGridNode) {
                        var pos = tick.pos,
                            node = axis.treeGrid.mapOfPosToGridNode[pos],
                            breaks = axis.treeGrid.toggleCollapse(node);
                        brokenAxis.setBreaks(breaks, pick(redraw, true));
                    }
                };
                return Additions;
            }());
            TreeGridTick.Additions = Additions;
        })(TreeGridTick || (TreeGridTick = {}));

        return TreeGridTick;
    });
    _registerModule(_modules, 'Mixins/TreeSeries.js', [_modules['Core/Color.js'], _modules['Core/Utilities.js']], function (Color, U) {
        /* *
         *
         *  !!!!!!! SOURCE GETS TRANSPILED BY TYPESCRIPT. EDIT TS FILE ONLY. !!!!!!!
         *
         * */
        var extend = U.extend,
            isArray = U.isArray,
            isNumber = U.isNumber,
            isObject = U.isObject,
            merge = U.merge,
            pick = U.pick;
        var isBoolean = function (x) {
                return typeof x === 'boolean';
        }, isFn = function (x) {
            return typeof x === 'function';
        };
        /* eslint-disable valid-jsdoc */
        /**
         * @todo Combine buildTree and buildNode with setTreeValues
         * @todo Remove logic from Treemap and make it utilize this mixin.
         * @private
         */
        var setTreeValues = function setTreeValues(tree,
            options) {
                var before = options.before,
            idRoot = options.idRoot,
            mapIdToNode = options.mapIdToNode,
            nodeRoot = mapIdToNode[idRoot],
            levelIsConstant = (isBoolean(options.levelIsConstant) ?
                    options.levelIsConstant :
                    true),
            points = options.points,
            point = points[tree.i],
            optionsPoint = point && point.options || {},
            childrenTotal = 0,
            children = [],
            value;
            extend(tree, {
                levelDynamic: tree.level - (levelIsConstant ? 0 : nodeRoot.level),
                name: pick(point && point.name, ''),
                visible: (idRoot === tree.id ||
                    (isBoolean(options.visible) ? options.visible : false))
            });
            if (isFn(before)) {
                tree = before(tree, options);
            }
            // First give the children some values
            tree.children.forEach(function (child, i) {
                var newOptions = extend({},
                    options);
                extend(newOptions, {
                    index: i,
                    siblings: tree.children.length,
                    visible: tree.visible
                });
                child = setTreeValues(child, newOptions);
                children.push(child);
                if (child.visible) {
                    childrenTotal += child.val;
                }
            });
            tree.visible = childrenTotal > 0 || tree.visible;
            // Set the values
            value = pick(optionsPoint.value, childrenTotal);
            extend(tree, {
                children: children,
                childrenTotal: childrenTotal,
                isLeaf: tree.visible && !childrenTotal,
                val: value
            });
            return tree;
        };
        /**
         * @private
         */
        var getColor = function getColor(node,
            options) {
                var index = options.index,
            mapOptionsToLevel = options.mapOptionsToLevel,
            parentColor = options.parentColor,
            parentColorIndex = options.parentColorIndex,
            series = options.series,
            colors = options.colors,
            siblings = options.siblings,
            points = series.points,
            getColorByPoint,
            chartOptionsChart = series.chart.options.chart,
            point,
            level,
            colorByPoint,
            colorIndexByPoint,
            color,
            colorIndex;
            /**
             * @private
             */
            function variation(color) {
                var colorVariation = level && level.colorVariation;
                if (colorVariation) {
                    if (colorVariation.key === 'brightness') {
                        return Color.parse(color).brighten(colorVariation.to * (index / siblings)).get();
                    }
                }
                return color;
            }
            if (node) {
                point = points[node.i];
                level = mapOptionsToLevel[node.level] || {};
                getColorByPoint = point && level.colorByPoint;
                if (getColorByPoint) {
                    colorIndexByPoint = point.index % (colors ?
                        colors.length :
                        chartOptionsChart.colorCount);
                    colorByPoint = colors && colors[colorIndexByPoint];
                }
                // Select either point color, level color or inherited color.
                if (!series.chart.styledMode) {
                    color = pick(point && point.options.color, level && level.color, colorByPoint, parentColor && variation(parentColor), series.color);
                }
                colorIndex = pick(point && point.options.colorIndex, level && level.colorIndex, colorIndexByPoint, parentColorIndex, options.colorIndex);
            }
            return {
                color: color,
                colorIndex: colorIndex
            };
        };
        /**
         * Creates a map from level number to its given options.
         *
         * @private
         * @function getLevelOptions
         * @param {object} params
         *        Object containing parameters.
         *        - `defaults` Object containing default options. The default options
         *           are merged with the userOptions to get the final options for a
         *           specific level.
         *        - `from` The lowest level number.
         *        - `levels` User options from series.levels.
         *        - `to` The highest level number.
         * @return {Highcharts.Dictionary<object>|null}
         *         Returns a map from level number to its given options.
         */
        var getLevelOptions = function getLevelOptions(params) {
                var result = null,
            defaults,
            converted,
            i,
            from,
            to,
            levels;
            if (isObject(params)) {
                result = {};
                from = isNumber(params.from) ? params.from : 1;
                levels = params.levels;
                converted = {};
                defaults = isObject(params.defaults) ? params.defaults : {};
                if (isArray(levels)) {
                    converted = levels.reduce(function (obj, item) {
                        var level,
                            levelIsConstant,
                            options;
                        if (isObject(item) && isNumber(item.level)) {
                            options = merge({}, item);
                            levelIsConstant = (isBoolean(options.levelIsConstant) ?
                                options.levelIsConstant :
                                defaults.levelIsConstant);
                            // Delete redundant properties.
                            delete options.levelIsConstant;
                            delete options.level;
                            // Calculate which level these options apply to.
                            level = item.level + (levelIsConstant ? 0 : from - 1);
                            if (isObject(obj[level])) {
                                extend(obj[level], options);
                            }
                            else {
                                obj[level] = options;
                            }
                        }
                        return obj;
                    }, {});
                }
                to = isNumber(params.to) ? params.to : 1;
                for (i = 0; i <= to; i++) {
                    result[i] = merge({}, defaults, isObject(converted[i]) ? converted[i] : {});
                }
            }
            return result;
        };
        /**
         * Update the rootId property on the series. Also makes sure that it is
         * accessible to exporting.
         *
         * @private
         * @function updateRootId
         *
         * @param {object} series
         *        The series to operate on.
         *
         * @return {string}
         *         Returns the resulting rootId after update.
         */
        var updateRootId = function (series) {
                var rootId,
            options;
            if (isObject(series)) {
                // Get the series options.
                options = isObject(series.options) ? series.options : {};
                // Calculate the rootId.
                rootId = pick(series.rootNode, options.rootId, '');
                // Set rootId on series.userOptions to pick it up in exporting.
                if (isObject(series.userOptions)) {
                    series.userOptions.rootId = rootId;
                }
                // Set rootId on series to pick it up on next update.
                series.rootNode = rootId;
            }
            return rootId;
        };
        var result = {
                getColor: getColor,
                getLevelOptions: getLevelOptions,
                setTreeValues: setTreeValues,
                updateRootId: updateRootId
            };

        return result;
    });
    _registerModule(_modules, 'Core/Axis/GridAxis.js', [_modules['Core/Axis/Axis.js'], _modules['Core/Globals.js'], _modules['Core/Options.js'], _modules['Core/Axis/Tick.js'], _modules['Core/Utilities.js']], function (Axis, H, O, Tick, U) {
        /* *
         *
         *  (c) 2016 Highsoft AS
         *  Authors: Lars A. V. Cabrera
         *
         *  License: www.highcharts.com/license
         *
         *  !!!!!!! SOURCE GETS TRANSPILED BY TYPESCRIPT. EDIT TS FILE ONLY. !!!!!!!
         *
         * */
        var dateFormat = O.dateFormat;
        var addEvent = U.addEvent,
            defined = U.defined,
            erase = U.erase,
            find = U.find,
            isArray = U.isArray,
            isNumber = U.isNumber,
            merge = U.merge,
            pick = U.pick,
            timeUnits = U.timeUnits,
            wrap = U.wrap;
        var argsToArray = function (args) {
                return Array.prototype.slice.call(args, 1);
        }, isObject = function (x) {
            // Always use strict mode
            return U.isObject(x, true);
        }, Chart = H.Chart;
        var applyGridOptions = function applyGridOptions(axis) {
                var options = axis.options;
            // Center-align by default
            if (!options.labels) {
                options.labels = {};
            }
            options.labels.align = pick(options.labels.align, 'center');
            // @todo: Check against tickLabelPlacement between/on etc
            /* Prevents adding the last tick label if the axis is not a category
               axis.
               Since numeric labels are normally placed at starts and ends of a
               range of value, and this module makes the label point at the value,
               an "extra" label would appear. */
            if (!axis.categories) {
                options.showLastLabel = false;
            }
            // Prevents rotation of labels when squished, as rotating them would not
            // help.
            axis.labelRotation = 0;
            options.labels.rotation = 0;
        };
        /**
         * For a datetime axis, the scale will automatically adjust to the
         * appropriate unit. This member gives the default string
         * representations used for each unit. For intermediate values,
         * different units may be used, for example the `day` unit can be used
         * on midnight and `hour` unit be used for intermediate values on the
         * same axis.
         * For grid axes (like in Gantt charts),
         * it is possible to declare as a list to provide different
         * formats depending on available space.
         * For an overview of the replacement codes, see
         * [dateFormat](/class-reference/Highcharts#dateFormat).
         *
         * Defaults to:
         * ```js
         * {
                hour: {
                    list: ['%H:%M', '%H']
                },
                day: {
                    list: ['%A, %e. %B', '%a, %e. %b', '%E']
                },
                week: {
                    list: ['Week %W', 'W%W']
                },
                month: {
                    list: ['%B', '%b', '%o']
                }
            },
         * ```
         *
         * @sample {gantt} gantt/demo/left-axis-table
         *         Gantt Chart with custom axis date format.
         *
         * @product gantt
         * @apioption xAxis.dateTimeLabelFormats
         */
        /**
         * Set grid options for the axis labels. Requires Highcharts Gantt.
         *
         * @since     6.2.0
         * @product   gantt
         * @apioption xAxis.grid
         */
        /**
         * Enable grid on the axis labels. Defaults to true for Gantt charts.
         *
         * @type      {boolean}
         * @default   true
         * @since     6.2.0
         * @product   gantt
         * @apioption xAxis.grid.enabled
         */
        /**
         * Set specific options for each column (or row for horizontal axes) in the
         * grid. Each extra column/row is its own axis, and the axis options can be set
         * here.
         *
         * @sample gantt/demo/left-axis-table
         *         Left axis as a table
         *
         * @type      {Array<Highcharts.XAxisOptions>}
         * @apioption xAxis.grid.columns
         */
        /**
         * Set border color for the label grid lines.
         *
         * @type      {Highcharts.ColorString}
         * @apioption xAxis.grid.borderColor
         */
        /**
         * Set border width of the label grid lines.
         *
         * @type      {number}
         * @default   1
         * @apioption xAxis.grid.borderWidth
         */
        /**
         * Set cell height for grid axis labels. By default this is calculated from font
         * size. This option only applies to horizontal axes.
         *
         * @sample gantt/grid-axis/cellheight
         *         Gant chart with custom cell height
         * @type      {number}
         * @apioption xAxis.grid.cellHeight
         */
        ''; // detach doclets above
        /**
         * Get the largest label width and height.
         *
         * @private
         * @function Highcharts.Axis#getMaxLabelDimensions
         *
         * @param {Highcharts.Dictionary<Highcharts.Tick>} ticks
         * All the ticks on one axis.
         *
         * @param {Array<number|string>} tickPositions
         * All the tick positions on one axis.
         *
         * @return {Highcharts.SizeObject}
         * Object containing the properties height and width.
         *
         * @todo Move this to the generic axis implementation, as it is used there.
         */
        Axis.prototype.getMaxLabelDimensions = function (ticks, tickPositions) {
            var dimensions = {
                    width: 0,
                    height: 0
                };
            tickPositions.forEach(function (pos) {
                var tick = ticks[pos],
                    tickHeight = 0,
                    tickWidth = 0,
                    label;
                if (isObject(tick)) {
                    label = isObject(tick.label) ? tick.label : {};
                    // Find width and height of tick
                    tickHeight = label.getBBox ? label.getBBox().height : 0;
                    if (label.textStr) {
                        // Set the tickWidth same as the label width after ellipsis
                        // applied #10281
                        tickWidth = Math.round(label.getBBox().width);
                    }
                    // Update the result if width and/or height are larger
                    dimensions.height = Math.max(tickHeight, dimensions.height);
                    dimensions.width = Math.max(tickWidth, dimensions.width);
                }
            });
            return dimensions;
        };
        // Adds week date format
        H.dateFormats.W = function (timestamp) {
            var d = new this.Date(timestamp);
            var firstDay = (this.get('Day',
                d) + 6) % 7;
            var thursday = new this.Date(d.valueOf());
            this.set('Date', thursday, this.get('Date', d) - firstDay + 3);
            var firstThursday = new this.Date(this.get('FullYear',
                thursday), 0, 1);
            if (this.get('Day', firstThursday) !== 4) {
                this.set('Month', d, 0);
                this.set('Date', d, 1 + (11 - this.get('Day', firstThursday)) % 7);
            }
            return (1 +
                Math.floor((thursday.valueOf() - firstThursday.valueOf()) / 604800000)).toString();
        };
        // First letter of the day of the week, e.g. 'M' for 'Monday'.
        H.dateFormats.E = function (timestamp) {
            return dateFormat('%a', timestamp, true).charAt(0);
        };
        /* eslint-disable no-invalid-this */
        addEvent(Chart, 'afterSetChartSize', function () {
            this.axes.forEach(function (axis) {
                (axis.grid && axis.grid.columns || []).forEach(function (column) {
                    column.setAxisSize();
                    column.setAxisTranslation();
                });
            });
        });
        // Center tick labels in cells.
        addEvent(Tick, 'afterGetLabelPosition', function (e) {
            var tick = this,
                label = tick.label,
                axis = tick.axis,
                reversed = axis.reversed,
                chart = axis.chart,
                options = axis.options,
                gridOptions = options.grid || {},
                labelOpts = axis.options.labels,
                align = labelOpts.align, 
                // verticalAlign is currently not supported for axis.labels.
                verticalAlign = 'middle', // labelOpts.verticalAlign,
                side = GridAxis.Side[axis.side],
                tickmarkOffset = e.tickmarkOffset,
                tickPositions = axis.tickPositions,
                tickPos = tick.pos - tickmarkOffset,
                nextTickPos = (isNumber(tickPositions[e.index + 1]) ?
                    tickPositions[e.index + 1] - tickmarkOffset :
                    axis.max + tickmarkOffset),
                tickSize = axis.tickSize('tick'),
                tickWidth = tickSize ? tickSize[0] : 0,
                crispCorr = tickSize ? tickSize[1] / 2 : 0,
                labelHeight,
                lblMetrics,
                lines,
                bottom,
                top,
                left,
                right;
            // Only center tick labels in grid axes
            if (gridOptions.enabled === true) {
                // Calculate top and bottom positions of the cell.
                if (side === 'top') {
                    bottom = axis.top + axis.offset;
                    top = bottom - tickWidth;
                }
                else if (side === 'bottom') {
                    top = chart.chartHeight - axis.bottom + axis.offset;
                    bottom = top + tickWidth;
                }
                else {
                    bottom = axis.top + axis.len - axis.translate(reversed ? nextTickPos : tickPos);
                    top = axis.top + axis.len - axis.translate(reversed ? tickPos : nextTickPos);
                }
                // Calculate left and right positions of the cell.
                if (side === 'right') {
                    left = chart.chartWidth - axis.right + axis.offset;
                    right = left + tickWidth;
                }
                else if (side === 'left') {
                    right = axis.left + axis.offset;
                    left = right - tickWidth;
                }
                else {
                    left = Math.round(axis.left + axis.translate(reversed ? nextTickPos : tickPos)) - crispCorr;
                    right = Math.round(axis.left + axis.translate(reversed ? tickPos : nextTickPos)) - crispCorr;
                }
                tick.slotWidth = right - left;
                // Calculate the positioning of the label based on
                // alignment.
                e.pos.x = (align === 'left' ?
                    left :
                    align === 'right' ?
                        right :
                        left + ((right - left) / 2) // default to center
                );
                e.pos.y = (verticalAlign === 'top' ?
                    top :
                    verticalAlign === 'bottom' ?
                        bottom :
                        top + ((bottom - top) / 2) // default to middle
                );
                lblMetrics = chart.renderer.fontMetrics(labelOpts.style.fontSize, label.element);
                labelHeight = label.getBBox().height;
                // Adjustment to y position to align the label correctly.
                // Would be better to have a setter or similar for this.
                if (!labelOpts.useHTML) {
                    lines = Math.round(labelHeight / lblMetrics.h);
                    e.pos.y += (
                    // Center the label
                    // TODO: why does this actually center the label?
                    ((lblMetrics.b - (lblMetrics.h - lblMetrics.f)) / 2) +
                        // Adjust for height of additional lines.
                        -(((lines - 1) * lblMetrics.h) / 2));
                }
                else {
                    e.pos.y += (
                    // Readjust yCorr in htmlUpdateTransform
                    lblMetrics.b +
                        // Adjust for height of html label
                        -(labelHeight / 2));
                }
                e.pos.x += (axis.horiz && labelOpts.x || 0);
            }
        });
        /* eslint-enable no-invalid-this */
        /**
         * Additions for grid axes.
         * @private
         * @class
         */
        var GridAxisAdditions = /** @class */ (function () {
                /* *
                 *
                 *  Constructors
                 *
                 * */
                function GridAxisAdditions(axis) {
                    this.axis = axis;
            }
            /* *
             *
             *  Functions
             *
             * */
            /**
             * Checks if an axis is the outer axis in its dimension. Since
             * axes are placed outwards in order, the axis with the highest
             * index is the outermost axis.
             *
             * Example: If there are multiple x-axes at the top of the chart,
             * this function returns true if the axis supplied is the last
             * of the x-axes.
             *
             * @private
             *
             * @return {boolean}
             * True if the axis is the outermost axis in its dimension; false if
             * not.
             */
            GridAxisAdditions.prototype.isOuterAxis = function () {
                var axis = this.axis;
                var chart = axis.chart;
                var columnIndex = axis.grid.columnIndex;
                var columns = (axis.linkedParent && axis.linkedParent.grid.columns ||
                        axis.grid.columns);
                var parentAxis = columnIndex ? axis.linkedParent : axis;
                var thisIndex = -1,
                    lastIndex = 0;
                chart[axis.coll].forEach(function (otherAxis, index) {
                    if (otherAxis.side === axis.side && !otherAxis.options.isInternal) {
                        lastIndex = index;
                        if (otherAxis === parentAxis) {
                            // Get the index of the axis in question
                            thisIndex = index;
                        }
                    }
                });
                return (lastIndex === thisIndex &&
                    (isNumber(columnIndex) ? columns.length === columnIndex : true));
            };
            return GridAxisAdditions;
        }());
        /**
         * Axis with grid support.
         * @private
         * @class
         */
        var GridAxis = /** @class */ (function () {
                function GridAxis() {
                }
                /* *
                 *
                 *  Static Functions
                 *
                 * */
                /* eslint-disable valid-jsdoc */
                /**
                 * Extends axis class with grid support.
                 * @private
                 */
                GridAxis.compose = function (AxisClass) {
                    Axis.keepProps.push('grid');
                wrap(AxisClass.prototype, 'unsquish', GridAxis.wrapUnsquish);
                // Add event handlers
                addEvent(AxisClass, 'init', GridAxis.onInit);
                addEvent(AxisClass, 'afterGetOffset', GridAxis.onAfterGetOffset);
                addEvent(AxisClass, 'afterGetTitlePosition', GridAxis.onAfterGetTitlePosition);
                addEvent(AxisClass, 'afterInit', GridAxis.onAfterInit);
                addEvent(AxisClass, 'afterRender', GridAxis.onAfterRender);
                addEvent(AxisClass, 'afterSetAxisTranslation', GridAxis.onAfterSetAxisTranslation);
                addEvent(AxisClass, 'afterSetOptions', GridAxis.onAfterSetOptions);
                addEvent(AxisClass, 'afterSetOptions', GridAxis.onAfterSetOptions2);
                addEvent(AxisClass, 'afterSetScale', GridAxis.onAfterSetScale);
                addEvent(AxisClass, 'afterTickSize', GridAxis.onAfterTickSize);
                addEvent(AxisClass, 'trimTicks', GridAxis.onTrimTicks);
                addEvent(AxisClass, 'destroy', GridAxis.onDestroy);
            };
            /**
             * Handle columns and getOffset.
             * @private
             */
            GridAxis.onAfterGetOffset = function () {
                var grid = this.grid;
                (grid && grid.columns || []).forEach(function (column) {
                    column.getOffset();
                });
            };
            /**
             * @private
             */
            GridAxis.onAfterGetTitlePosition = function (e) {
                var axis = this;
                var options = axis.options;
                var gridOptions = options.grid || {};
                if (gridOptions.enabled === true) {
                    // compute anchor points for each of the title align options
                    var title = axis.axisTitle,
                        axisHeight = axis.height,
                        horiz = axis.horiz,
                        axisLeft = axis.left,
                        offset = axis.offset,
                        opposite = axis.opposite,
                        _a = axis.options.title,
                        axisTitleOptions = _a === void 0 ? {} : _a,
                        axisTop = axis.top,
                        axisWidth = axis.width;
                    var tickSize = axis.tickSize();
                    var titleWidth = title && title.getBBox().width;
                    var xOption = axisTitleOptions.x || 0;
                    var yOption = axisTitleOptions.y || 0;
                    var titleMargin = pick(axisTitleOptions.margin,
                        horiz ? 5 : 10);
                    var titleFontSize = axis.chart.renderer.fontMetrics(axisTitleOptions.style &&
                            axisTitleOptions.style.fontSize,
                        title).f;
                    var crispCorr = tickSize ? tickSize[0] / 2 : 0;
                    // TODO account for alignment
                    // the position in the perpendicular direction of the axis
                    var offAxis = ((horiz ? axisTop + axisHeight : axisLeft) +
                            (horiz ? 1 : -1) * // horizontal axis reverses the margin
                                (opposite ? -1 : 1) * // so does opposite axes
                                crispCorr +
                            (axis.side === GridAxis.Side.bottom ? titleFontSize : 0));
                    e.titlePosition.x = horiz ?
                        axisLeft - titleWidth / 2 - titleMargin + xOption :
                        offAxis + (opposite ? axisWidth : 0) + offset + xOption;
                    e.titlePosition.y = horiz ?
                        (offAxis -
                            (opposite ? axisHeight : 0) +
                            (opposite ? titleFontSize : -titleFontSize) / 2 +
                            offset +
                            yOption) :
                        axisTop - titleMargin + yOption;
                }
            };
            /**
             * @private
             */
            GridAxis.onAfterInit = function () {
                var axis = this;
                var chart = axis.chart,
                    _a = axis.options.grid,
                    gridOptions = _a === void 0 ? {} : _a,
                    userOptions = axis.userOptions;
                if (gridOptions.enabled) {
                    applyGridOptions(axis);
                    /* eslint-disable no-invalid-this */
                    // TODO: wrap the axis instead
                    wrap(axis, 'labelFormatter', function (proceed) {
                        var _a = this,
                            axis = _a.axis,
                            value = _a.value;
                        var tickPos = axis.tickPositions;
                        var series = (axis.isLinked ?
                                axis.linkedParent :
                                axis).series[0];
                        var isFirst = value === tickPos[0];
                        var isLast = value === tickPos[tickPos.length - 1];
                        var point = series && find(series.options.data,
                            function (p) {
                                return p[axis.isXAxis ? 'x' : 'y'] === value;
                        });
                        // Make additional properties available for the
                        // formatter
                        this.isFirst = isFirst;
                        this.isLast = isLast;
                        this.point = point;
                        // Call original labelFormatter
                        return proceed.call(this);
                    });
                    /* eslint-enable no-invalid-this */
                }
                if (gridOptions.columns) {
                    var columns = axis.grid.columns = [],
                        columnIndex = axis.grid.columnIndex = 0;
                    // Handle columns, each column is a grid axis
                    while (++columnIndex < gridOptions.columns.length) {
                        var columnOptions = merge(userOptions,
                            gridOptions.columns[gridOptions.columns.length - columnIndex - 1], {
                                linkedTo: 0,
                                // Force to behave like category axis
                                type: 'category',
                                // Disable by default the scrollbar on the grid axis
                                scrollbar: {
                                    enabled: false
                                }
                            });
                        delete columnOptions.grid.columns; // Prevent recursion
                        var column = new Axis(axis.chart,
                            columnOptions);
                        column.grid.isColumn = true;
                        column.grid.columnIndex = columnIndex;
                        // Remove column axis from chart axes array, and place it
                        // in the columns array.
                        erase(chart.axes, column);
                        erase(chart[axis.coll], column);
                        columns.push(column);
                    }
                }
            };
            /**
             * Draw an extra line on the far side of the outermost axis,
             * creating floor/roof/wall of a grid. And some padding.
             * ```
             * Make this:
             *             (axis.min) __________________________ (axis.max)
             *                           |    |    |    |    |
             * Into this:
             *             (axis.min) __________________________ (axis.max)
             *                        ___|____|____|____|____|__
             * ```
             * @private
             */
            GridAxis.onAfterRender = function () {
                var axis = this;
                var grid = axis.grid;
                var options = axis.options;
                var renderer = axis.chart.renderer;
                var gridOptions = options.grid || {};
                var yStartIndex,
                    yEndIndex,
                    xStartIndex,
                    xEndIndex;
                if (gridOptions.enabled === true) {
                    // @todo acutual label padding (top, bottom, left, right)
                    axis.maxLabelDimensions = axis.getMaxLabelDimensions(axis.ticks, axis.tickPositions);
                    // Remove right wall before rendering if updating
                    if (axis.rightWall) {
                        axis.rightWall.destroy();
                    }
                    /*
                    Draw an extra axis line on outer axes
                                >
                    Make this:    |______|______|______|___

                                > _________________________
                    Into this:    |______|______|______|__|
                                                            */
                    if (axis.grid && axis.grid.isOuterAxis() && axis.axisLine) {
                        var lineWidth = options.lineWidth;
                        if (lineWidth) {
                            var linePath = axis.getLinePath(lineWidth);
                            var startPoint = linePath[0];
                            var endPoint = linePath[1];
                            // Negate distance if top or left axis
                            // Subtract 1px to draw the line at the end of the tick
                            var tickLength = (axis.tickSize('tick') || [1])[0];
                            var distance = (tickLength - 1) * ((axis.side === GridAxis.Side.top ||
                                    axis.side === GridAxis.Side.left) ? -1 : 1);
                            // If axis is horizontal, reposition line path vertically
                            if (startPoint[0] === 'M' && endPoint[0] === 'L') {
                                if (axis.horiz) {
                                    startPoint[2] += distance;
                                    endPoint[2] += distance;
                                }
                                else {
                                    // If axis is vertical, reposition line path
                                    // horizontally
                                    startPoint[1] += distance;
                                    endPoint[1] += distance;
                                }
                            }
                            if (!axis.grid.axisLineExtra) {
                                axis.grid.axisLineExtra = renderer
                                    .path(linePath)
                                    .attr({
                                    zIndex: 7
                                })
                                    .addClass('highcharts-axis-line')
                                    .add(axis.axisGroup);
                                if (!renderer.styledMode) {
                                    axis.grid.axisLineExtra.attr({
                                        stroke: options.lineColor,
                                        'stroke-width': lineWidth
                                    });
                                }
                            }
                            else {
                                axis.grid.axisLineExtra.animate({
                                    d: linePath
                                });
                            }
                            // show or hide the line depending on
                            // options.showEmpty
                            axis.axisLine[axis.showAxis ? 'show' : 'hide'](true);
                        }
                    }
                    (grid && grid.columns || []).forEach(function (column) {
                        column.render();
                    });
                }
            };
            /**
             * @private
             */
            GridAxis.onAfterSetAxisTranslation = function () {
                var axis = this;
                var tickInfo = axis.tickPositions && axis.tickPositions.info;
                var options = axis.options;
                var gridOptions = options.grid || {};
                var userLabels = axis.userOptions.labels || {};
                if (axis.horiz) {
                    if (gridOptions.enabled === true) {
                        axis.series.forEach(function (series) {
                            series.options.pointRange = 0;
                        });
                    }
                    // Lower level time ticks, like hours or minutes, represent
                    // points in time and not ranges. These should be aligned
                    // left in the grid cell by default. The same applies to
                    // years of higher order.
                    if (tickInfo &&
                        options.dateTimeLabelFormats &&
                        options.labels &&
                        !defined(userLabels.align) &&
                        (options.dateTimeLabelFormats[tickInfo.unitName].range === false ||
                            tickInfo.count > 1 // years
                        )) {
                        options.labels.align = 'left';
                        if (!defined(userLabels.x)) {
                            options.labels.x = 3;
                        }
                    }
                }
            };
            /**
             * Creates a left and right wall on horizontal axes:
             * - Places leftmost tick at the start of the axis, to create a left
             *   wall
             * - Ensures that the rightmost tick is at the end of the axis, to
             *   create a right wall.
             * @private
             */
            GridAxis.onAfterSetOptions = function (e) {
                var options = this.options,
                    userOptions = e.userOptions,
                    gridAxisOptions,
                    gridOptions = ((options && isObject(options.grid)) ? options.grid : {});
                if (gridOptions.enabled === true) {
                    // Merge the user options into default grid axis options so
                    // that when a user option is set, it takes presedence.
                    gridAxisOptions = merge(true, {
                        className: ('highcharts-grid-axis ' + (userOptions.className || '')),
                        dateTimeLabelFormats: {
                            hour: {
                                list: ['%H:%M', '%H']
                            },
                            day: {
                                list: ['%A, %e. %B', '%a, %e. %b', '%E']
                            },
                            week: {
                                list: ['Week %W', 'W%W']
                            },
                            month: {
                                list: ['%B', '%b', '%o']
                            }
                        },
                        grid: {
                            borderWidth: 1
                        },
                        labels: {
                            padding: 2,
                            style: {
                                fontSize: '13px'
                            }
                        },
                        margin: 0,
                        title: {
                            text: null,
                            reserveSpace: false,
                            rotation: 0
                        },
                        // In a grid axis, only allow one unit of certain types,
                        // for example we shouln't have one grid cell spanning
                        // two days.
                        units: [[
                                'millisecond',
                                [1, 10, 100]
                            ], [
                                'second',
                                [1, 10]
                            ], [
                                'minute',
                                [1, 5, 15]
                            ], [
                                'hour',
                                [1, 6]
                            ], [
                                'day',
                                [1]
                            ], [
                                'week',
                                [1]
                            ], [
                                'month',
                                [1]
                            ], [
                                'year',
                                null
                            ]]
                    }, userOptions);
                    // X-axis specific options
                    if (this.coll === 'xAxis') {
                        // For linked axes, tickPixelInterval is used only if
                        // the tickPositioner below doesn't run or returns
                        // undefined (like multiple years)
                        if (defined(userOptions.linkedTo) &&
                            !defined(userOptions.tickPixelInterval)) {
                            gridAxisOptions.tickPixelInterval = 350;
                        }
                        // For the secondary grid axis, use the primary axis'
                        // tick intervals and return ticks one level higher.
                        if (
                        // Check for tick pixel interval in options
                        !defined(userOptions.tickPixelInterval) &&
                            // Only for linked axes
                            defined(userOptions.linkedTo) &&
                            !defined(userOptions.tickPositioner) &&
                            !defined(userOptions.tickInterval)) {
                            gridAxisOptions.tickPositioner = function (min, max) {
                                var parentInfo = (this.linkedParent &&
                                        this.linkedParent.tickPositions &&
                                        this.linkedParent.tickPositions.info);
                                if (parentInfo) {
                                    var unitIdx,
                                        count,
                                        unitName,
                                        i,
                                        units = gridAxisOptions.units,
                                        unitRange;
                                    for (i = 0; i < units.length; i++) {
                                        if (units[i][0] ===
                                            parentInfo.unitName) {
                                            unitIdx = i;
                                            break;
                                        }
                                    }
                                    // Get the first allowed count on the next
                                    // unit.
                                    if (units[unitIdx + 1]) {
                                        unitName = units[unitIdx + 1][0];
                                        count =
                                            (units[unitIdx + 1][1] || [1])[0];
                                        // In case the base X axis shows years, make
                                        // the secondary axis show ten times the
                                        // years (#11427)
                                    }
                                    else if (parentInfo.unitName === 'year') {
                                        unitName = 'year';
                                        count = parentInfo.count * 10;
                                    }
                                    unitRange = timeUnits[unitName];
                                    this.tickInterval = unitRange * count;
                                    return this.getTimeTicks({
                                        unitRange: unitRange,
                                        count: count,
                                        unitName: unitName
                                    }, min, max, this.options.startOfWeek);
                                }
                            };
                        }
                    }
                    // Now merge the combined options into the axis options
                    merge(true, this.options, gridAxisOptions);
                    if (this.horiz) {
                        /*               _________________________
                        Make this:    ___|_____|_____|_____|__|
                                        ^                     ^
                                        _________________________
                        Into this:    |_____|_____|_____|_____|
                                            ^                 ^    */
                        options.minPadding = pick(userOptions.minPadding, 0);
                        options.maxPadding = pick(userOptions.maxPadding, 0);
                    }
                    // If borderWidth is set, then use its value for tick and
                    // line width.
                    if (isNumber(options.grid.borderWidth)) {
                        options.tickWidth = options.lineWidth = gridOptions.borderWidth;
                    }
                }
            };
            /**
             * @private
             */
            GridAxis.onAfterSetOptions2 = function (e) {
                var axis = this;
                var userOptions = e.userOptions;
                var gridOptions = userOptions && userOptions.grid || {};
                var columns = gridOptions.columns;
                // Add column options to the parent axis. Children has their column
                // options set on init in onGridAxisAfterInit.
                if (gridOptions.enabled && columns) {
                    merge(true, axis.options, columns[columns.length - 1]);
                }
            };
            /**
             * Handle columns and setScale.
             * @private
             */
            GridAxis.onAfterSetScale = function () {
                var axis = this;
                (axis.grid.columns || []).forEach(function (column) {
                    column.setScale();
                });
            };
            /**
             * Draw vertical axis ticks extra long to create cell floors and roofs.
             * Overrides the tickLength for vertical axes.
             * @private
             */
            GridAxis.onAfterTickSize = function (e) {
                var defaultLeftAxisOptions = Axis.defaultLeftAxisOptions;
                var _a = this,
                    horiz = _a.horiz,
                    maxLabelDimensions = _a.maxLabelDimensions,
                    _b = _a.options.grid,
                    gridOptions = _b === void 0 ? {} : _b;
                if (gridOptions.enabled && maxLabelDimensions) {
                    var labelPadding = (Math.abs(defaultLeftAxisOptions.labels.x) * 2);
                    var distance = horiz ?
                            gridOptions.cellHeight || labelPadding + maxLabelDimensions.height :
                            labelPadding + maxLabelDimensions.width;
                    if (isArray(e.tickSize)) {
                        e.tickSize[0] = distance;
                    }
                    else {
                        e.tickSize = [distance, 0];
                    }
                }
            };
            /**
             * @private
             */
            GridAxis.onDestroy = function (e) {
                var grid = this.grid;
                (grid.columns || []).forEach(function (column) {
                    column.destroy(e.keepEvents);
                });
                grid.columns = void 0;
            };
            /**
             * Wraps axis init to draw cell walls on vertical axes.
             * @private
             */
            GridAxis.onInit = function (e) {
                var axis = this;
                var userOptions = e.userOptions || {};
                var gridOptions = userOptions.grid || {};
                if (gridOptions.enabled && defined(gridOptions.borderColor)) {
                    userOptions.tickColor = userOptions.lineColor = gridOptions.borderColor;
                }
                if (!axis.grid) {
                    axis.grid = new GridAxisAdditions(axis);
                }
            };
            /**
             * Makes tick labels which are usually ignored in a linked axis
             * displayed if they are within range of linkedParent.min.
             * ```
             *                        _____________________________
             *                        |   |       |       |       |
             * Make this:             |   |   2   |   3   |   4   |
             *                        |___|_______|_______|_______|
             *                          ^
             *                        _____________________________
             *                        |   |       |       |       |
             * Into this:             | 1 |   2   |   3   |   4   |
             *                        |___|_______|_______|_______|
             *                          ^
             * ```
             * @private
             * @todo Does this function do what the drawing says? Seems to affect
             *       ticks and not the labels directly?
             */
            GridAxis.onTrimTicks = function () {
                var axis = this;
                var options = axis.options;
                var gridOptions = options.grid || {};
                var categoryAxis = axis.categories;
                var tickPositions = axis.tickPositions;
                var firstPos = tickPositions[0];
                var lastPos = tickPositions[tickPositions.length - 1];
                var linkedMin = axis.linkedParent && axis.linkedParent.min;
                var linkedMax = axis.linkedParent && axis.linkedParent.max;
                var min = linkedMin || axis.min;
                var max = linkedMax || axis.max;
                var tickInterval = axis.tickInterval;
                var endMoreThanMin = (firstPos < min &&
                        firstPos + tickInterval > min);
                var startLessThanMax = (lastPos > max &&
                        lastPos - tickInterval < max);
                if (gridOptions.enabled === true &&
                    !categoryAxis &&
                    (axis.horiz || axis.isLinked)) {
                    if (endMoreThanMin && !options.startOnTick) {
                        tickPositions[0] = min;
                    }
                    if (startLessThanMax && !options.endOnTick) {
                        tickPositions[tickPositions.length - 1] = max;
                    }
                }
            };
            /**
             * Avoid altering tickInterval when reserving space.
             * @private
             */
            GridAxis.wrapUnsquish = function (proceed) {
                var axis = this;
                var _a = axis.options.grid,
                    gridOptions = _a === void 0 ? {} : _a;
                if (gridOptions.enabled === true && axis.categories) {
                    return axis.tickInterval;
                }
                return proceed.apply(axis, argsToArray(arguments));
            };
            return GridAxis;
        }());
        (function (GridAxis) {
            /**
             * Enum for which side the axis is on. Maps to axis.side.
             * @private
             */
            var Side;
            (function (Side) {
                Side[Side["top"] = 0] = "top";
                Side[Side["right"] = 1] = "right";
                Side[Side["bottom"] = 2] = "bottom";
                Side[Side["left"] = 3] = "left";
            })(Side = GridAxis.Side || (GridAxis.Side = {}));
        })(GridAxis || (GridAxis = {}));
        GridAxis.compose(Axis);

        return GridAxis;
    });
    _registerModule(_modules, 'Core/Axis/BrokenAxis.js', [_modules['Core/Axis/Axis.js'], _modules['Core/Globals.js'], _modules['Core/Utilities.js'], _modules['Extensions/Stacking.js']], function (Axis, H, U, StackItem) {
        /* *
         *
         *  (c) 2009-2020 Torstein Honsi
         *
         *  License: www.highcharts.com/license
         *
         *  !!!!!!! SOURCE GETS TRANSPILED BY TYPESCRIPT. EDIT TS FILE ONLY. !!!!!!!
         *
         * */
        var addEvent = U.addEvent,
            find = U.find,
            fireEvent = U.fireEvent,
            isArray = U.isArray,
            isNumber = U.isNumber,
            pick = U.pick;
        var Series = H.Series;
        /* eslint-disable valid-jsdoc */
        /**
         * Provides support for broken axes.
         * @private
         * @class
         */
        var BrokenAxisAdditions = /** @class */ (function () {
                /* *
                 *
                 *  Constructors
                 *
                 * */
                function BrokenAxisAdditions(axis) {
                    this.hasBreaks = false;
                this.axis = axis;
            }
            /* *
             *
             *  Static Functions
             *
             * */
            /**
             * @private
             */
            BrokenAxisAdditions.isInBreak = function (brk, val) {
                var ret,
                    repeat = brk.repeat || Infinity,
                    from = brk.from,
                    length = brk.to - brk.from,
                    test = (val >= from ?
                        (val - from) % repeat :
                        repeat - ((from - val) % repeat));
                if (!brk.inclusive) {
                    ret = test < length && test !== 0;
                }
                else {
                    ret = test <= length;
                }
                return ret;
            };
            /**
             * @private
             */
            BrokenAxisAdditions.lin2Val = function (val) {
                var axis = this;
                var brokenAxis = axis.brokenAxis;
                var breakArray = brokenAxis && brokenAxis.breakArray;
                if (!breakArray) {
                    return val;
                }
                var nval = val,
                    brk,
                    i;
                for (i = 0; i < breakArray.length; i++) {
                    brk = breakArray[i];
                    if (brk.from >= nval) {
                        break;
                    }
                    else if (brk.to < nval) {
                        nval += brk.len;
                    }
                    else if (BrokenAxisAdditions.isInBreak(brk, nval)) {
                        nval += brk.len;
                    }
                }
                return nval;
            };
            /**
             * @private
             */
            BrokenAxisAdditions.val2Lin = function (val) {
                var axis = this;
                var brokenAxis = axis.brokenAxis;
                var breakArray = brokenAxis && brokenAxis.breakArray;
                if (!breakArray) {
                    return val;
                }
                var nval = val,
                    brk,
                    i;
                for (i = 0; i < breakArray.length; i++) {
                    brk = breakArray[i];
                    if (brk.to <= val) {
                        nval -= brk.len;
                    }
                    else if (brk.from >= val) {
                        break;
                    }
                    else if (BrokenAxisAdditions.isInBreak(brk, val)) {
                        nval -= (val - brk.from);
                        break;
                    }
                }
                return nval;
            };
            /* *
             *
             *  Functions
             *
             * */
            /**
             * Returns the first break found where the x is larger then break.from and
             * smaller then break.to.
             *
             * @param {number} x
             * The number which should be within a break.
             *
             * @param {Array<Highcharts.XAxisBreaksOptions>} breaks
             * The array of breaks to search within.
             *
             * @return {Highcharts.XAxisBreaksOptions|undefined}
             * Returns the first break found that matches, returns false if no break is
             * found.
             */
            BrokenAxisAdditions.prototype.findBreakAt = function (x, breaks) {
                return find(breaks, function (b) {
                    return b.from < x && x < b.to;
                });
            };
            /**
             * @private
             */
            BrokenAxisAdditions.prototype.isInAnyBreak = function (val, testKeep) {
                var brokenAxis = this;
                var axis = brokenAxis.axis;
                var breaks = axis.options.breaks,
                    i = breaks && breaks.length,
                    inbrk,
                    keep,
                    ret;
                if (i) {
                    while (i--) {
                        if (BrokenAxisAdditions.isInBreak(breaks[i], val)) {
                            inbrk = true;
                            if (!keep) {
                                keep = pick(breaks[i].showPoints, !axis.isXAxis);
                            }
                        }
                    }
                    if (inbrk && testKeep) {
                        ret = inbrk && !keep;
                    }
                    else {
                        ret = inbrk;
                    }
                }
                return ret;
            };
            /**
             * Dynamically set or unset breaks in an axis. This function in lighter than
             * usin Axis.update, and it also preserves animation.
             *
             * @private
             * @function Highcharts.Axis#setBreaks
             *
             * @param {Array<Highcharts.XAxisBreaksOptions>} [breaks]
             *        The breaks to add. When `undefined` it removes existing breaks.
             *
             * @param {boolean} [redraw=true]
             *        Whether to redraw the chart immediately.
             *
             * @return {void}
             */
            BrokenAxisAdditions.prototype.setBreaks = function (breaks, redraw) {
                var brokenAxis = this;
                var axis = brokenAxis.axis;
                var hasBreaks = (isArray(breaks) && !!breaks.length);
                axis.isDirty = brokenAxis.hasBreaks !== hasBreaks;
                brokenAxis.hasBreaks = hasBreaks;
                axis.options.breaks = axis.userOptions.breaks = breaks;
                axis.forceRedraw = true; // Force recalculation in setScale
                // Recalculate series related to the axis.
                axis.series.forEach(function (series) {
                    series.isDirty = true;
                });
                if (!hasBreaks && axis.val2lin === BrokenAxisAdditions.val2Lin) {
                    // Revert to prototype functions
                    delete axis.val2lin;
                    delete axis.lin2val;
                }
                if (hasBreaks) {
                    axis.userOptions.ordinal = false;
                    axis.lin2val = BrokenAxisAdditions.lin2Val;
                    axis.val2lin = BrokenAxisAdditions.val2Lin;
                    axis.setExtremes = function (newMin, newMax, redraw, animation, eventArguments) {
                        // If trying to set extremes inside a break, extend min to
                        // after, and max to before the break ( #3857 )
                        if (brokenAxis.hasBreaks) {
                            var axisBreak,
                                breaks = this.options.breaks;
                            while ((axisBreak = brokenAxis.findBreakAt(newMin, breaks))) {
                                newMin = axisBreak.to;
                            }
                            while ((axisBreak = brokenAxis.findBreakAt(newMax, breaks))) {
                                newMax = axisBreak.from;
                            }
                            // If both min and max is within the same break.
                            if (newMax < newMin) {
                                newMax = newMin;
                            }
                        }
                        Axis.prototype.setExtremes.call(this, newMin, newMax, redraw, animation, eventArguments);
                    };
                    axis.setAxisTranslation = function (saveOld) {
                        Axis.prototype.setAxisTranslation.call(this, saveOld);
                        brokenAxis.unitLength = null;
                        if (brokenAxis.hasBreaks) {
                            var breaks = axis.options.breaks || [], 
                                // Temporary one:
                                breakArrayT = [],
                                breakArray = [],
                                length = 0,
                                inBrk,
                                repeat,
                                min = axis.userMin || axis.min,
                                max = axis.userMax || axis.max,
                                pointRangePadding = pick(axis.pointRangePadding, 0),
                                start,
                                i;
                            // Min & max check (#4247)
                            breaks.forEach(function (brk) {
                                repeat = brk.repeat || Infinity;
                                if (BrokenAxisAdditions.isInBreak(brk, min)) {
                                    min +=
                                        (brk.to % repeat) -
                                            (min % repeat);
                                }
                                if (BrokenAxisAdditions.isInBreak(brk, max)) {
                                    max -=
                                        (max % repeat) -
                                            (brk.from % repeat);
                                }
                            });
                            // Construct an array holding all breaks in the axis
                            breaks.forEach(function (brk) {
                                start = brk.from;
                                repeat = brk.repeat || Infinity;
                                while (start - repeat > min) {
                                    start -= repeat;
                                }
                                while (start < min) {
                                    start += repeat;
                                }
                                for (i = start; i < max; i += repeat) {
                                    breakArrayT.push({
                                        value: i,
                                        move: 'in'
                                    });
                                    breakArrayT.push({
                                        value: i + (brk.to - brk.from),
                                        move: 'out',
                                        size: brk.breakSize
                                    });
                                }
                            });
                            breakArrayT.sort(function (a, b) {
                                return ((a.value === b.value) ?
                                    ((a.move === 'in' ? 0 : 1) -
                                        (b.move === 'in' ? 0 : 1)) :
                                    a.value - b.value);
                            });
                            // Simplify the breaks
                            inBrk = 0;
                            start = min;
                            breakArrayT.forEach(function (brk) {
                                inBrk += (brk.move === 'in' ? 1 : -1);
                                if (inBrk === 1 && brk.move === 'in') {
                                    start = brk.value;
                                }
                                if (inBrk === 0) {
                                    breakArray.push({
                                        from: start,
                                        to: brk.value,
                                        len: brk.value - start - (brk.size || 0)
                                    });
                                    length += brk.value - start - (brk.size || 0);
                                }
                            });
                            /**
                             * HC <= 8 backwards compatibility, used by demo samples.
                             * @deprecated
                             * @private
                             * @requires modules/broken-axis
                             */
                            axis.breakArray = brokenAxis.breakArray = breakArray;
                            // Used with staticScale, and below the actual axis length,
                            // when breaks are substracted.
                            brokenAxis.unitLength = max - min - length + pointRangePadding;
                            fireEvent(axis, 'afterBreaks');
                            if (axis.staticScale) {
                                axis.transA = axis.staticScale;
                            }
                            else if (brokenAxis.unitLength) {
                                axis.transA *=
                                    (max - axis.min + pointRangePadding) /
                                        brokenAxis.unitLength;
                            }
                            if (pointRangePadding) {
                                axis.minPixelPadding =
                                    axis.transA * axis.minPointOffset;
                            }
                            axis.min = min;
                            axis.max = max;
                        }
                    };
                }
                if (pick(redraw, true)) {
                    axis.chart.redraw();
                }
            };
            return BrokenAxisAdditions;
        }());
        /**
         * Axis with support of broken data rows.
         * @private
         * @class
         */
        var BrokenAxis = /** @class */ (function () {
                function BrokenAxis() {
                }
                /**
                 * Adds support for broken axes.
                 * @private
                 */
                BrokenAxis.compose = function (AxisClass, SeriesClass) {
                    AxisClass.keepProps.push('brokenAxis');
                var seriesProto = Series.prototype;
                /**
                 * @private
                 */
                seriesProto.drawBreaks = function (axis, keys) {
                    var series = this,
                        points = series.points,
                        breaks,
                        threshold,
                        eventName,
                        y;
                    if (axis && // #5950
                        axis.brokenAxis &&
                        axis.brokenAxis.hasBreaks) {
                        var brokenAxis_1 = axis.brokenAxis;
                        keys.forEach(function (key) {
                            breaks = brokenAxis_1 && brokenAxis_1.breakArray || [];
                            threshold = axis.isXAxis ?
                                axis.min :
                                pick(series.options.threshold, axis.min);
                            points.forEach(function (point) {
                                y = pick(point['stack' + key.toUpperCase()], point[key]);
                                breaks.forEach(function (brk) {
                                    if (isNumber(threshold) && isNumber(y)) {
                                        eventName = false;
                                        if ((threshold < brk.from && y > brk.to) ||
                                            (threshold > brk.from && y < brk.from)) {
                                            eventName = 'pointBreak';
                                        }
                                        else if ((threshold < brk.from && y > brk.from && y < brk.to) ||
                                            (threshold > brk.from && y > brk.to && y < brk.from)) {
                                            eventName = 'pointInBreak';
                                        }
                                        if (eventName) {
                                            fireEvent(axis, eventName, { point: point, brk: brk });
                                        }
                                    }
                                });
                            });
                        });
                    }
                };
                /**
                 * Extend getGraphPath by identifying gaps in the data so that we can
                 * draw a gap in the line or area. This was moved from ordinal axis
                 * module to broken axis module as of #5045.
                 *
                 * @private
                 * @function Highcharts.Series#gappedPath
                 *
                 * @return {Highcharts.SVGPathArray}
                 * Gapped path
                 */
                seriesProto.gappedPath = function () {
                    var currentDataGrouping = this.currentDataGrouping,
                        groupingSize = currentDataGrouping && currentDataGrouping.gapSize,
                        gapSize = this.options.gapSize,
                        points = this.points.slice(),
                        i = points.length - 1,
                        yAxis = this.yAxis,
                        stack;
                    /**
                     * Defines when to display a gap in the graph, together with the
                     * [gapUnit](plotOptions.series.gapUnit) option.
                     *
                     * In case when `dataGrouping` is enabled, points can be grouped
                     * into a larger time span. This can make the grouped points to have
                     * a greater distance than the absolute value of `gapSize` property,
                     * which will result in disappearing graph completely. To prevent
                     * this situation the mentioned distance between grouped points is
                     * used instead of previously defined `gapSize`.
                     *
                     * In practice, this option is most often used to visualize gaps in
                     * time series. In a stock chart, intraday data is available for
                     * daytime hours, while gaps will appear in nights and weekends.
                     *
                     * @see [gapUnit](plotOptions.series.gapUnit)
                     * @see [xAxis.breaks](#xAxis.breaks)
                     *
                     * @sample {highstock} stock/plotoptions/series-gapsize/
                     *         Setting the gap size to 2 introduces gaps for weekends
                     *         in daily datasets.
                     *
                     * @type      {number}
                     * @default   0
                     * @product   highstock
                     * @requires  modules/broken-axis
                     * @apioption plotOptions.series.gapSize
                     */
                    /**
                     * Together with [gapSize](plotOptions.series.gapSize), this option
                     * defines where to draw gaps in the graph.
                     *
                     * When the `gapUnit` is `"relative"` (default), a gap size of 5
                     * means that if the distance between two points is greater than
                     * 5 times that of the two closest points, the graph will be broken.
                     *
                     * When the `gapUnit` is `"value"`, the gap is based on absolute
                     * axis values, which on a datetime axis is milliseconds. This also
                     * applies to the navigator series that inherits gap options from
                     * the base series.
                     *
                     * @see [gapSize](plotOptions.series.gapSize)
                     *
                     * @type       {string}
                     * @default    relative
                     * @since      5.0.13
                     * @product    highstock
                     * @validvalue ["relative", "value"]
                     * @requires   modules/broken-axis
                     * @apioption  plotOptions.series.gapUnit
                     */
                    if (gapSize && i > 0) { // #5008
                        // Gap unit is relative
                        if (this.options.gapUnit !== 'value') {
                            gapSize *= this.basePointRange;
                        }
                        // Setting a new gapSize in case dataGrouping is enabled (#7686)
                        if (groupingSize &&
                            groupingSize > gapSize &&
                            // Except when DG is forced (e.g. from other series)
                            // and has lower granularity than actual points (#11351)
                            groupingSize >= this.basePointRange) {
                            gapSize = groupingSize;
                        }
                        // extension for ordinal breaks
                        var current = void 0,
                            next = void 0;
                        while (i--) {
                            // Reassign next if it is not visible
                            if (!(next && next.visible !== false)) {
                                next = points[i + 1];
                            }
                            current = points[i];
                            // Skip iteration if one of the points is not visible
                            if (next.visible === false || current.visible === false) {
                                continue;
                            }
                            if (next.x - current.x > gapSize) {
                                var xRange = (current.x + next.x) / 2;
                                points.splice(// insert after this one
                                i + 1, 0, {
                                    isNull: true,
                                    x: xRange
                                });
                                // For stacked chart generate empty stack items, #6546
                                if (yAxis.stacking && this.options.stacking) {
                                    stack = yAxis.stacking.stacks[this.stackKey][xRange] =
                                        new StackItem(yAxis, yAxis.options
                                            .stackLabels, false, xRange, this.stack);
                                    stack.total = 0;
                                }
                            }
                            // Assign current to next for the upcoming iteration
                            next = current;
                        }
                    }
                    // Call base method
                    return this.getGraphPath(points);
                };
                /* eslint-disable no-invalid-this */
                addEvent(AxisClass, 'init', function () {
                    var axis = this;
                    if (!axis.brokenAxis) {
                        axis.brokenAxis = new BrokenAxisAdditions(axis);
                    }
                });
                addEvent(AxisClass, 'afterInit', function () {
                    if (typeof this.brokenAxis !== 'undefined') {
                        this.brokenAxis.setBreaks(this.options.breaks, false);
                    }
                });
                addEvent(AxisClass, 'afterSetTickPositions', function () {
                    var axis = this;
                    var brokenAxis = axis.brokenAxis;
                    if (brokenAxis &&
                        brokenAxis.hasBreaks) {
                        var tickPositions = this.tickPositions,
                            info = this.tickPositions.info,
                            newPositions = [],
                            i;
                        for (i = 0; i < tickPositions.length; i++) {
                            if (!brokenAxis.isInAnyBreak(tickPositions[i])) {
                                newPositions.push(tickPositions[i]);
                            }
                        }
                        this.tickPositions = newPositions;
                        this.tickPositions.info = info;
                    }
                });
                // Force Axis to be not-ordinal when breaks are defined
                addEvent(AxisClass, 'afterSetOptions', function () {
                    if (this.brokenAxis && this.brokenAxis.hasBreaks) {
                        this.options.ordinal = false;
                    }
                });
                addEvent(SeriesClass, 'afterGeneratePoints', function () {
                    var _a = this,
                        isDirty = _a.isDirty,
                        connectNulls = _a.options.connectNulls,
                        points = _a.points,
                        xAxis = _a.xAxis,
                        yAxis = _a.yAxis;
                    // Set, or reset visibility of the points. Axis.setBreaks marks the
                    // series as isDirty
                    if (isDirty) {
                        var i = points.length;
                        while (i--) {
                            var point = points[i];
                            // Respect nulls inside the break (#4275)
                            var nullGap = point.y === null && connectNulls === false;
                            var isPointInBreak = (!nullGap && ((xAxis &&
                                    xAxis.brokenAxis &&
                                    xAxis.brokenAxis.isInAnyBreak(point.x,
                                true)) || (yAxis &&
                                    yAxis.brokenAxis &&
                                    yAxis.brokenAxis.isInAnyBreak(point.y,
                                true))));
                            // Set point.visible if in any break.
                            // If not in break, reset visible to original value.
                            point.visible = isPointInBreak ?
                                false :
                                point.options.visible !== false;
                        }
                    }
                });
                addEvent(SeriesClass, 'afterRender', function drawPointsWrapped() {
                    this.drawBreaks(this.xAxis, ['x']);
                    this.drawBreaks(this.yAxis, pick(this.pointArrayMap, ['y']));
                });
            };
            return BrokenAxis;
        }());
        BrokenAxis.compose(Axis, Series); // @todo remove automatism

        return BrokenAxis;
    });
    _registerModule(_modules, 'Core/Axis/TreeGridAxis.js', [_modules['Core/Axis/Axis.js'], _modules['Core/Axis/Tick.js'], _modules['Gantt/Tree.js'], _modules['Core/Axis/TreeGridTick.js'], _modules['Mixins/TreeSeries.js'], _modules['Core/Utilities.js']], function (Axis, Tick, Tree, TreeGridTick, mixinTreeSeries, U) {
        /* *
         *
         *  (c) 2016 Highsoft AS
         *  Authors: Jon Arild Nygard
         *
         *  License: www.highcharts.com/license
         *
         *  !!!!!!! SOURCE GETS TRANSPILED BY TYPESCRIPT. EDIT TS FILE ONLY. !!!!!!!
         *
         * */
        var getLevelOptions = mixinTreeSeries.getLevelOptions;
        var addEvent = U.addEvent,
            find = U.find,
            fireEvent = U.fireEvent,
            isNumber = U.isNumber,
            isObject = U.isObject,
            isString = U.isString,
            merge = U.merge,
            pick = U.pick,
            wrap = U.wrap;
        /**
         * @private
         */
        var TreeGridAxis;
        (function (TreeGridAxis) {
            /* *
             *
             *  Interfaces
             *
             * */
            /* *
             *
             *  Variables
             *
             * */
            var applied = false;
            /* *
             *
             *  Functions
             *
             * */
            /**
             * @private
             */
            function compose(AxisClass) {
                if (!applied) {
                    wrap(AxisClass.prototype, 'generateTick', wrapGenerateTick);
                    wrap(AxisClass.prototype, 'getMaxLabelDimensions', wrapGetMaxLabelDimensions);
                    wrap(AxisClass.prototype, 'init', wrapInit);
                    wrap(AxisClass.prototype, 'setTickInterval', wrapSetTickInterval);
                    TreeGridTick.compose(Tick);
                    applied = true;
                }
            }
            TreeGridAxis.compose = compose;
            /**
             * @private
             */
            function getBreakFromNode(node, max) {
                var from = node.collapseStart || 0,
                    to = node.collapseEnd || 0;
                // In broken-axis, the axis.max is minimized until it is not within a
                // break. Therefore, if break.to is larger than axis.max, the axis.to
                // should not add the 0.5 axis.tickMarkOffset, to avoid adding a break
                // larger than axis.max.
                // TODO consider simplifying broken-axis and this might solve itself
                if (to >= max) {
                    from -= 0.5;
                }
                return {
                    from: from,
                    to: to,
                    showPoints: false
                };
            }
            /**
             * Creates a tree structure of the data, and the treegrid. Calculates
             * categories, and y-values of points based on the tree.
             *
             * @private
             * @function getTreeGridFromData
             *
             * @param {Array<Highcharts.GanttPointOptions>} data
             * All the data points to display in the axis.
             *
             * @param {boolean} uniqueNames
             * Wether or not the data node with the same name should share grid cell. If
             * true they do share cell. False by default.
             *
             * @param {number} numberOfSeries
             *
             * @return {object}
             * Returns an object containing categories, mapOfIdToNode,
             * mapOfPosToGridNode, and tree.
             *
             * @todo There should be only one point per line.
             * @todo It should be optional to have one category per point, or merge
             *       cells
             * @todo Add unit-tests.
             */
            function getTreeGridFromData(data, uniqueNames, numberOfSeries) {
                var categories = [],
                    collapsedNodes = [],
                    mapOfIdToNode = {},
                    mapOfPosToGridNode = {},
                    posIterator = -1,
                    uniqueNamesEnabled = typeof uniqueNames === 'boolean' ? uniqueNames : false,
                    tree;
                // Build the tree from the series data.
                var treeParams = {
                        // After the children has been created.
                        after: function (node) {
                            var gridNode = mapOfPosToGridNode[node.pos],
                    height = 0,
                    descendants = 0;
                        gridNode.children.forEach(function (child) {
                            descendants += (child.descendants || 0) + 1;
                            height = Math.max((child.height || 0) + 1, height);
                        });
                        gridNode.descendants = descendants;
                        gridNode.height = height;
                        if (gridNode.collapsed) {
                            collapsedNodes.push(gridNode);
                        }
                    },
                    // Before the children has been created.
                    before: function (node) {
                        var data = isObject(node.data,
                            true) ? node.data : {},
                            name = isString(data.name) ? data.name : '',
                            parentNode = mapOfIdToNode[node.parent],
                            parentGridNode = (isObject(parentNode,
                            true) ?
                                mapOfPosToGridNode[parentNode.pos] :
                                null),
                            hasSameName = function (x) {
                                return x.name === name;
                        }, gridNode, pos;
                        // If not unique names, look for sibling node with the same name
                        if (uniqueNamesEnabled &&
                            isObject(parentGridNode, true) &&
                            !!(gridNode = find(parentGridNode.children, hasSameName))) {
                            // If there is a gridNode with the same name, reuse position
                            pos = gridNode.pos;
                            // Add data node to list of nodes in the grid node.
                            gridNode.nodes.push(node);
                        }
                        else {
                            // If it is a new grid node, increment position.
                            pos = posIterator++;
                        }
                        // Add new grid node to map.
                        if (!mapOfPosToGridNode[pos]) {
                            mapOfPosToGridNode[pos] = gridNode = {
                                depth: parentGridNode ? parentGridNode.depth + 1 : 0,
                                name: name,
                                nodes: [node],
                                children: [],
                                pos: pos
                            };
                            // If not root, then add name to categories.
                            if (pos !== -1) {
                                categories.push(name);
                            }
                            // Add name to list of children.
                            if (isObject(parentGridNode, true)) {
                                parentGridNode.children.push(gridNode);
                            }
                        }
                        // Add data node to map
                        if (isString(node.id)) {
                            mapOfIdToNode[node.id] = node;
                        }
                        // If one of the points are collapsed, then start the grid node
                        // in collapsed state.
                        if (gridNode &&
                            data.collapsed === true) {
                            gridNode.collapsed = true;
                        }
                        // Assign pos to data node
                        node.pos = pos;
                    }
                };
                var updateYValuesAndTickPos = function (map,
                    numberOfSeries) {
                        var setValues = function (gridNode,
                    start,
                    result) {
                            var nodes = gridNode.nodes,
                    end = start + (start === -1 ? 0 : numberOfSeries - 1),
                    diff = (end - start) / 2,
                    padding = 0.5,
                    pos = start + diff;
                        nodes.forEach(function (node) {
                            var data = node.data;
                            if (isObject(data, true)) {
                                // Update point
                                data.y = start + (data.seriesIndex || 0);
                                // Remove the property once used
                                delete data.seriesIndex;
                            }
                            node.pos = pos;
                        });
                        result[pos] = gridNode;
                        gridNode.pos = pos;
                        gridNode.tickmarkOffset = diff + padding;
                        gridNode.collapseStart = end + padding;
                        gridNode.children.forEach(function (child) {
                            setValues(child, end + 1, result);
                            end = (child.collapseEnd || 0) - padding;
                        });
                        // Set collapseEnd to the end of the last child node.
                        gridNode.collapseEnd = end + padding;
                        return result;
                    };
                    return setValues(map['-1'], -1, {});
                };
                // Create tree from data
                tree = Tree.getTree(data, treeParams);
                // Update y values of data, and set calculate tick positions.
                mapOfPosToGridNode = updateYValuesAndTickPos(mapOfPosToGridNode, numberOfSeries);
                // Return the resulting data.
                return {
                    categories: categories,
                    mapOfIdToNode: mapOfIdToNode,
                    mapOfPosToGridNode: mapOfPosToGridNode,
                    collapsedNodes: collapsedNodes,
                    tree: tree
                };
            }
            /**
             * Builds the tree of categories and calculates its positions.
             * @private
             * @param {object} e Event object
             * @param {object} e.target The chart instance which the event was fired on.
             * @param {object[]} e.target.axes The axes of the chart.
             */
            function onBeforeRender(e) {
                var chart = e.target,
                    axes = chart.axes;
                axes.filter(function (axis) {
                    return axis.options.type === 'treegrid';
                }).forEach(function (axis) {
                    var options = axis.options || {},
                        labelOptions = options.labels,
                        uniqueNames = options.uniqueNames,
                        numberOfSeries = 0,
                        isDirty,
                        data,
                        treeGrid,
                        max = options.max;
                    // Check whether any of series is rendering for the first time,
                    // visibility has changed, or its data is dirty,
                    // and only then update. #10570, #10580
                    // Also check if mapOfPosToGridNode exists. #10887
                    isDirty = (!axis.treeGrid.mapOfPosToGridNode ||
                        axis.series.some(function (series) {
                            return !series.hasRendered ||
                                series.isDirtyData ||
                                series.isDirty;
                        }));
                    if (isDirty) {
                        // Concatenate data from all series assigned to this axis.
                        data = axis.series.reduce(function (arr, s) {
                            if (s.visible) {
                                // Push all data to array
                                (s.options.data || []).forEach(function (data) {
                                    if (isObject(data, true)) {
                                        // Set series index on data. Removed again
                                        // after use.
                                        data.seriesIndex = numberOfSeries;
                                        arr.push(data);
                                    }
                                });
                                // Increment series index
                                if (uniqueNames === true) {
                                    numberOfSeries++;
                                }
                            }
                            return arr;
                        }, []);
                        // If max is higher than set data - add a
                        // dummy data to render categories #10779
                        if (max && data.length < max) {
                            for (var i = data.length; i <= max; i++) {
                                data.push({
                                    // Use the zero-width character
                                    // to avoid conflict with uniqueNames
                                    name: i + '\u200B'
                                });
                            }
                        }
                        // setScale is fired after all the series is initialized,
                        // which is an ideal time to update the axis.categories.
                        treeGrid = getTreeGridFromData(data, uniqueNames || false, (uniqueNames === true) ? numberOfSeries : 1);
                        // Assign values to the axis.
                        axis.categories = treeGrid.categories;
                        axis.treeGrid.mapOfPosToGridNode = treeGrid.mapOfPosToGridNode;
                        axis.hasNames = true;
                        axis.treeGrid.tree = treeGrid.tree;
                        // Update yData now that we have calculated the y values
                        axis.series.forEach(function (series) {
                            var data = (series.options.data || []).map(function (d) {
                                    return isObject(d,
                                true) ? merge(d) : d;
                            });
                            // Avoid destroying points when series is not visible
                            if (series.visible) {
                                series.setData(data, false);
                            }
                        });
                        // Calculate the label options for each level in the tree.
                        axis.treeGrid.mapOptionsToLevel =
                            getLevelOptions({
                                defaults: labelOptions,
                                from: 1,
                                levels: labelOptions && labelOptions.levels,
                                to: axis.treeGrid.tree && axis.treeGrid.tree.height
                            });
                        // Setting initial collapsed nodes
                        if (e.type === 'beforeRender') {
                            axis.treeGrid.collapsedNodes = treeGrid.collapsedNodes;
                        }
                    }
                });
            }
            /**
             * Generates a tick for initial positioning.
             *
             * @private
             * @function Highcharts.GridAxis#generateTick
             *
             * @param {Function} proceed
             * The original generateTick function.
             *
             * @param {number} pos
             * The tick position in axis values.
             */
            function wrapGenerateTick(proceed, pos) {
                var axis = this,
                    mapOptionsToLevel = axis.treeGrid.mapOptionsToLevel || {},
                    isTreeGrid = axis.options.type === 'treegrid',
                    ticks = axis.ticks;
                var tick = ticks[pos],
                    levelOptions,
                    options,
                    gridNode;
                if (isTreeGrid &&
                    axis.treeGrid.mapOfPosToGridNode) {
                    gridNode = axis.treeGrid.mapOfPosToGridNode[pos];
                    levelOptions = mapOptionsToLevel[gridNode.depth];
                    if (levelOptions) {
                        options = {
                            labels: levelOptions
                        };
                    }
                    if (!tick) {
                        ticks[pos] = tick =
                            new Tick(axis, pos, void 0, void 0, {
                                category: gridNode.name,
                                tickmarkOffset: gridNode.tickmarkOffset,
                                options: options
                            });
                    }
                    else {
                        // update labels depending on tick interval
                        tick.parameters.category = gridNode.name;
                        tick.options = options;
                        tick.addLabel();
                    }
                }
                else {
                    proceed.apply(axis, Array.prototype.slice.call(arguments, 1));
                }
            }
            /**
             * Override to add indentation to axis.maxLabelDimensions.
             *
             * @private
             * @function Highcharts.GridAxis#getMaxLabelDimensions
             *
             * @param {Function} proceed
             * The original function
             */
            function wrapGetMaxLabelDimensions(proceed) {
                var axis = this,
                    options = axis.options,
                    labelOptions = options && options.labels,
                    indentation = (labelOptions && isNumber(labelOptions.indentation) ?
                        labelOptions.indentation :
                        0),
                    retVal = proceed.apply(axis,
                    Array.prototype.slice.call(arguments, 1)),
                    isTreeGrid = axis.options.type === 'treegrid';
                var treeDepth;
                if (isTreeGrid && axis.treeGrid.mapOfPosToGridNode) {
                    treeDepth = axis.treeGrid.mapOfPosToGridNode[-1].height || 0;
                    retVal.width += indentation * (treeDepth - 1);
                }
                return retVal;
            }
            /**
             * @private
             */
            function wrapInit(proceed, chart, userOptions) {
                var axis = this,
                    isTreeGrid = userOptions.type === 'treegrid';
                if (!axis.treeGrid) {
                    axis.treeGrid = new Additions(axis);
                }
                // Set default and forced options for TreeGrid
                if (isTreeGrid) {
                    // Add event for updating the categories of a treegrid.
                    // NOTE Preferably these events should be set on the axis.
                    addEvent(chart, 'beforeRender', onBeforeRender);
                    addEvent(chart, 'beforeRedraw', onBeforeRender);
                    // Add new collapsed nodes on addseries
                    addEvent(chart, 'addSeries', function (e) {
                        if (e.options.data) {
                            var treeGrid = getTreeGridFromData(e.options.data,
                                userOptions.uniqueNames || false, 1);
                            axis.treeGrid.collapsedNodes = (axis.treeGrid.collapsedNodes || []).concat(treeGrid.collapsedNodes);
                        }
                    });
                    // Collapse all nodes in axis.treegrid.collapsednodes
                    // where collapsed equals true.
                    addEvent(axis, 'foundExtremes', function () {
                        if (axis.treeGrid.collapsedNodes) {
                            axis.treeGrid.collapsedNodes.forEach(function (node) {
                                var breaks = axis.treeGrid.collapse(node);
                                if (axis.brokenAxis) {
                                    axis.brokenAxis.setBreaks(breaks, false);
                                    // remove the node from the axis collapsedNodes
                                    if (axis.treeGrid.collapsedNodes) {
                                        axis.treeGrid.collapsedNodes = axis.treeGrid.collapsedNodes.filter(function (n) {
                                            return node.collapseStart !== n.collapseStart ||
                                                node.collapseEnd !== n.collapseEnd;
                                        });
                                    }
                                }
                            });
                        }
                    });
                    // If staticScale is not defined on the yAxis
                    // and chart height is set, set axis.isDirty
                    // to ensure collapsing works (#12012)
                    addEvent(axis, 'afterBreaks', function () {
                        var _a;
                        if (axis.coll === 'yAxis' && !axis.staticScale && ((_a = axis.chart.options.chart) === null || _a === void 0 ? void 0 : _a.height)) {
                            axis.isDirty = true;
                        }
                    });
                    userOptions = merge({
                        // Default options
                        grid: {
                            enabled: true
                        },
                        // TODO: add support for align in treegrid.
                        labels: {
                            align: 'left',
                            /**
                            * Set options on specific levels in a tree grid axis. Takes
                            * precedence over labels options.
                            *
                            * @sample {gantt} gantt/treegrid-axis/labels-levels
                            *         Levels on TreeGrid Labels
                            *
                            * @type      {Array<*>}
                            * @product   gantt
                            * @apioption yAxis.labels.levels
                            *
                            * @private
                            */
                            levels: [{
                                    /**
                                    * Specify the level which the options within this object
                                    * applies to.
                                    *
                                    * @type      {number}
                                    * @product   gantt
                                    * @apioption yAxis.labels.levels.level
                                    *
                                    * @private
                                    */
                                    level: void 0
                                }, {
                                    level: 1,
                                    /**
                                     * @type      {Highcharts.CSSObject}
                                     * @product   gantt
                                     * @apioption yAxis.labels.levels.style
                                     *
                                     * @private
                                     */
                                    style: {
                                        /** @ignore-option */
                                        fontWeight: 'bold'
                                    }
                                }],
                            /**
                             * The symbol for the collapse and expand icon in a
                             * treegrid.
                             *
                             * @product      gantt
                             * @optionparent yAxis.labels.symbol
                             *
                             * @private
                             */
                            symbol: {
                                /**
                                 * The symbol type. Points to a definition function in
                                 * the `Highcharts.Renderer.symbols` collection.
                                 *
                                 * @type {Highcharts.SymbolKeyValue}
                                 *
                                 * @private
                                 */
                                type: 'triangle',
                                x: -5,
                                y: -5,
                                height: 10,
                                width: 10,
                                padding: 5
                            }
                        },
                        uniqueNames: false
                    }, userOptions, {
                        // Forced options
                        reversed: true,
                        // grid.columns is not supported in treegrid
                        grid: {
                            columns: void 0
                        }
                    });
                }
                // Now apply the original function with the original arguments,
                // which are sliced off this function's arguments
                proceed.apply(axis, [chart, userOptions]);
                if (isTreeGrid) {
                    axis.hasNames = true;
                    axis.options.showLastLabel = true;
                }
            }
            /**
             * Set the tick positions, tickInterval, axis min and max.
             *
             * @private
             * @function Highcharts.GridAxis#setTickInterval
             *
             * @param {Function} proceed
             * The original setTickInterval function.
             */
            function wrapSetTickInterval(proceed) {
                var axis = this,
                    options = axis.options,
                    isTreeGrid = options.type === 'treegrid';
                if (isTreeGrid) {
                    axis.min = pick(axis.userMin, options.min, axis.dataMin);
                    axis.max = pick(axis.userMax, options.max, axis.dataMax);
                    fireEvent(axis, 'foundExtremes');
                    // setAxisTranslation modifies the min and max according to
                    // axis breaks.
                    axis.setAxisTranslation(true);
                    axis.tickmarkOffset = 0.5;
                    axis.tickInterval = 1;
                    axis.tickPositions = axis.treeGrid.mapOfPosToGridNode ?
                        axis.treeGrid.getTickPositions() :
                        [];
                }
                else {
                    proceed.apply(axis, Array.prototype.slice.call(arguments, 1));
                }
            }
            /* *
             *
             *  Classes
             *
             * */
            /**
             * @private
             * @class
             */
            var Additions = /** @class */ (function () {
                    /* *
                     *
                     *  Constructors
                     *
                     * */
                    /**
                     * @private
                     */
                    function Additions(axis) {
                        this.axis = axis;
                }
                /* *
                 *
                 *  Functions
                 *
                 * */
                /**
                 * Calculates the new axis breaks to collapse a node.
                 *
                 * @private
                 *
                 * @param {Highcharts.Axis} axis
                 * The axis to check against.
                 *
                 * @param {Highcharts.GridNode} node
                 * The node to collapse.
                 *
                 * @param {number} pos
                 * The tick position to collapse.
                 *
                 * @return {Array<object>}
                 * Returns an array of the new breaks for the axis.
                 */
                Additions.prototype.collapse = function (node) {
                    var axis = this.axis,
                        breaks = (axis.options.breaks || []),
                        obj = getBreakFromNode(node,
                        axis.max);
                    breaks.push(obj);
                    return breaks;
                };
                /**
                 * Calculates the new axis breaks to expand a node.
                 *
                 * @private
                 *
                 * @param {Highcharts.Axis} axis
                 * The axis to check against.
                 *
                 * @param {Highcharts.GridNode} node
                 * The node to expand.
                 *
                 * @param {number} pos
                 * The tick position to expand.
                 *
                 * @return {Array<object>}
                 * Returns an array of the new breaks for the axis.
                 */
                Additions.prototype.expand = function (node) {
                    var axis = this.axis,
                        breaks = (axis.options.breaks || []),
                        obj = getBreakFromNode(node,
                        axis.max);
                    // Remove the break from the axis breaks array.
                    return breaks.reduce(function (arr, b) {
                        if (b.to !== obj.to || b.from !== obj.from) {
                            arr.push(b);
                        }
                        return arr;
                    }, []);
                };
                /**
                 * Creates a list of positions for the ticks on the axis. Filters out
                 * positions that are outside min and max, or is inside an axis break.
                 *
                 * @private
                 *
                 * @return {Array<number>}
                 * List of positions.
                 */
                Additions.prototype.getTickPositions = function () {
                    var axis = this.axis;
                    return Object.keys(axis.treeGrid.mapOfPosToGridNode || {}).reduce(function (arr, key) {
                        var pos = +key;
                        if (axis.min <= pos &&
                            axis.max >= pos &&
                            !(axis.brokenAxis && axis.brokenAxis.isInAnyBreak(pos))) {
                            arr.push(pos);
                        }
                        return arr;
                    }, []);
                };
                /**
                 * Check if a node is collapsed.
                 *
                 * @private
                 *
                 * @param {Highcharts.Axis} axis
                 * The axis to check against.
                 *
                 * @param {object} node
                 * The node to check if is collapsed.
                 *
                 * @param {number} pos
                 * The tick position to collapse.
                 *
                 * @return {boolean}
                 * Returns true if collapsed, false if expanded.
                 */
                Additions.prototype.isCollapsed = function (node) {
                    var axis = this.axis,
                        breaks = (axis.options.breaks || []),
                        obj = getBreakFromNode(node,
                        axis.max);
                    return breaks.some(function (b) {
                        return b.from === obj.from && b.to === obj.to;
                    });
                };
                /**
                 * Calculates the new axis breaks after toggling the collapse/expand
                 * state of a node. If it is collapsed it will be expanded, and if it is
                 * exapended it will be collapsed.
                 *
                 * @private
                 *
                 * @param {Highcharts.Axis} axis
                 * The axis to check against.
                 *
                 * @param {Highcharts.GridNode} node
                 * The node to toggle.
                 *
                 * @return {Array<object>}
                 * Returns an array of the new breaks for the axis.
                 */
                Additions.prototype.toggleCollapse = function (node) {
                    return (this.isCollapsed(node) ?
                        this.expand(node) :
                        this.collapse(node));
                };
                return Additions;
            }());
            TreeGridAxis.Additions = Additions;
        })(TreeGridAxis || (TreeGridAxis = {}));
        // Make utility functions available for testing.
        Axis.prototype.utils = {
            getNode: Tree.getNode
        };
        TreeGridAxis.compose(Axis);

        return TreeGridAxis;
    });
    _registerModule(_modules, 'masters/modules/treegrid.src.js', [], function () {


    });
}));