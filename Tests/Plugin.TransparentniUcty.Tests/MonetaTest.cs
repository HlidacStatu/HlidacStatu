using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HlidacStatu.Plugin.TransparetniUcty
{
	[TestClass]
	public class MonetaTest : TestBase
	{
		[TestMethod]
		public void VerifyReferenceItem()
		{
			RunBasicTest(
				"222065117/0600",
				"https://transparentniucty.moneta.cz/homepage/-/transparent-account/222065117",
				actual => actual.FirstOrDefault(a => a.NazevProtiuctu == "JOSEF ZICKLER - GL" &&
															a.Datum == new DateTime(2018, 9, 5) &&
															a.Castka == 20000 &&
															a.ZpravaProPrijemce == "DAR" &&
															a.VS == "560429"));
		}

	}
}
