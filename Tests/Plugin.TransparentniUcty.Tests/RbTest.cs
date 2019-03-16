using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HlidacStatu.Plugin.TransparetniUcty
{
	[TestClass]
	public class RbTest : TestBase
	{
		[TestMethod]
		public void VerifyReferenceItem()
		{
			RunBasicTest(
				"9711283001/5500",
				"https://www.rb.cz/cs/o-nas/povinne-uverejnovane-informace/transparentni-ucty?mvcPath=transactions&accountNumber=9711283001",
				actual => actual.FirstOrDefault(a => a.Datum == new DateTime(2019, 2, 19) &&
														  a.PopisTransakce == "Převod" &&
														  a.Castka == new decimal(4200.28) &&
														  a.NazevProtiuctu == "Nadace VIA" &&
														  a.ZpravaProPrijemce == "PLATBA DARUJME.CZ" &&
														  a.VS == "190219090" &&
														  a.KS == "558"));
		}

	}
}
