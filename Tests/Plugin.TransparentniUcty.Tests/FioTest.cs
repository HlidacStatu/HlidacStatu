using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HlidacStatu.Plugin.TransparetniUcty
{
	[TestClass]
	public class FioTest : TestBase
	{
		[TestMethod]
		public void VerifyReferenceItem()
		{
			RunBasicTest(
				"2701199023/2010",
				"https://www.fio.cz/ib2/transparent?a=2701199023",
				actual => actual.FirstOrDefault(a => a.Datum == new DateTime(2018, 5, 23) &&
												a.PopisTransakce == "Bezhotovostní příjem" &&
												a.Castka == 1372 &&
												a.NazevProtiuctu == "Nadace VIA" &&
												a.ZpravaProPrijemce.Contains("PLATBA DARUJME.CZ") &&
												a.VS == "180522108" &&
												a.KS == "0558")
				);
		}
	}
}
