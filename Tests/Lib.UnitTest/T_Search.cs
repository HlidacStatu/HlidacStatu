using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HlidacStatu.Lib;
using HlidacStatu.Lib.Search.Rules;

namespace HlidacStatu.Lib.Tests
{
    [TestClass]
    public class T_Search
    {

        Search.Rules.IRule r_osobaId = new Search.Rules.OsobaId("osobaid:","ico:");
        Search.Rules.IRule r_holding = new Search.Rules.Holding(null, "ico:");
        Search.Rules.IRule r_holding2 = new Search.Rules.Holding("holdingprijemce:", "icoprijemce:");
        Search.Rules.IRule r_ico_tpwv = new Search.Rules.TransformPrefixWithValue("ico:", " ( prijemce.ico:${q} OR platce.ico:${q} ) ", null);
        Search.Rules.IRule r_podeps_tpwv1 = new Search.Rules.TransformPrefixWithValue("podepsano:", " datumUzavreni:[${q} TO ${q}||+1d] ", "\\d+");
        Search.Rules.IRule r_zverejn_tps = new Search.Rules.TransformPrefix("zverejneno:", "casZverejneni:", "[<>]?[{\\[]+");
        Search.Rules.IRule r_schyby = new Search.Rules.Smlouva_Chyby();
        Search.Rules.IRule r_cpv = new Search.Rules.VZ_CPV();
        Search.Rules.IRule r_form = new Search.Rules.VZ_Form();
        Search.Rules.IRule r_obl = new Search.Rules.VZ_Oblast();

