/*
 Highcharts JS v7.0.0 (2018-12-11)
 Arrow Symbols

 (c) 2017-2018 Lars A. V. Cabrera

 License: www.highcharts.com/license
*/
(function(f){"object"===typeof module&&module.exports?module.exports=f:"function"===typeof define&&define.amd?define(function(){return f}):f("undefined"!==typeof Highcharts?Highcharts:void 0)})(function(f){(function(a){a.SVGRenderer.prototype.symbols.arrow=function(d,b,a,c){return["M",d,b+c/2,"L",d+a,b,"L",d,b+c/2,"L",d+a,b+c]};a.SVGRenderer.prototype.symbols["arrow-half"]=function(d,b,e,c){return a.SVGRenderer.prototype.symbols.arrow(d,b,e/2,c)};a.SVGRenderer.prototype.symbols["triangle-left"]=function(a,
b,e,c){return["M",a+e,b,"L",a,b+c/2,"L",a+e,b+c,"Z"]};a.SVGRenderer.prototype.symbols["arrow-filled"]=a.SVGRenderer.prototype.symbols["triangle-left"];a.SVGRenderer.prototype.symbols["triangle-left-half"]=function(d,b,e,c){return a.SVGRenderer.prototype.symbols["triangle-left"](d,b,e/2,c)};a.SVGRenderer.prototype.symbols["arrow-filled-half"]=a.SVGRenderer.prototype.symbols["triangle-left-half"]})(f)});
//# sourceMappingURL=arrow-symbols.js.map
