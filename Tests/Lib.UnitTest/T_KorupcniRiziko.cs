﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static HlidacStatu.Lib.Analysis.KorupcniRiziko.Calculator;

namespace HlidacStatu.Lib.Tests
{
    [TestClass]
    public class T_KorupcniRiziko
    {
        
        static SmlouvyForIndex[]  stavebnictvi2017 = new SmlouvyForIndex[]{
new SmlouvyForIndex("","45274924",12136517877,1,0),
new SmlouvyForIndex("","26271303",5006948027,1,0),
new SmlouvyForIndex("","25755811",4454718332,1,0),
new SmlouvyForIndex("","60838744",3498740615,1,0),
new SmlouvyForIndex("","46342796",1783235048,1,0),
new SmlouvyForIndex("","15504158",1668306772,1,0),
new SmlouvyForIndex("","48292516",1266761242,1,0),
new SmlouvyForIndex("","45146802",1129373891,1,0),
new SmlouvyForIndex("","26177005",1022985319,1,0),
new SmlouvyForIndex("","25671464",965621355,1,0),
new SmlouvyForIndex("","46980806",850217785,1,0),
new SmlouvyForIndex("","45311005",682938958,1,0),
new SmlouvyForIndex("","25289390",606289751,1,0),
new SmlouvyForIndex("","48035599",603362322,1,0),
new SmlouvyForIndex("","25378147",510637139,1,0),
new SmlouvyForIndex("","42196868",489240391,1,0),
new SmlouvyForIndex("","46678468",423737793,1,0),
new SmlouvyForIndex("","25337220",399897484,1,0),
new SmlouvyForIndex("","46976469",388998300,1,0),
new SmlouvyForIndex("","40614948",382315709,1,0),
new SmlouvyForIndex("","25110977",374478507,1,0),
new SmlouvyForIndex("","48029483",372922691,1,0),
new SmlouvyForIndex("","45273910",293164355,1,0),
new SmlouvyForIndex("","25520059",288393903,1,0),
};

        static SmlouvyForIndex[] monopol = new SmlouvyForIndex[] {
                new SmlouvyForIndex("","a",1,1,0),
                new SmlouvyForIndex("","a",1,1,0),
                new SmlouvyForIndex("","a",1,1,0),
                new SmlouvyForIndex("","a",1,1,0),
                new SmlouvyForIndex("","a",1,1,0),
            };

        static SmlouvyForIndex[] good = new SmlouvyForIndex[] {
                new SmlouvyForIndex("","a",1,1,0),
                new SmlouvyForIndex("","b",1,1,0),
                new SmlouvyForIndex("","c",1,1,0),
                new SmlouvyForIndex("","d",1,1,0),
                new SmlouvyForIndex("","e",1,1,0),
            };

        [TestMethod]
        public void Indexy()
        {
            SmlouvyForIndex[] data = null;

            data = good;
            var v = HlidacStatu.Lib.Analysis.KorupcniRiziko.Calculator.Herfindahl_Hirschman_Index(data,1);
            Assert.AreEqual(v, 0.2m,"ideal");

            data = monopol;
            v = HlidacStatu.Lib.Analysis.KorupcniRiziko.Calculator.Herfindahl_Hirschman_Modified(data,1);
            Assert.AreEqual(v, 1- 1m/data.Count(), "ideal");


            data = monopol;
            v = Hall_Tideman_Index(data,1);
            data = good;
            v = Hall_Tideman_Index(data,1);
            //real data
            /*
   ID           CR          Deciles        Gini        HH          HK         HT         TE   
    ___________    _______    _____________    _______    _______    ________    _______    _______
    "Portfolio"    0.30648    [1×11 double]    0.59188    0.13951    0.058327    0.10209    0.68839

            HK = Hannah-Kay
            HT = Hall-Tideman
            TE = Theil entropy index
    
             */

            v = Herfindahl_Hirschman_Index(stavebnictvi2017,1);
            Assert.AreEqual(v, 0.1395145447634315720570318091M, "HHI stavebnictvi");
            // 0.1395145447634315720570318091M
            v = Hall_Tideman_Index(stavebnictvi2017,1);
            Assert.AreEqual(v, 0.1020947600539618758024922592M, "HTI stavebnictvi");
            //0.1020947600539618758024922592M

        }
    }
}
