using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HlidacStatu.Lib;

namespace HlidacStatu.Lib.Tests
{
    [TestClass]
    public class T_Search
    {
        Search.Rules.IRule r_osobaId = new Search.Rules.OsobaId("ico");
        Search.Rules.IRule r_holding = new Search.Rules.Holding("ico:");
        Search.Rules.IRule r_tpwv = new Search.Rules.TransformPrefixWithValue("ico:", " ( prijemce.ico:${q} OR platce.ico:${q} ) ", null);
        Search.Rules.IRule r_tpwv1 = new Search.Rules.TransformPrefixWithValue("podepsano:", " datumUzavreni:[${q} TO ${q}||+1d] ", "(?=\\d)");
        Search.Rules.IRule r_tps = new Search.Rules.TransformPrefixSimple("zverejneno:", "casZverejneni:", "(?=[<>])");
        Search.Rules.IRule r_schyby = new Search.Rules.Smlouva_Chyby();
        Search.Rules.IRule r_cpv = new Search.Rules.VZ_CPV();
        Search.Rules.IRule r_form = new Search.Rules.VZ_Form();
        Search.Rules.IRule r_obl = new Search.Rules.VZ_Oblast();

        [TestMethod]
        public void TestSearchRules()
        {

            Assert.IsNull(r_osobaId.Process(Search.SplittingQuery.SplitQuery("").Parts.FirstOrDefault()));
            Assert.IsNull(r_holding.Process(Search.SplittingQuery.SplitQuery("").Parts.FirstOrDefault()));
            Assert.IsNull(r_tpwv.Process(Search.SplittingQuery.SplitQuery("").Parts.FirstOrDefault()));
            Assert.IsNull(r_tpwv1.Process(Search.SplittingQuery.SplitQuery("").Parts.FirstOrDefault()));
            Assert.IsNull(r_tps.Process(Search.SplittingQuery.SplitQuery("").Parts.FirstOrDefault()));
            Assert.IsNull(r_schyby.Process(Search.SplittingQuery.SplitQuery("").Parts.FirstOrDefault()));
            Assert.IsNull(r_form.Process(Search.SplittingQuery.SplitQuery("").Parts.FirstOrDefault()));
            Assert.IsNull(r_obl.Process(Search.SplittingQuery.SplitQuery("").Parts.FirstOrDefault()));

            Assert.IsNull(r_osobaId.Process(Search.SplittingQuery.SplitQuery("autobus").Parts.FirstOrDefault()));
            Assert.IsNull(r_holding.Process(Search.SplittingQuery.SplitQuery("autobus").Parts.FirstOrDefault()));
            Assert.IsNull(r_tpwv.Process(Search.SplittingQuery.SplitQuery("autobus").Parts.FirstOrDefault()));
            Assert.IsNull(r_tpwv1.Process(Search.SplittingQuery.SplitQuery("autobus").Parts.FirstOrDefault()));
            Assert.IsNull(r_tps.Process(Search.SplittingQuery.SplitQuery("autobus").Parts.FirstOrDefault()));
            Assert.IsNull(r_schyby.Process(Search.SplittingQuery.SplitQuery("autobus").Parts.FirstOrDefault()));
            Assert.IsNull(r_form.Process(Search.SplittingQuery.SplitQuery("autobus").Parts.FirstOrDefault()));
            Assert.IsNull(r_obl.Process(Search.SplittingQuery.SplitQuery("autobus").Parts.FirstOrDefault()));

            Assert.AreEqual<string>("( ( ico:04711157 ) OR ( ico:02795281 ) OR ( ico:04156528 ) OR ( ico:25854631 ) OR ( ico:27233545 ) OR ( ico:28275918 ) OR ( ico:29010969 ) OR ( ico:08178607 ) OR ( ico:05965527 ) OR ( ico:27169685 ) OR ( ico:04096908 ) )", 
                r_osobaId.Process(Search.SplittingQuery.SplitQuery("osobaId:michal-blaha-2").Parts.FirstOrDefault()).Query.FullQuery);

            Assert.AreEqual<string>("( (  ( (prijemce.ico:25854631 OR platce.ico:25854631) )  OR  ( (prijemce.ico:04096908 OR platce.ico:04096908) )  ) )",
                r_osobaId.Process(Search.SplittingQuery.SplitQuery("holding:25854631").Parts.FirstOrDefault()).Query.FullQuery);

            Assert.AreEqual<string>(" ( prijemce.ico:04711157 OR platce.ico:04711157 ) ) ",
                r_osobaId.Process(Search.SplittingQuery.SplitQuery("ico:04711157").Parts.FirstOrDefault()).Query.FullQuery);

            Assert.AreEqual<string>("datumUzavreni:[2019-01-01 TO 2019-01-01||+1d]",
                r_osobaId.Process(Search.SplittingQuery.SplitQuery("podepsano:2019-01-01").Parts.FirstOrDefault()).Query.FullQuery);

            Assert.AreEqual<string>("casZverejneni:[2019-01-01 TO 2019-12-31]",
                r_osobaId.Process(Search.SplittingQuery.SplitQuery("zverejneno:[2019-01-01 TO 2019-12-31]").Parts.FirstOrDefault()).Query.FullQuery);

            Assert.AreEqual<string>("casZverejneni:{2019-01-01 TO 2019-12-31}",
                r_osobaId.Process(Search.SplittingQuery.SplitQuery("zverejneno:{2019-01-01 TO 2019-12-31}").Parts.FirstOrDefault()).Query.FullQuery);
        }
    }
}
