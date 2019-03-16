/*
 Highcharts JS v7.0.0 (2018-12-11)
 Highcharts cylinder module

 (c) 2010-2018 Kacper Madej

 License: www.highcharts.com/license
*/
(function(h){"object"===typeof module&&module.exports?module.exports=h:"function"===typeof define&&define.amd?define(function(){return h}):h("undefined"!==typeof Highcharts?Highcharts:void 0)})(function(h){(function(e){var h=e.charts,k=e.color,q=e.deg2rad,r=e.perspective,t=e.seriesType,g=e.Renderer.prototype,u=g.cuboidPath;t("cylinder","column",{},{},{shapeType:"cylinder"});e=e.merge(g.elements3d.cuboid,{parts:["top","bottom","front","back"],pathType:"cylinder",fillSetter:function(a){this.singleSetterForParts("fill",
null,{front:a,back:a,top:k(a).brighten(.1).get(),bottom:k(a).brighten(-.1).get()});this.color=this.fill=a;return this}});g.elements3d.cylinder=e;g.cylinder=function(a){return this.element3d("cylinder",a)};g.cylinderPath=function(a){var b=h[this.chartIndex],c=u.call(this,a),d=!c.isTop,e=!c.isFront,f=this.getCylinderEnd(b,a);a=this.getCylinderEnd(b,a,!0);return{front:this.getCylinderFront(f,a),back:this.getCylinderBack(f,a),top:f,bottom:a,zIndexes:{top:d?3:0,bottom:d?0:3,front:e?2:1,back:e?1:2,group:c.zIndexes.group}}};
g.getCylinderFront=function(a,b){a=a.slice(0,a.simplified?9:17);a.push("L");b.simplified?(a=a.concat(b.slice(7,9)).concat(b.slice(3,6)).concat(b.slice(0,3)),a[a.length-3]="L"):a.push(b[15],b[16],"C",b[13],b[14],b[11],b[12],b[8],b[9],"C",b[6],b[7],b[4],b[5],b[1],b[2]);a.push("Z");return a};g.getCylinderBack=function(a,b){var c=["M"];b.simplified?(c=c.concat(a.slice(7,12)),c.push("L",a[1],a[2])):c=c.concat(a.slice(15));c.push("L");b.simplified?c=c.concat(b.slice(1,3)).concat(b.slice(9,12)).concat(b.slice(6,
9)):c.push(b[29],b[30],"C",b[27],b[28],b[25],b[26],b[22],b[23],"C",b[20],b[21],b[18],b[19],b[15],b[16]);c.push("Z");return c};g.getCylinderEnd=function(a,b,c){var d=Math.min(b.width,b.depth)/2,e=q*(a.options.chart.options3d.beta-90);c=b.y+(c?b.height:0);var f=.5519*d,g=b.width/2+b.x,h=b.depth/2+b.z,l=[{x:0,y:c,z:d},{x:f,y:c,z:d},{x:d,y:c,z:f},{x:d,y:c,z:0},{x:d,y:c,z:-f},{x:f,y:c,z:-d},{x:0,y:c,z:-d},{x:-f,y:c,z:-d},{x:-d,y:c,z:-f},{x:-d,y:c,z:0},{x:-d,y:c,z:f},{x:-f,y:c,z:d},{x:0,y:c,z:d}],k=Math.cos(e),
p=Math.sin(e),m,n;l.forEach(function(a,b){m=a.x;n=a.z;l[b].x=m*k-n*p+g;l[b].z=n*k+m*p+h});a=r(l,a,!0);2.5>Math.abs(a[3].y-a[9].y)?(a=this.toLinePath([a[0],a[3],a[6],a[9]],!0),a.simplified=!0):a=this.getCurvedPath(a);return a};g.getCurvedPath=function(a){var b=["M",a[0].x,a[0].y],c=a.length-2,d;for(d=1;d<c;d+=3)b.push("C",a[d].x,a[d].y,a[d+1].x,a[d+1].y,a[d+2].x,a[d+2].y);return b}})(h)});
//# sourceMappingURL=cylinder.js.map