        [TestMethod]
        public void TestSearchRules()
        {

            Assert.IsNull(r_osobaId.Process(Search.SplittingQuery.SplitQuery("").Parts.FirstOrDefault()));
            Assert.IsNull(r_holding.Process(Search.SplittingQuery.SplitQuery("").Parts.FirstOrDefault()));
            Assert.IsNull(r_holding2.Process(Search.SplittingQuery.SplitQuery("").Parts.FirstOrDefault()));
            Assert.IsNull(r_ico_tpwv.Process(Search.SplittingQuery.SplitQuery("").Parts.FirstOrDefault()));
            Assert.IsNull(r_podeps_tpwv1.Process(Search.SplittingQuery.SplitQuery("").Parts.FirstOrDefault()));
            Assert.IsNull(r_zverejn_tps.Process(Search.SplittingQuery.SplitQuery("").Parts.FirstOrDefault()));
            Assert.IsNull(r_schyby.Process(Search.SplittingQuery.SplitQuery("").Parts.FirstOrDefault()));
            Assert.IsNull(r_form.Process(Search.SplittingQuery.SplitQuery("").Parts.FirstOrDefault()));
            Assert.IsNull(r_obl.Process(Search.SplittingQuery.SplitQuery("").Parts.FirstOrDefault()));

            Assert.IsNull(r_osobaId.Process(Search.SplittingQuery.SplitQuery("autobus").Parts.FirstOrDefault()));
            Assert.IsNull(r_holding.Process(Search.SplittingQuery.SplitQuery("autobus").Parts.FirstOrDefault()));
            Assert.IsNull(r_holding2.Process(Search.SplittingQuery.SplitQuery("autobus").Parts.FirstOrDefault()));
            Assert.IsNull(r_ico_tpwv.Process(Search.SplittingQuery.SplitQuery("autobus").Parts.FirstOrDefault()));
            Assert.IsNull(r_podeps_tpwv1.Process(Search.SplittingQuery.SplitQuery("autobus").Parts.FirstOrDefault()));
            Assert.IsNull(r_zverejn_tps.Process(Search.SplittingQuery.SplitQuery("autobus").Parts.FirstOrDefault()));
            Assert.IsNull(r_schyby.Process(Search.SplittingQuery.SplitQuery("autobus").Parts.FirstOrDefault()));
            Assert.IsNull(r_form.Process(Search.SplittingQuery.SplitQuery("autobus").Parts.FirstOrDefault()));
            Assert.IsNull(r_obl.Process(Search.SplittingQuery.SplitQuery("autobus").Parts.FirstOrDefault()));

            Assert.AreEqual<string>("( ( ico:04711157 ) OR ( ico:02795281 ) OR ( ico:04156528 ) OR ( ico:25854631 ) OR ( ico:27233545 ) OR ( ico:28275918 ) OR ( ico:29010969 ) OR ( ico:08178607 ) OR ( ico:05965527 ) OR ( ico:27169685 ) OR ( ico:04096908 ) )",
                r_osobaId.Process(Search.SplittingQuery.SplitQuery("osobaId:michal-blaha-2").Parts.FirstOrDefault())?.Query?.FullQuery());

            var res_holding = r_holding.Process(Search.SplittingQuery.SplitQuery("holding:25854631").Parts.FirstOrDefault())?.Query?.FullQuery();
            Assert.AreEqual<string>("( ( ico:25854631 ) OR ( ico:04096908 ) )", res_holding);

            var res_holding2 = r_holding2.Process(Search.SplittingQuery.SplitQuery("holdingprijemce:25854631").Parts.FirstOrDefault())?.Query?.FullQuery();
            Assert.AreEqual<string>("( ( icoprijemce:25854631 ) OR ( icoprijemce:04096908 ) )", res_holding2);

            Assert.IsNull(r_osobaId.Process(Search.SplittingQuery.SplitQuery("holding:25854631").Parts.FirstOrDefault()));

            Assert.AreEqual<string>("( prijemce.ico:04711157 OR platce.ico:04711157 )",
                r_ico_tpwv.Process(Search.SplittingQuery.SplitQuery("ico:04711157").Parts.FirstOrDefault())?.Query?.FullQuery());


            Assert.AreEqual<string>("datumUzavreni:[2019-01-01 TO 2019-01-01||+1d]",
                r_podeps_tpwv1.Process(Search.SplittingQuery.SplitQuery("podepsano:2019-01-01").Parts.FirstOrDefault())?.Query?.FullQuery());

            Assert.AreEqual<string>("casZverejneni:{2019-01-01 TO 2019-01-02}",
                r_zverejn_tps.Process(Search.SplittingQuery.SplitQuery("zverejneno:{2019-01-01 TO 2019-01-02}").Parts.FirstOrDefault())?.Query?.FullQuery());


            Assert.AreEqual<string>("( issues.issueTypeId:1 OR issues.issueTypeId:3 OR issues.issueTypeId:8 OR issues.issueTypeId:21 OR issues.issueTypeId:22 )",
                r_schyby.Process(Search.SplittingQuery.SplitQuery("chyby:zasadni").Parts.FirstOrDefault())?.Query?.FullQuery());

            Assert.AreEqual<string>("( cPV:15* OR cPV:16* )",
                r_cpv.Process(Search.SplittingQuery.SplitQuery("cpv:15,16").Parts.FirstOrDefault())?.Query?.FullQuery());

            Assert.IsNull(r_cpv.Process(Search.SplittingQuery.SplitQuery("cpv:15*").Parts.FirstOrDefault())?.Query?.FullQuery());

            Assert.AreEqual<string>("( formulare.druh:( 1* OR 2* ) )",
                r_form.Process(Search.SplittingQuery.SplitQuery("form:1,2").Parts.FirstOrDefault())?.Query?.FullQuery());

            Assert.AreEqual<string>("( cPV:70* OR cPV:791* OR cPV:7524* )",
            r_obl.Process(Search.SplittingQuery.SplitQuery("oblast:legal").Parts.FirstOrDefault())?.Query?.FullQuery());



            //Assert.AreEqual<string>("( (  ( (prijemce.ico:25854631 OR platce.ico:25854631) )  OR  ( (prijemce.ico:04096908 OR platce.ico:04096908) )  ) )",
            //    r_ico_tpwv.Process(Search.SplittingQuery.SplitQuery(res_holding).Parts.FirstOrDefault())?.Query?.FullQuery());

        }
        [TestMethod]
        public void TestSimpleSearch()
        {

            IRule[] rules = new IRule[] {
               new OsobaId("osobaid:","ico:" ),
               new Holding("holdingprijemce:","icoprijemce:" ),
               new Holding("holdingplatce:","icoplatce:" ),
               new Holding("holdingdodavatel:","icoprijemce:" ),
               new Holding("holdingzadavatel:","icoplatce:" ),
               new Holding(null,"ico:" ),

               new TransformPrefixWithValue("ds:","(prijemce.datovaSchranka:${q} OR platce.datovaSchranka:${q}) ",null ),
               new TransformPrefix("dsprijemce:","prijemce.datovaSchranka:",null  ),
               new TransformPrefix("dsplatce:","platce.datovaSchranka:",null  ),
               new TransformPrefixWithValue("ico:","(prijemce.ico:${q} OR platce.ico:${q}) ",null ),
               new TransformPrefix("icoprijemce:","prijemce.ico:",null ),
               new TransformPrefix("icoplatce:","platce.ico:",null ),
               new TransformPrefix("jmenoprijemce:","prijemce.nazev:",null ),
               new TransformPrefix("jmenoplatce:","platce.nazev:",null ),
               new TransformPrefix("id:","id:",null ),
               new TransformPrefix("idverze:","id:",null ),
               new TransformPrefix("idsmlouvy:","identifikator.idSmlouvy:",null ),
               new TransformPrefix("predmet:","predmet:",null ),
               new TransformPrefix("cislosmlouvy:","cisloSmlouvy:",null ),
               new TransformPrefix("mena:","ciziMena.mena:",null ),
               new TransformPrefix("cenasdph:","hodnotaVcetneDph:",null ),
               new TransformPrefix("cenabezdph:","hodnotaBezDph:",null ),
               new TransformPrefix("cena:","calculatedPriceWithVATinCZK:",null ),
               new TransformPrefix("zverejneno:","casZverejneni:", "[<>]?[{\\[]+" ),
               new TransformPrefix("zverejneno:","casZverejneni:", "[<>]?[{\\[]+" ),
               new TransformPrefixWithValue("zverejneno:","casZverejneni:[${q} TO ${q}||+1d]", "\\d+" ),
               new TransformPrefix("podepsano:","datumUzavreni:[", "[<>]?[{\\[]+" ),
               new TransformPrefixWithValue("podepsano:","datumUzavreni:${q}", "[<>]?[{\\[]+" ),
               new TransformPrefixWithValue("podepsano:","datumUzavreni:[${q} TO ${q}||+1d]", "\\d+"  ),
               new TransformPrefix("schvalil:","schvalil:",null ),
               new TransformPrefix("textsmlouvy:","prilohy.plainTextContent:",null ),
               new Smlouva_Chyby(),

            };

            Assert.AreEqual<string>("cpv:15,16",
                Search.SimpleQueryCreator.GetSimpleQuery("cpv:15,16", rules).FullQuery());

            Assert.AreEqual<string>("( ( ( prijemce.ico:04711157 OR platce.ico:04711157 ) ) OR ( ( prijemce.ico:02795281 OR platce.ico:02795281 ) ) OR ( ( prijemce.ico:04156528 OR platce.ico:04156528 ) ) OR ( ( prijemce.ico:25854631 OR platce.ico:25854631 ) ) OR ( ( prijemce.ico:27233545 OR platce.ico:27233545 ) ) OR ( ( prijemce.ico:28275918 OR platce.ico:28275918 ) ) OR ( ( prijemce.ico:29010969 OR platce.ico:29010969 ) ) OR ( ( prijemce.ico:08178607 OR platce.ico:08178607 ) ) OR ( ( prijemce.ico:05965527 OR platce.ico:05965527 ) ) OR ( ( prijemce.ico:27169685 OR platce.ico:27169685 ) ) OR ( ( prijemce.ico:04096908 OR platce.ico:04096908 ) ) )",
                Search.SimpleQueryCreator.GetSimpleQuery("osobaId:michal-blaha-2", rules).FullQuery());

            Assert.AreEqual<string>("( prijemce.ico:00551023 OR platce.ico:00551023 ) AND ( prijemce.ico:27233545 OR platce.ico:27233545 )",
                Search.SimpleQueryCreator.GetSimpleQuery("ico:00551023 AND ico:27233545", rules).FullQuery());

            Assert.AreEqual<string>("( prijemce.ico:27233545 OR platce.ico:27233545 ) nalezení částí systému s nízkou výkonností",
                Search.SimpleQueryCreator.GetSimpleQuery("ico:27233545  nalezení částí systému s nízkou výkonností", rules).FullQuery());

            Assert.AreEqual<string>("( prijemce.ico:27233545 OR platce.ico:27233545 ) \"nalezení částí systému s nízkou výkonností\"",
                Search.SimpleQueryCreator.GetSimpleQuery("ico:27233545  \"nalezení částí systému s nízkou výkonností\"", rules).FullQuery());

            Assert.AreEqual<string>("( prijemce.ico:27233545 OR platce.ico:27233545 ) classification.types.typeValue:10005",
                Search.SimpleQueryCreator.GetSimpleQuery("ico:27233545 classification.types.typeValue:10005", rules).FullQuery());


            Assert.AreEqual<string>("( prijemce.ico:27233545 OR platce.ico:27233545 ) calculatedPriceWithVATinCZK:>2000 NEN",
                Search.SimpleQueryCreator.GetSimpleQuery("ico:27233545 cena:>2000 NEN", rules).FullQuery());


            Assert.AreEqual<string>("podepsano:2019-01-01",
                Search.SimpleQueryCreator.GetSimpleQuery("podepsano:2019-01-01", rules).FullQuery());


        }

    }
}
