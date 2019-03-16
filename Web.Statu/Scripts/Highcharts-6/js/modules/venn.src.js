/**
 * @license Highcharts JS v7.0.0 (2018-12-11)
 *
 * (c) 2017-2018 Highsoft AS
 * Authors: Jon Arild Nygard
 *
 * License: www.highcharts.com/license
 */
'use strict';
(function (factory) {
	if (typeof module === 'object' && module.exports) {
		module.exports = factory;
	} else if (typeof define === 'function' && define.amd) {
		define(function () {
			return factory;
		});
	} else {
		factory(typeof Highcharts !== 'undefined' ? Highcharts : undefined);
	}
}(function (Highcharts) {
	var draw = (function () {
		var isFn = function (x) {
		    return typeof x === 'function';
		};

		/**
		 * Handles the drawing of a point.
		 *
		 * @private
		 * @function draw
		 *
		 * @param {object} params
		 *        Parameters.
		 *
		 * @todo
		 * - add type checking.
		 */
		var draw = function draw(params) {
		    var point = this,
		        graphic = point.graphic,
		        animatableAttribs = params.animatableAttribs,
		        onComplete = params.onComplete,
		        css = params.css,
		        renderer = params.renderer;

		    if (point.shouldDraw()) {
		        if (!graphic) {
		            point.graphic = graphic =
		                renderer[params.shapeType](params.shapeArgs).add(params.group);
		        }
		        graphic
		            .css(css)
		            .attr(params.attribs)
		            .animate(
		                animatableAttribs,
		                params.isNew ? false : undefined,
		                onComplete
		            );
		    } else if (graphic) {
		        graphic.animate(animatableAttribs, undefined, function () {
		            point.graphic = graphic = graphic.destroy();
		            if (isFn(onComplete)) {
		                onComplete();
		            }
		        });
		    }
		    if (graphic) {
		        graphic.addClass(point.getClassName(), true);
		    }
		};


		return draw;
	}());
	var geometry = (function () {
		/**
		 * Calculates the center between a list of points.
		 *
		 * @param {array} points A list of points to calculate the center of.
		 */
		var getCenterOfPoints = function getCenterOfPoints(points) {
		    var sum = points.reduce(function (sum, point) {
		        sum.x += point.x;
		        sum.y += point.y;
		        return sum;
		    }, { x: 0, y: 0 });

		    return {
		        x: sum.x / points.length,
		        y: sum.y / points.length
		    };
		};

		/**
		 * Calculates the distance between two points based on their x and y
		 * coordinates.
		 *
		 * @param {object} p1 The x and y coordinates of the first point.
		 * @param {object} p2 The x and y coordinates of the second point.
		 * @returns {number} Returns the distance between the points.
		 */
		var getDistanceBetweenPoints = function getDistanceBetweenPoints(p1, p2) {
		    return Math.sqrt(Math.pow(p2.x - p1.x, 2) + Math.pow(p2.y - p1.y, 2));
		};

		/**
		 * Calculates the angle between two points.
		 *
		 * TODO: add unit tests.
		 *
		 * @param {object} p1 The first point.
		 * @param {object} p2 The second point.
		 * @returns {number} Returns the angle in radians.
		 */
		var getAngleBetweenPoints = function getAngleBetweenPoints(p1, p2) {
		    return Math.atan2(p2.x - p1.x, p2.y - p1.y);
		};

		var geometry = {
		    getAngleBetweenPoints: getAngleBetweenPoints,
		    getCenterOfPoints: getCenterOfPoints,
		    getDistanceBetweenPoints: getDistanceBetweenPoints
		};


		return geometry;
	}());
	var geometryCircles = (function (geometry) {
		var getAngleBetweenPoints = geometry.getAngleBetweenPoints,
		    getCenterOfPoints = geometry.getCenterOfPoints,
		    getDistanceBetweenPoints = geometry.getDistanceBetweenPoints;

		var round = function round(x, decimals) {
		    var a = Math.pow(10, decimals);
		    return Math.round(x * a) / a;
		};

		/**
		 * Calculates the area of overlap between two circles based on their radiuses
		 * and the distance between them.
		 * See http://mathworld.wolfram.com/Circle-CircleIntersection.html
		 *
		 * @param {number} r1 Radius of the first circle.
		 * @param {number} r2 Radius of the second circle.
		 * @param {number} d The distance between the two circles.
		 * @returns {number} Returns the area of overlap between the two circles.
		 */
		var getOverlapBetweenCircles =
		function getOverlapBetweenCircles(r1, r2, d) {
		    var overlap = 0;

		    // If the distance is larger than the sum of the radiuses then the circles
		    // does not overlap.
		    if (d < r1 + r2) {
		        var r1Square = r1 * r1,
		            r2Square = r2 * r2;

		        if (d <= Math.abs(r2 - r1)) {
		            // If the circles are completely overlapping, then the overlap
		            // equals the area of the smallest circle.
		            overlap = Math.PI * Math.min(r1Square, r2Square);
		        } else {
		                // d^2 - r^2 + R^2 / 2d
		            var x = (r1Square - r2Square + d * d) / (2 * d),
		                // y^2 = R^2 - x^2
		                y = Math.sqrt(r1Square - x * x);

		            overlap = (
		                r1Square * Math.asin(y / r1) +
		                r2Square * Math.asin(y / r2) -
		                y * (x + Math.sqrt(x * x + r2Square - r1Square))
		            );
		        }
		        // Round the result to two decimals.
		        overlap = round(overlap, 14);
		    }
		    return overlap;
		};

		/**
		 * Calculates the intersection points of two circles.
		 *
		 * NOTE: does not handle floating errors well.
		 *
		 * @param {object} c1 The first circle.s
		 * @param {object} c2 The second sircle.
		 * @returns {array} Returns the resulting intersection points.
		 */
		var getCircleCircleIntersection =
		function getCircleCircleIntersection(c1, c2) {
		    var d = getDistanceBetweenPoints(c1, c2),
		        r1 = c1.r,
		        r2 = c2.r,
		        points = [];

		    if (d < r1 + r2 && d > Math.abs(r1 - r2)) {
		        // If the circles are overlapping, but not completely overlapping, then
		        // it exists intersecting points.
		        var r1Square = r1 * r1,
		            r2Square = r2 * r2,
		            // d^2 - r^2 + R^2 / 2d
		            x = (r1Square - r2Square + d * d) / (2 * d),
		            // y^2 = R^2 - x^2
		            y = Math.sqrt(r1Square - x * x),
		            x1 = c1.x,
		            x2 = c2.x,
		            y1 = c1.y,
		            y2 = c2.y,
		            x0 = x1 + x * (x2 - x1) / d,
		            y0 = y1 + x * (y2 - y1) / d,
		            rx = -(y2 - y1) * (y / d),
		            ry = -(x2 - x1) * (y / d);

		        points = [
		            { x: round(x0 + rx, 14), y: round(y0 - ry, 14) },
		            { x: round(x0 - rx, 14), y: round(y0 + ry, 14) }
		        ];
		    }
		    return points;
		};

		/**
		 * Calculates all the intersection points for between a list of circles.
		 *
		 * @param {array} circles The circles to calculate the points from.
		 * @returns {array} Returns a list of intersection points.
		 */
		var getCirclesIntersectionPoints = function getIntersectionPoints(circles) {
		    return circles.reduce(function (points, c1, i, arr) {
		        var additional = arr.slice(i + 1)
		            .reduce(function (points, c2, j) {
		                var indexes = [i, j + i + 1];
		                return points.concat(
		                    getCircleCircleIntersection(c1, c2)
		                        .map(function (p) {
		                            p.indexes = indexes;
		                            return p;
		                        })
		                );
		            }, []);
		        return points.concat(additional);
		    }, []);
		};

		/**
		 * Tests wether a point lies within a given circle.
		 *
		 * @param {object} point The point to test for.
		 * @param {object} circle The circle to test if the point is within.
		 * @returns {boolean} Returns true if the point is inside, false if outside.
		 */
		var isPointInsideCircle = function isPointInsideCircle(point, circle) {
		    return getDistanceBetweenPoints(point, circle) <= circle.r + 1e-10;
		};

		/**
		 * Tests wether a point lies within a set of circles.
		 *
		 * @param {object} point The point to test.
		 * @param {array} circles The list of circles to test against.
		 * @returns {boolean} Returns true if the point is inside all the circles, false
		 * if not.
		 */
		var isPointInsideAllCircles = function isPointInsideAllCircles(point, circles) {
		    return !circles.some(function (circle) {
		        return !isPointInsideCircle(point, circle);
		    });
		};

		/**
		 * Tests wether a point lies outside a set of circles.
		 *
		 * TODO: add unit tests.
		 *
		 * @param {object} point The point to test.
		 * @param {array} circles The list of circles to test against.
		 * @returns {boolean} Returns true if the point is outside all the circles,
		 * false if not.
		 */
		var isPointOutsideAllCircles =
		function isPointOutsideAllCircles(point, circles) {
		    return !circles.some(function (circle) {
		        return isPointInsideCircle(point, circle);
		    });
		};

		/**
		 * Calculate the path for the area of overlap between a set of circles.
		 *
		 * TODO: handle cases with only 1 or 0 arcs.
		 *
		 * @param {array} circles List of circles to calculate area of.
		 * @returns {string} Returns the path for the area of overlap. Returns an empty
		 * string if there are no intersection between all the circles.
		 */
		var getAreaOfIntersectionBetweenCircles =
		function getAreaOfIntersectionBetweenCircles(circles) {
		    var intersectionPoints = getCirclesIntersectionPoints(circles)
		        .filter(function (p) {
		            return isPointInsideAllCircles(p, circles);
		        }),
		        result;

		    if (intersectionPoints.length > 1) {
		        // Calculate the center of the intersection points.
		        var center = getCenterOfPoints(intersectionPoints);

		        intersectionPoints = intersectionPoints
		            // Calculate the angle between the center and the points.
		            .map(function (p) {
		                p.angle = getAngleBetweenPoints(center, p);
		                return p;
		            })
		            // Sort the points by the angle to the center.
		            .sort(function (a, b) {
		                return b.angle - a.angle;
		            });

		        var startPoint = intersectionPoints[intersectionPoints.length - 1];
		        var arcs = intersectionPoints
		            .reduce(function (data, p1) {
		                var startPoint = data.startPoint,
		                    midPoint = getCenterOfPoints([startPoint, p1]);

		                // Calculate the arc from the intersection points and their
		                // circles.
		                var arc = p1.indexes
		                    // Filter out circles that are not included in both
		                    // intersection points.
		                    .filter(function (index) {
		                        return startPoint.indexes.indexOf(index) > -1;
		                    })
		                    // Iterate the circles of the intersection points and
		                    // calculate arcs.
		                    .reduce(function (arc, index) {
		                        var circle = circles[index],
		                            angle1 = getAngleBetweenPoints(circle, p1),
		                            angle2 = getAngleBetweenPoints(circle, startPoint),
		                            angleDiff = angle2 - angle1 +
		                                (angle2 < angle1 ? 2 * Math.PI : 0),
		                            angle = angle2 - angleDiff / 2,
		                            width = getDistanceBetweenPoints(
		                                midPoint,
		                                {
		                                    x: circle.x + circle.r * Math.sin(angle),
		                                    y: circle.y + circle.r * Math.cos(angle)
		                                }
		                            ),
		                            r = circle.r;

		                        // Width can sometimes become to large due to floating
		                        // point errors
		                        if (width > r * 2) {
		                            width = r * 2;
		                        }
		                        // Get the arc with the smallest width.
		                        if (!arc || arc.width > width) {
		                            arc = {
		                                r: r,
		                                largeArc: width > r ? 1 : 0,
		                                width: width,
		                                x: p1.x,
		                                y: p1.y
		                            };
		                        }

		                        // Return the chosen arc.
		                        return arc;
		                    }, null);

		                // If we find an arc then add it to the list and update p2.
		                if (arc) {
		                    var r = arc.r;
		                    data.arcs.push(
		                        ['A', r, r, 0, arc.largeArc, 1, arc.x, arc.y]
		                    );
		                    data.startPoint = p1;
		                }
		                return data;
		            }, {
		                startPoint: startPoint,
		                arcs: []
		            }).arcs;

		        if (arcs.length === 0) {
		        } else if (arcs.length === 1) {
		        } else {
		            arcs.unshift(['M', startPoint.x, startPoint.y]);
		            result = {
		                center: center,
		                d: arcs
		            };
		        }
		    }

		    return result;
		};

		var geometryCircles = {
		    getAreaOfIntersectionBetweenCircles: getAreaOfIntersectionBetweenCircles,
		    getCircleCircleIntersection: getCircleCircleIntersection,
		    getCirclesIntersectionPoints: getCirclesIntersectionPoints,
		    getOverlapBetweenCircles: getOverlapBetweenCircles,
		    isPointInsideCircle: isPointInsideCircle,
		    isPointInsideAllCircles: isPointInsideAllCircles,
		    isPointOutsideAllCircles: isPointOutsideAllCircles,
		    round: round
		};


		return geometryCircles;
	}(geometry));
	(function (draw, geometry, geometryCircles, H) {
		/* *
		 *
		 *  (c) 2016-2018 Highsoft AS
		 *
		 *  Authors: Jon Arild Nygard
		 *
		 *  Layout algorithm by Ben Frederickson:
		 *  https://www.benfrederickson.com/better-venn-diagrams/
		 *
		 *  License: www.highcharts.com/license
		 *
		 *  This is an experimental Highcharts module which enables visualization of a
		 *  Venn Diagram.
		 *
		 * */



		var color = H.Color,
		    extend = H.extend,
		    getAreaOfIntersectionBetweenCircles =
		        geometryCircles.getAreaOfIntersectionBetweenCircles,
		    getCircleCircleIntersection = geometryCircles.getCircleCircleIntersection,
		    getCenterOfPoints = geometry.getCenterOfPoints,
		    getDistanceBetweenPoints = geometry.getDistanceBetweenPoints,
		    getOverlapBetweenCirclesByDistance =
		        geometryCircles.getOverlapBetweenCircles,
		    isArray = H.isArray,
		    isNumber = H.isNumber,
		    isObject = H.isObject,
		    isPointInsideAllCircles = geometryCircles.isPointInsideAllCircles,
		    isPointOutsideAllCircles = geometryCircles.isPointOutsideAllCircles,
		    isString = H.isString,
		    merge = H.merge,
		    round = geometryCircles.round,
		    seriesType = H.seriesType;

		var objectValues = function objectValues(obj) {
		    return Object.keys(obj).map(function (x) {
		        return obj[x];
		    });
		};

		/**
		 * Calculates the area of overlap between a list of circles.
		 * @private
		 * @todo add support for calculating overlap between more than 2 circles.
		 * @param {Array<object>} circles List of circles with their given positions.
		 * @return {number} Returns the area of overlap between all the circles.
		 */
		var getOverlapBetweenCircles = function getOverlapBetweenCircles(circles) {
		    var overlap = 0;

		    // When there is only two circles we can find the overlap by using their
		    // radiuses and the distance between them.
		    if (circles.length === 2) {
		        var circle1 = circles[0];
		        var circle2 = circles[1];

		        overlap = getOverlapBetweenCirclesByDistance(
		            circle1.r,
		            circle2.r,
		            getDistanceBetweenPoints(circle1, circle2)
		        );
		    }

		    return overlap;
		};

		/**
		 * Calculates the difference between the desired overlap and the actual overlap
		 * between two circles.
		 * @private
		 * @param {object} mapOfIdToCircle Map from id to circle.
		 * @param {Array<object>} relations List of relations to calculate the loss of.
		 * @return {number} Returns the loss between positions of the circles for the
		 * given relations.
		 */
		var loss = function loss(mapOfIdToCircle, relations) {
		    var precision = 10e10;
		    // Iterate all the relations and calculate their individual loss.
		    return relations.reduce(function (totalLoss, relation) {
		        var loss = 0;

		        if (relation.sets.length > 1) {
		            var wantedOverlap = relation.value;
		            // Calculate the actual overlap between the sets.
		            var actualOverlap = getOverlapBetweenCircles(
		                // Get the circles for the given sets.
		                relation.sets.map(function (set) {
		                    return mapOfIdToCircle[set];
		                })
		            );

		            var diff = wantedOverlap - actualOverlap;
		            loss = Math.round((diff * diff) * precision) / precision;
		        }

		        // Add calculated loss to the sum.
		        return totalLoss + loss;
		    }, 0);
		};

		/**
		 * A binary search function that takes an higher order function to opererate on
		 * the values in the array before returning the result to compare with the value
		 * that is searched for.
		 * Useful to find a value that will give a result that meets some requirement.
		 * @private
		 * @param {Array<number>} arr The array to search for.
		 * @param {*} value The value that is wanted to find.
		 * @param {Function} fn The higher order function to operate on the values in
		 * the array.
		 * @return {number} Returns the index of the matching value, or -1 if the value
		 * was not found.
		 */
		var binarySearch = function binarySearch(arr, value, fn) {
		    var start = 0,
		        stop = arr.length - 1,
		        middle = Math.floor((start + stop) / 2),
		        res;

		    while ((res = fn(arr[middle], value)) !== value && start < stop) {
		        if (value < res) {
		            stop = middle - 1;
		        } else {
		            start = middle + 1;
		        }
		        middle = Math.floor((start + stop) / 2);
		    }

		    return res === value ? middle : -1;
		};

		/**
		 * Creates a list that contains a sequence of numbers ranging from start to end.
		 * @private
		 * @todo add unit tests.
		 * @param {number} start The smallest number in the list.
		 * @param {number} end The largest number in the list.
		 * @param {number} [step=1] The increment between the values in the sequence.
		 * @return {Array<number>} Returns the sequence of numbers in the range from start to
		 * end.
		 */
		var range = function range(start, end, step) {
		    var s = step || 1,
		        length = Math.round((start + end) / s),
		        range = Array.apply(0, Array(length)).map(function (_, i) {
		            return i * s;
		        });
		    return range;
		};

		/**
		 * Uses binary search to make a best guess of the ideal distance between two
		 * circles too get the desired overlap.
		 * Currently there is no known formula to calculate the distance from the area
		 * of overlap, which makes the binary search a preferred method.
		 * @private
		 * @param {number} r1 Radius of the first circle.
		 * @param {number} r2 Radiues of the second circle.
		 * @param {number} overlap The wanted overlap between the two circles.
		 * @return {number} Returns the distance needed to get the wanted overlap
		 * between the two circles.
		 */
		var getDistanceBetweenCirclesByOverlap =
		function getDistanceBetweenCirclesByOverlap(r1, r2, overlap) {
		    var error = 0.01,
		        maxDistance = r1 + r2,
		        list = range(0, maxDistance, 0.001),
		        index = binarySearch(list, 0, function (x) {
		            var actualOverlap = getOverlapBetweenCirclesByDistance(r1, r2, x),
		                diff = overlap - actualOverlap;
		            // If the difference is below accepted error then return overlap to
		            // confirm we have found the right value.
		            return (Math.abs(diff) < error) ? 0 : diff;
		        });

		    // Round the resulting value to have two decimals.
		    return round(list[index], 14);
		};

		var isSet = function (x) {
		    return isArray(x.sets) && x.sets.length === 1;
		};

		/**
		 * Finds an optimal position for a given point.
		 * @private
		 * @todo add unit tests.
		 * @todo add constraints to optimize the algorithm.
		 * @param {Function} fn The function to test a point.
		 * @param {Array<*>} initial The initial point to optimize.
		 * @return {Array<*>} Returns the opimized position of a point.
		 */
		var nelderMead = function nelderMead(fn, initial) {
		    var maxIterations = 100,
		        sortByFx = function (a, b) {
		            return a.fx - b.fx;
		        },
		        pRef = 1, // Reflection parameter
		        pExp = 2, // Expansion parameter
		        pCon = -0.5, // Contraction parameter
		        pOCon = pCon * pRef, // Outwards contraction parameter
		        pShrink = 0.5; // Shrink parameter

		    var weightedSum = function weightedSum(weight1, v1, weight2, v2) {
		        return v1.map(function (x, i) {
		            return weight1 * x + weight2 * v2[i];
		        });
		    };

		    var getSimplex = function getSimplex(initial) {
		        var n = initial.length,
		            simplex = new Array(n + 1);

		        // Initial point to the simplex.
		        simplex[0] = initial;
		        simplex[0].fx = fn(initial);

		        // Create a set of extra points based on the initial.
		        for (var i = 0; i < n; ++i) {
		            var point = initial.slice();
		            point[i] = point[i] ? point[i] * 1.05 : 0.001;
		            point.fx = fn(point);
		            simplex[i + 1] = point;
		        }
		        return simplex;
		    };

		    var updateSimplex = function (simplex, point) {
		        point.fx = fn(point);
		        simplex[simplex.length - 1] = point;
		        return simplex;
		    };

		    var shrinkSimplex = function (simplex) {
		        var best = simplex[0];
		        return simplex.map(function (point) {
		            var p = weightedSum(1 - pShrink, best, pShrink, point);
		            p.fx = fn(p);
		            return p;
		        });
		    };

		    var getCentroid = function (simplex) {
		        var arr = simplex.slice(0, -1),
		            length = arr.length,
		            result = [],
		            sum = function (data, point) {
		                data.sum += point[data.i];
		                return data;
		            };

		        for (var i = 0; i < length; i++) {
		            result[i] = simplex.reduce(sum, { sum: 0, i: i }).sum / length;
		        }
		        return result;
		    };

		    var getPoint = function (centroid, worst, a, b) {
		        var point = weightedSum(a, centroid, b, worst);
		        point.fx = fn(point);
		        return point;
		    };

		    // Create a simplex
		    var simplex = getSimplex(initial);

		    // Iterate from 0 to max iterations
		    for (var i = 0; i < maxIterations; i++) {
		        // Sort the simplex
		        simplex.sort(sortByFx);

		        // Create a centroid from the simplex
		        var worst = simplex[simplex.length - 1];
		        var centroid = getCentroid(simplex);

		        // Calculate the reflected point.
		        var reflected = getPoint(centroid, worst, 1 + pRef, -pRef);

		        if (reflected.fx < simplex[0].fx) {
		            // If reflected point is the best, then possibly expand.
		            var expanded = getPoint(centroid, worst, 1 + pExp, -pExp);
		            simplex = updateSimplex(
		                simplex,
		                (expanded.fx < reflected.fx) ? expanded : reflected
		            );
		        } else if (reflected.fx >= simplex[simplex.length - 2].fx) {
		            // If the reflected point is worse than the second worse, then
		            // contract.
		            var contracted;
		            if (reflected.fx > worst.fx) {
		                // If the reflected is worse than the worst point, do a
		                // contraction
		                contracted = getPoint(centroid, worst, 1 + pCon, -pCon);
		                if (contracted.fx < worst.fx) {
		                    simplex = updateSimplex(simplex, contracted);
		                } else {
		                    simplex = shrinkSimplex(simplex);
		                }
		            } else {
		                // Otherwise do an outwards contraction
		                contracted = getPoint(centroid, worst, 1 - pOCon, pOCon);
		                if (contracted.fx < reflected.fx) {
		                    simplex = updateSimplex(simplex, contracted);
		                } else {
		                    simplex = shrinkSimplex(simplex);
		                }
		            }
		        } else {
		            simplex = updateSimplex(simplex, reflected);
		        }
		    }

		    return simplex[0];
		};

		/**
		 * Calculates a margin for a point based on the iternal and external circles.
		 * The margin describes if the point is well placed within the internal circles,
		 * and away from the external
		 * @private
		 * @todo add unit tests.
		 * @param {object} point The point to evaluate.
		 * @param {Array<object>} internal The internal circles.
		 * @param {Array<object>} external The external circles.
		 * @return {number} Returns the margin.
		 */
		var getMarginFromCircles =
		function getMarginFromCircles(point, internal, external) {
		    var margin = internal.reduce(function (margin, circle) {
		        var m = circle.r - getDistanceBetweenPoints(point, circle);
		        return (m <= margin) ? m : margin;
		    }, Number.MAX_SAFE_INTEGER);

		    margin = external.reduce(function (margin, circle) {
		        var m = getDistanceBetweenPoints(point, circle) - circle.r;
		        return (m <= margin) ? m : margin;
		    }, margin);

		    return margin;
		};

		/**
		 * Finds the optimal label position by looking for a position that has a low
		 * distance from the internal circles, and as large possible distane to the
		 * external circles.
		 * @private
		 * @todo Optimize the intial position.
		 * @todo Add unit tests.
		 * @param {Array<object>} internal Internal circles.
		 * @param {Array<object>} external External circles.
		 * @return {object} Returns the found position.
		 */
		var getLabelPosition = function getLabelPosition(internal, external) {
		    // Get the best label position within the internal circles.
		    var best = internal.reduce(function (best, circle) {
		        var d = circle.r / 2;

		        // Give a set of points with the circle to evaluate as the best label
		        // position.
		        return [
		            { x: circle.x, y: circle.y },
		            { x: circle.x + d, y: circle.y },
		            { x: circle.x - d, y: circle.y },
		            { x: circle.x, y: circle.y + d },
		            { x: circle.x, y: circle.y - d }
		        ]
		        // Iterate the given points and return the one with the largest margin.
		        .reduce(function (best, point) {
		            var margin = getMarginFromCircles(point, internal, external);

		            // If the margin better than the current best, then update best.
		            if (best.margin < margin) {
		                best.point = point;
		                best.margin = margin;
		            }
		            return best;
		        }, best);
		    }, {
		        point: undefined,
		        margin: -Number.MAX_SAFE_INTEGER
		    }).point;

		    // Use nelder mead to optimize the initial label position.
		    var optimal = nelderMead(
		        function (p) {
		            return -(
		                getMarginFromCircles({ x: p[0], y: p[1] }, internal, external)
		            );
		        },
		        [best.x, best.y]
		    );

		    // Update best to be the point which was found to have the best margin.
		    best = {
		        x: optimal[0],
		        y: optimal[1]
		    };

		    if (!(
		        isPointInsideAllCircles(best, internal) &&
		        isPointOutsideAllCircles(best, external)
		    )) {
		        // If point was either outside one of the internal, or inside one of the
		        // external, then it was invalid and should use a fallback.
		        best = getCenterOfPoints(internal);
		    }

		    // Return the best point.
		    return best;
		};

		/**
		 * Calulates data label positions for a list of relations.
		 * @private
		 * @todo add unit tests
		 * @todo NOTE: may be better suited as a part of the layout function.
		 * @param {Array<object>} relations The list of relations.
		 * @return {object} Returns a map from id to the data label position.
		 */
		var getLabelPositions = function getLabelPositions(relations) {
		    var singleSets = relations.filter(isSet);
		    return relations.reduce(function (map, relation) {
		        if (relation.value) {
		            var sets = relation.sets,
		                id = sets.join(),
		                // Create a list of internal and external circles.
		                data = singleSets.reduce(function (data, set) {
		                    // If the set exists in this relation, then it is internal,
		                    // otherwise it will be external.
		                    var isInternal = sets.indexOf(set.sets[0]) > -1,
		                        property = isInternal ? 'internal' : 'external';

		                    // Add the circle to the list.
		                    data[property].push(set.circle);
		                    return data;
		                }, {
		                    internal: [],
		                    external: []
		                });

		            // Calulate the label position.
		            map[id] = getLabelPosition(
		                data.internal,
		                data.external
		            );
		        }
		        return map;
		    }, {});
		};

		/**
		 * Takes an array of relations and adds the properties `totalOverlap` and
		 * `overlapping` to each set. The property `totalOverlap` is the sum of value
		 * for each relation where this set is included. The property `overlapping` is
		 * a map of how much this set is overlapping another set.
		 * NOTE: This algorithm ignores relations consisting of more than 2 sets.
		 * @private
		 * @param {Array<object>} relations The list of relations that should be sorted.
		 * @return {Array<object>} Returns the modified input relations with added
		 * properties `totalOverlap` and `overlapping`.
		 */
		var addOverlapToSets = function addOverlapToSets(relations) {
		    // Calculate the amount of overlap per set.
		    var mapOfIdToProps = relations
		        // Filter out relations consisting of 2 sets.
		        .filter(function (relation) {
		            return relation.sets.length === 2;
		        })
		        // Sum up the amount of overlap for each set.
		        .reduce(function (map, relation) {
		            var sets = relation.sets;
		            sets.forEach(function (set, i, arr) {
		                if (!isObject(map[set])) {
		                    map[set] = {
		                        overlapping: {},
		                        totalOverlap: 0
		                    };
		                }
		                map[set].totalOverlap += relation.value;
		                map[set].overlapping[arr[1 - i]] = relation.value;
		            });
		            return map;
		        }, {});

		    relations
		        // Filter out single sets
		        .filter(isSet)
		        // Extend the set with the calculated properties.
		        .forEach(function (set) {
		            var properties = mapOfIdToProps[set.sets[0]];
		            extend(set, properties);
		        });

		    // Returns the modified relations.
		    return relations;
		};

		/**
		 * Takes two sets and finds the one with the largest total overlap.
		 * @private
		 * @param {object} a The first set to compare.
		 * @param {object} b The second set to compare.
		 * @return {number} Returns 0 if a and b are equal, <0 if a is greater, >0 if b
		 * is greater.
		 */
		var sortByTotalOverlap = function sortByTotalOverlap(a, b) {
		    return b.totalOverlap - a.totalOverlap;
		};

		/**
		 * Uses a greedy approach to position all the sets. Works well with a small
		 * number of sets, and are in these cases a good choice aesthetically.
		 * @private
		 * @param {Array<object>} relations List of the overlap between two or more
		 * sets, or the size of a single set.
		 * @return {Array<object>} List of circles and their calculated positions.
		 */
		var layoutGreedyVenn = function layoutGreedyVenn(relations) {
		    var positionedSets = [],
		        mapOfIdToCircles = {};

		    // Define a circle for each set.
		    relations
		        .filter(function (relation) {
		            return relation.sets.length === 1;
		        }).forEach(function (relation) {
		            mapOfIdToCircles[relation.sets[0]] = relation.circle = {
		                x: Number.MAX_SAFE_INTEGER,
		                y: Number.MAX_SAFE_INTEGER,
		                r: Math.sqrt(relation.value / Math.PI)
		            };
		        });

		    /**
		     * Takes a set and updates the position, and add the set to the list of
		     * positioned sets.
		     * @private
		     * @param {object} set The set to add to its final position.
		     * @param {object} coordinates The coordinates to position the set at.
		     */
		    var positionSet = function positionSet(set, coordinates) {
		        var circle = set.circle;
		        circle.x = coordinates.x;
		        circle.y = coordinates.y;
		        positionedSets.push(set);
		    };

		    // Find overlap between sets. Ignore relations with more then 2 sets.
		    addOverlapToSets(relations);

		    // Sort sets by the sum of their size from large to small.
		    var sortedByOverlap = relations
		        .filter(isSet)
		        .sort(sortByTotalOverlap);

		    // Position the most overlapped set at 0,0.
		    positionSet(sortedByOverlap.pop(), { x: 0, y: 0 });

		    var relationsWithTwoSets = relations.filter(function (x) {
		        return x.sets.length === 2;
		    });

		    // Iterate and position the remaining sets.
		    sortedByOverlap.forEach(function (set) {
		        var circle = set.circle,
		            radius = circle.r,
		            overlapping = set.overlapping;

		        var bestPosition = positionedSets
		        .reduce(function (best, positionedSet, i) {
		            var positionedCircle = positionedSet.circle,
		                overlap = overlapping[positionedSet.sets[0]];

		            // Calculate the distance between the sets to get the correct
		            // overlap
		            var distance = getDistanceBetweenCirclesByOverlap(
		                radius,
		                positionedCircle.r,
		                overlap
		            );

		            // Create a list of possible coordinates calculated from distance.
		            var possibleCoordinates = [
		                { x: positionedCircle.x + distance, y: positionedCircle.y },
		                { x: positionedCircle.x - distance, y: positionedCircle.y },
		                { x: positionedCircle.x, y: positionedCircle.y + distance },
		                { x: positionedCircle.x, y: positionedCircle.y - distance }
		            ];

		            // If there are more circles overlapping, then add the intersection
		            // points as possible positions.
		            positionedSets.slice(i + 1).forEach(function (positionedSet2) {
		                var positionedCircle2 = positionedSet2.circle,
		                    overlap2 = overlapping[positionedSet2.sets[0]],
		                    distance2 = getDistanceBetweenCirclesByOverlap(
		                        radius,
		                        positionedCircle2.r,
		                        overlap2
		                    );

		                // Add intersections to list of coordinates.
		                possibleCoordinates = possibleCoordinates.concat(
		                    getCircleCircleIntersection({
		                        x: positionedCircle.x,
		                        y: positionedCircle.y,
		                        r: distance2
		                    }, {
		                        x: positionedCircle2.x,
		                        y: positionedCircle2.y,
		                        r: distance2
		                    })
		                );
		            });

		            // Iterate all suggested coordinates and find the best one.
		            possibleCoordinates.forEach(function (coordinates) {
		                circle.x = coordinates.x;
		                circle.y = coordinates.y;

		                // Calculate loss for the suggested coordinates.
		                var currentLoss = loss(mapOfIdToCircles, relationsWithTwoSets);

		                // If the loss is better, then use these new coordinates.
		                if (currentLoss < best.loss) {
		                    best.loss = currentLoss;
		                    best.coordinates = coordinates;
		                }
		            });

		            // Return resulting coordinates.
		            return best;
		        }, {
		            loss: Number.MAX_SAFE_INTEGER,
		            coordinates: undefined
		        });

		        // Add the set to its final position.
		        positionSet(set, bestPosition.coordinates);
		    });

		    // Return the positions of each set.
		    return mapOfIdToCircles;
		};

		/**
		 * Calculates the positions of all the sets in the venn diagram.
		 * @private
		 * @todo Add support for constrained MDS.
		 * @param {Array<object>} relations List of the overlap between two or more sets, or the
		 * size of a single set.
		 * @return {Arrat<object>} List of circles and their calculated positions.
		 */
		var layout = function (relations) {
		    var mapOfIdToShape = {};

		    // Calculate best initial positions by using greedy layout.
		    if (relations.length > 0) {
		        mapOfIdToShape = layoutGreedyVenn(relations);

		        relations
		            .filter(function (x) {
		                return !isSet(x);
		            })
		            .forEach(function (relation) {
		                var sets = relation.sets,
		                    id = sets.join(),
		                    circles = sets.map(function (set) {
		                        return mapOfIdToShape[set];
		                    });

		                // Add intersection shape to map
		                mapOfIdToShape[id] =
		                    getAreaOfIntersectionBetweenCircles(circles);
		            });
		    }
		    return mapOfIdToShape;
		};

		var isValidRelation = function (x) {
		    var map = {};
		    return (
		        isObject(x) &&
		        (isNumber(x.value) && x.value > -1) &&
		        (isArray(x.sets) && x.sets.length > 0) &&
		        !x.sets.some(function (set) {
		            var invalid = false;
		            if (!map[set] && isString(set)) {
		                map[set] = true;
		            } else {
		                invalid = true;
		            }
		            return invalid;
		        })
		    );
		};

		var isValidSet = function (x) {
		    return (isValidRelation(x) && isSet(x) && x.value > 0);
		};

		/**
		 * Prepares the venn data so that it is usable for the layout function. Filter
		 * out sets, or intersections that includes sets, that are missing in the data
		 * or has (value < 1). Adds missing relations between sets in the data as
		 * value = 0.
		 * @private
		 * @param {Array<object>} data The raw input data.
		 * @return {Array<object>} Returns an array of valid venn data.
		 */
		var processVennData = function processVennData(data) {
		    var d = isArray(data) ? data : [];

		    var validSets = d
		        .reduce(function (arr, x) {
		            // Check if x is a valid set, and that it is not an duplicate.
		            if (isValidSet(x) && arr.indexOf(x.sets[0]) === -1) {
		                arr.push(x.sets[0]);
		            }
		            return arr;
		        }, [])
		        .sort();

		    var mapOfIdToRelation = d.reduce(function (mapOfIdToRelation, relation) {
		        if (isValidRelation(relation) && !relation.sets.some(function (set) {
		            return validSets.indexOf(set) === -1;
		        })) {
		            mapOfIdToRelation[relation.sets.sort().join()] = relation;
		        }
		        return mapOfIdToRelation;
		    }, {});

		    validSets.reduce(function (combinations, set, i, arr) {
		        var remaining = arr.slice(i + 1);
		        remaining.forEach(function (set2) {
		            combinations.push(set + ',' + set2);
		        });
		        return combinations;
		    }, []).forEach(function (combination) {
		        if (!mapOfIdToRelation[combination]) {
		            var obj = {
		                sets: combination.split(','),
		                value: 0
		            };
		            mapOfIdToRelation[combination] = obj;
		        }
		    });

		    // Transform map into array.
		    return objectValues(mapOfIdToRelation);
		};

		/**
		 * Calculates the proper scale to fit the cloud inside the plotting area.
		 * @private
		 * @todo add unit test
		 * @param {number} targetWidth  Width of target area.
		 * @param {number} targetHeight Height of target area.
		 * @param {object} field The playing field.
		 * @param {Highcharts.Series} series Series object.
		 * @return {object} Returns the value to scale the playing field up to the size
		 * of the target area, and center of x and y.
		 */
		var getScale = function getScale(targetWidth, targetHeight, field) {
		    var height = field.bottom - field.top, // top is smaller than bottom
		        width = field.right - field.left,
		        scaleX = width > 0 ? 1 / width * targetWidth : 1,
		        scaleY = height > 0 ? 1 / height * targetHeight : 1,
		        adjustX = (field.right + field.left) / 2,
		        adjustY = (field.top + field.bottom) / 2,
		        scale = Math.min(scaleX, scaleY);

		    return {
		        scale: scale,
		        centerX: targetWidth / 2 - adjustX * scale,
		        centerY: targetHeight / 2 - adjustY * scale
		    };
		};

		/**
		 * If a circle is outside a give field, then the boundaries of the field is
		 * adjusted accordingly. Modifies the field object which is passed as the first
		 * parameter.
		 * @private
		 * @todo NOTE: Copied from wordcloud, can probably be unified.
		 * @param {object} field The bounding box of a playing field.
		 * @param {object} placement The bounding box for a placed point.
		 * @return {object} Returns a modified field object.
		 */
		var updateFieldBoundaries = function updateFieldBoundaries(field, circle) {
		    var left = circle.x - circle.r,
		        right = circle.x + circle.r,
		        bottom = circle.y + circle.r,
		        top = circle.y - circle.r;

		    // TODO improve type checking.
		    if (!isNumber(field.left) || field.left > left) {
		        field.left = left;
		    }
		    if (!isNumber(field.right) || field.right < right) {
		        field.right = right;
		    }
		    if (!isNumber(field.top) || field.top > top) {
		        field.top = top;
		    }
		    if (!isNumber(field.bottom) || field.bottom < bottom) {
		        field.bottom = bottom;
		    }
		    return field;
		};

		/**
		 * A Venn diagram displays all possible logical relations between a collection
		 * of different sets. The sets are represented by circles, and the relation
		 * between the sets are displayed by the overlap or lack of overlap between
		 * them. The venn diagram is a special case of Euler diagrams, which can also
		 * be displayed by this series type.
		 *
		 * @sample {highcharts} highcharts/demo/venn-diagram/
		 *         Venn diagram
		 * @sample {highcharts} highcharts/demo/euler-diagram/
		 *         Euler diagram
		 *
		 * @extends      plotOptions.scatter
		 * @excluding    connectEnds, connectNulls, cropThreshold, findNearestPointBy,
		 *               getExtremesFromAll, label, linecap, lineWidth, linkedTo,
		 *               marker, negativeColor, pointInterval, pointIntervalUnit,
		 *               pointPlacement, pointStart, softThreshold, stacking, steps,
		 *               threshold, xAxis, yAxis, zoneAxis, zones
		 * @product      highcharts
		 * @optionparent plotOptions.venn
		 */
		var vennOptions = {
		    borderColor: '#cccccc',
		    borderDashStyle: 'solid',
		    borderWidth: 1,
		    brighten: 0,
		    clip: false,
		    colorByPoint: true,
		    dataLabels: {
		        enabled: true,
		        formatter: function () {
		            return this.point.name;
		        }
		    },
		    marker: false,
		    opacity: 0.75,
		    showInLegend: false,
		    states: {
		        hover: {
		            opacity: 1,
		            halo: false,
		            borderColor: '#333333'
		        },
		        select: {
		            color: '#cccccc',
		            borderColor: '#000000',
		            animation: false
		        }
		    },
		    tooltip: {
		        pointFormat: '{point.name}: {point.value}'
		    }
		};

		var vennSeries = {
		    isCartesian: false,
		    axisTypes: [],
		    directTouch: true,
		    translate: function () {

		        var chart = this.chart;

		        this.processedXData = this.xData;
		        this.generatePoints();

		        // Process the data before passing it into the layout function.
		        var relations = processVennData(this.options.data);

		        // Calculate the positions of each circle.
		        var mapOfIdToShape = layout(relations);

		        // Calculate positions of each data label
		        var mapOfIdToLabelPosition = getLabelPositions(relations);

		        // Calculate the scale, and center of the plot area.
		        var field = Object.keys(mapOfIdToShape)
		            .filter(function (key) {
		                var shape = mapOfIdToShape[key];
		                return shape && isNumber(shape.r);
		            })
		            .reduce(function (field, key) {
		                return updateFieldBoundaries(field, mapOfIdToShape[key]);
		            }, { top: 0, bottom: 0, left: 0, right: 0 }),
		            scaling = getScale(chart.plotWidth, chart.plotHeight, field),
		            scale = scaling.scale,
		            centerX = scaling.centerX,
		            centerY = scaling.centerY;

		        // Iterate all points and calculate and draw their graphics.
		        this.points.forEach(function (point) {
		            var sets = isArray(point.sets) ? point.sets : [],
		                id = sets.join(),
		                shape = mapOfIdToShape[id],
		                shapeArgs,
		                dataLabelPosition = mapOfIdToLabelPosition[id];

		            if (shape) {
		                if (shape.r) {
		                    shapeArgs = {
		                        x: centerX + shape.x * scale,
		                        y: centerY + shape.y * scale,
		                        r: shape.r * scale
		                    };
		                } else if (shape.d) {
		                    // TODO: find a better way to handle scaling of a path.
		                    var d = shape.d.reduce(function (path, arr) {
		                        if (arr[0] === 'M') {
		                            arr[1] = centerX + arr[1] * scale;
		                            arr[2] = centerY + arr[2] * scale;
		                        } else if (arr[0] === 'A') {
		                            arr[1] = arr[1] * scale;
		                            arr[2] = arr[2] * scale;
		                            arr[6] = centerX + arr[6] * scale;
		                            arr[7] = centerY + arr[7] * scale;
		                        }
		                        return path.concat(arr);
		                    }, [])
		                    .join(' ');
		                    shapeArgs = {
		                        d: d
		                    };
		                }

		                // Scale the position for the data label.
		                if (dataLabelPosition) {
		                    dataLabelPosition.x = centerX + dataLabelPosition.x * scale;
		                    dataLabelPosition.y = centerY + dataLabelPosition.y * scale;
		                } else {
		                    dataLabelPosition = {};
		                }
		            }

		            point.shapeArgs = shapeArgs;

		            // Placement for the data labels
		            if (dataLabelPosition && shapeArgs) {
		                point.plotX = dataLabelPosition.x;
		                point.plotY = dataLabelPosition.y;
		            }

		            // Set name for usage in tooltip and in data label.
		            point.name = point.options.name || sets.join('∩');
		        });
		    },
		    /**
		     * Draw the graphics for each point.
		     * @private
		     */
		    drawPoints: function () {
		        var series = this,
		            // Series properties
		            chart = series.chart,
		            group = series.group,
		            points = series.points || [],
		            // Chart properties
		            renderer = chart.renderer;

		        // Iterate all points and calculate and draw their graphics.
		        points.forEach(function (point) {
		            var attribs,
		                shapeArgs = point.shapeArgs;
		            // Add point attribs
		            if (!chart.styledMode) {
		                attribs = series.pointAttribs(point, point.state);
		            }
		            // Draw the point graphic.
		            point.draw({
		                isNew: !point.graphic,
		                animatableAttribs: shapeArgs,
		                attribs: attribs,
		                group: group,
		                renderer: renderer,
		                shapeType: shapeArgs && shapeArgs.d ? 'path' : 'circle'
		            });
		        });

		    },
		    /**
		     * Calculates the style attributes for a point. The attributes can vary
		     * depending on the state of the point.
		     * @private
		     * @param {object} point The point which will get the resulting attributes.
		     * @param {string} state The state of the point.
		     * @return {object} Returns the calculated attributes.
		     */
		    pointAttribs: function (point, state) {
		        var series = this,
		            seriesOptions = series.options || {},
		            pointOptions = point && point.options || {},
		            stateOptions = (state && seriesOptions.states[state]) || {},
		            options = merge(
		                seriesOptions,
		                { color: point && point.color },
		                pointOptions,
		                stateOptions
		            );

		        // Return resulting values for the attributes.
		        return {
		            'fill': color(options.color)
		                .setOpacity(options.opacity)
		                .brighten(options.brightness)
		                .get(),
		            'stroke': options.borderColor,
		            'stroke-width': options.borderWidth,
		            'dashstyle': options.borderDashStyle
		        };
		    },
		    animate: function (init) {
		        if (!init) {
		            var series = this,
		                animOptions = H.animObject(series.options.animation);

		            series.points.forEach(function (point) {
		                var args = point.shapeArgs;
		                if (point.graphic && args) {
		                    var attr = {},
		                        animate = {};

		                    if (args.d) {
		                        // If shape is a path, then animate opacity.
		                        attr.opacity = 0.001;
		                    } else {
		                        // If shape is a circle, then animate radius.
		                        attr.r = 0;
		                        animate.r = args.r;
		                    }

		                    point.graphic
		                        .attr(attr)
		                        .animate(animate, animOptions);

		                    // If shape is path, then fade it in after the circles
		                    // animation
		                    if (args.d) {
		                        setTimeout(function () {
		                            if (point && point.graphic) {
		                                point.graphic.animate({
		                                    opacity: 1
		                                });
		                            }
		                        }, animOptions.duration);
		                    }
		                }
		            }, series);
		            series.animate = null;
		        }
		    },
		    utils: {
		        addOverlapToSets: addOverlapToSets,
		        binarySearch: binarySearch,
		        geometry: geometry,
		        geometryCircles: geometryCircles,
		        getDistanceBetweenCirclesByOverlap: getDistanceBetweenCirclesByOverlap,
		        loss: loss,
		        processVennData: processVennData,
		        sortByTotalOverlap: sortByTotalOverlap
		    }
		};

		var vennPoint = {
		    draw: draw,
		    shouldDraw: function () {
		        var point = this;

		        // Only draw points with single sets.
		        return !!point.shapeArgs;
		    },
		    isValid: function () {
		        return isNumber(this.value);
		    }
		};

		/**
		 * A `venn` series. If the [type](#series.venn.type) option is
		 * not specified, it is inherited from [chart.type](#chart.type).
		 *
		 * @extends   series,plotOptions.venn
		 * @excluding connectEnds, connectNulls, cropThreshold, dataParser, dataURL,
		 *            findNearestPointBy, getExtremesFromAll, label, linecap, lineWidth,
		 *            linkedTo, marker, negativeColor, pointInterval, pointIntervalUnit,
		 *            pointPlacement, pointStart, softThreshold, stack, stacking, steps,
		 *            threshold, xAxis, yAxis, zoneAxis, zones
		 * @product   highcharts
		 * @apioption series.venn
		 */

		/**
		 * @type      {Array<*>}
		 * @extends   series.scatter.data
		 * @excluding marker, x, y
		 * @product   highcharts
		 * @apioption series.venn.data
		 */

		/**
		 * The name of the point. Used in data labels and tooltip. If name is not
		 * defined then it will default to the joined values in
		 * [sets](#series.venn.sets).
		 *
		 * @sample {highcharts} highcharts/demo/venn-diagram/
		 *         Venn diagram
		 * @sample {highcharts} highcharts/demo/euler-diagram/
		 *         Euler diagram
		 *
		 * @type      {number}
		 * @since     7.0.0
		 * @product   highcharts
		 * @apioption series.venn.data.name
		 */

		/**
		 * The value of the point, resulting in a relative area of the circle, or area
		 * of overlap between two sets in the venn or euler diagram.
		 *
		 * @sample {highcharts} highcharts/demo/venn-diagram/
		 *         Venn diagram
		 * @sample {highcharts} highcharts/demo/euler-diagram/
		 *         Euler diagram
		 *
		 * @type      {number}
		 * @since     7.0.0
		 * @product   highcharts
		 * @apioption series.venn.data.value
		 */

		/**
		 * The set or sets the options will be applied to. If a single entry is defined,
		 * then it will create a new set. If more than one entry is defined, then it
		 * will define the overlap between the sets in the array.
		 *
		 * @sample {highcharts} highcharts/demo/venn-diagram/
		 *         Venn diagram
		 * @sample {highcharts} highcharts/demo/euler-diagram/
		 *         Euler diagram
		 *
		 * @type      {Array<string>}
		 * @since     7.0.0
		 * @product   highcharts
		 * @apioption series.venn.data.sets
		 */

		/**
		 * @private
		 * @class
		 * @name Highcharts.seriesTypes.venn
		 *
		 * @augments Highcharts.Series
		 */
		seriesType('venn', 'scatter', vennOptions, vennSeries, vennPoint);

	}(draw, geometry, geometryCircles, Highcharts));
	return (function () {


	}());
}));
