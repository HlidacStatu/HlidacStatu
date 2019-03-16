/*
  Highcharts JS v7.0.0 (2018-12-11)

 Variable Pie module for Highcharts

 (c) 2010-2018 Grzegorz Blachliski

 License: www.highcharts.com/license
*/
(function(c){"object"===typeof module&&module.exports?module.exports=c:"function"===typeof define&&define.amd?define(function(){return c}):c("undefined"!==typeof Highcharts?Highcharts:void 0)})(function(c){(function(c){var q=c.pick,r=c.arrayMin,t=c.arrayMax,w=c.seriesType,x=c.seriesTypes.pie.prototype;w("variablepie","pie",{minPointSize:"10%",maxPointSize:"100%",zMin:void 0,zMax:void 0,sizeBy:"area",tooltip:{pointFormat:'\x3cspan style\x3d"color:{point.color}"\x3e\u25cf\x3c/span\x3e {series.name}\x3cbr/\x3eValue: {point.y}\x3cbr/\x3eSize: {point.z}\x3cbr/\x3e'}},
{pointArrayMap:["y","z"],parallelArrays:["x","y","z"],redraw:function(){this.center=null;x.redraw.call(this,arguments)},zValEval:function(a){return"number"!==typeof a||isNaN(a)?null:!0},calculateExtremes:function(){var a=this.chart,c=this.options,d;d=this.zData;var l=Math.min(a.plotWidth,a.plotHeight)-2*(c.slicedOffset||0),g={},a=this.center||this.getCenter();["minPointSize","maxPointSize"].forEach(function(a){var b=c[a],d=/%$/.test(b),b=parseInt(b,10);g[a]=d?l*b/100:2*b});this.minPxSize=a[3]+g.minPointSize;
this.maxPxSize=Math.max(Math.min(a[2],g.maxPointSize),a[3]+g.minPointSize);d.length&&(a=q(c.zMin,r(d.filter(this.zValEval))),d=q(c.zMax,t(d.filter(this.zValEval))),this.getRadii(a,d,this.minPxSize,this.maxPxSize))},getRadii:function(a,c,d,l){var g=0,e,b=this.zData,k=b.length,m=[],p="radius"!==this.options.sizeBy,h=c-a;for(g;g<k;g++)e=this.zValEval(b[g])?b[g]:a,e<=a?e=d/2:e>=c?e=l/2:(e=0<h?(e-a)/h:.5,p&&(e=Math.sqrt(e)),e=Math.ceil(d+e*(l-d))/2),m.push(e);this.radii=m},translate:function(a){this.generatePoints();
var c=0,d=this.options,l=d.slicedOffset,g=l+(d.borderWidth||0),e,b,k,m=d.startAngle||0,p=Math.PI/180*(m-90),h=Math.PI/180*(q(d.endAngle,m+360)-90),m=h-p,u=this.points,v,r=d.dataLabels.distance,d=d.ignoreHiddenPoint,t=u.length,f,n;this.startAngleRad=p;this.endAngleRad=h;this.calculateExtremes();a||(this.center=a=this.getCenter());for(h=0;h<t;h++){f=u[h];n=this.radii[h];f.labelDistance=q(f.options.dataLabels&&f.options.dataLabels.distance,r);this.maxLabelDistance=Math.max(this.maxLabelDistance||0,f.labelDistance);
b=p+c*m;if(!d||f.visible)c+=f.percentage/100;k=p+c*m;f.shapeType="arc";f.shapeArgs={x:a[0],y:a[1],r:n,innerR:a[3]/2,start:Math.round(1E3*b)/1E3,end:Math.round(1E3*k)/1E3};b=(k+b)/2;b>1.5*Math.PI?b-=2*Math.PI:b<-Math.PI/2&&(b+=2*Math.PI);f.slicedTranslation={translateX:Math.round(Math.cos(b)*l),translateY:Math.round(Math.sin(b)*l)};e=Math.cos(b)*a[2]/2;v=Math.sin(b)*a[2]/2;k=Math.cos(b)*n;n*=Math.sin(b);f.tooltipPos=[a[0]+.7*e,a[1]+.7*v];f.half=b<-Math.PI/2||b>Math.PI/2?1:0;f.angle=b;e=Math.min(g,
f.labelDistance/5);f.labelPosition={natural:{x:a[0]+k+Math.cos(b)*f.labelDistance,y:a[1]+n+Math.sin(b)*f.labelDistance},final:{},alignment:f.half?"right":"left",connectorPosition:{breakAt:{x:a[0]+k+Math.cos(b)*e,y:a[1]+n+Math.sin(b)*e},touchingSliceAt:{x:a[0]+k,y:a[1]+n}}}}}})})(c)});
//# sourceMappingURL=variable-pie.js.map
