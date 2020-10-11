/*
 Highcharts JS v8.2.0 (2020-08-20)

 (c) 2009-2019 Torstein Honsi

 License: www.highcharts.com/license
*/
(function(a){"object"===typeof module&&module.exports?(a["default"]=a,module.exports=a):"function"===typeof define&&define.amd?define("highcharts/themes/sand-signika",["highcharts"],function(b){a(b);a.Highcharts=b;return a}):a("undefined"!==typeof Highcharts?Highcharts:void 0)})(function(a){function b(a,c,b,d){a.hasOwnProperty(c)||(a[c]=d.apply(null,b))}a=a?a._modules:{};b(a,"Extensions/Themes/SandSignika.js",[a["Core/Globals.js"],a["Core/Utilities.js"]],function(a,b){b=b.setOptions;a.createElement("link",
{href:"https://fonts.googleapis.com/css?family=Signika:400,700",rel:"stylesheet",type:"text/css"},null,document.getElementsByTagName("head")[0]);a.addEvent(a.Chart,"afterGetContainer",function(){this.container.style.background="url(https://www.highcharts.com/samples/graphics/sand.png)"});a.theme={colors:"#f45b5b #8085e9 #8d4654 #7798BF #aaeeee #ff0066 #eeaaee #55BF3B #DF5353 #7798BF #aaeeee".split(" "),chart:{backgroundColor:null,style:{fontFamily:"Signika, serif"}},title:{style:{color:"black",fontSize:"16px",
fontWeight:"bold"}},subtitle:{style:{color:"black"}},tooltip:{borderWidth:0},labels:{style:{color:"#6e6e70"}},legend:{backgroundColor:"#E0E0E8",itemStyle:{fontWeight:"bold",fontSize:"13px"}},xAxis:{labels:{style:{color:"#6e6e70"}}},yAxis:{labels:{style:{color:"#6e6e70"}}},plotOptions:{series:{shadow:!0},candlestick:{lineColor:"#404048"},map:{shadow:!1}},navigator:{xAxis:{gridLineColor:"#D0D0D8"}},rangeSelector:{buttonTheme:{fill:"white",stroke:"#C0C0C8","stroke-width":1,states:{select:{fill:"#D0D0D8"}}}},
scrollbar:{trackBorderColor:"#C0C0C8"}};b(a.theme)});b(a,"masters/themes/sand-signika.src.js",[],function(){})});
//# sourceMappingURL=sand-signika.js.map