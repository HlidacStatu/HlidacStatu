using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HlidacStatu.Plugin.TransparetniUcty
{
	[TestClass]
	public class KbTest : TestBase
	{
		[TestMethod]
		public void VerifyReferenceItem()
		{
			RunBasicTest(
				"115-4449010207/0100",
				"https://www.kb.cz/cs/transparentni-ucty/snk-evropsti-demokrate-snk-evropsti-demokrate-czk",
				actual => actual.FirstOrDefault(a => a.Datum == new DateTime(2018, 10, 30) &&
															a.Castka == 15000 &&
															a.NazevProtiuctu == "ĎURČANSKÝ JOZEF" &&
															a.CisloProtiuctu == "9222820267/100" &&
															a.PopisTransakce == "PLATBA VE PROSPĚCH VAŠEHO ÚČTU" &&
															a.ZpravaProPrijemce == "DAR" &&
															a.VS == "471025752"));
		}
	}
}
