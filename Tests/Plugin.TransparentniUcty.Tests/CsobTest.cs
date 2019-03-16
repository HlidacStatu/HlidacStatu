using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace HlidacStatu.Plugin.TransparetniUcty
{
	[TestClass]
	public class CsobTest : TestBase
	{
		[TestMethod]
		public void VerifyReferenceItem()
		{
			RunBasicTest(
				"204040987/0300",
				"https://www.csob.cz/portal/podnikatele-firmy-a-instituce/produkty/ucty-a-platebni-styk/bezne-platebni-ucty/transparentni-ucet/ucet/-/tu/204040987",
				actual => actual.FirstOrDefault(a => a.CisloProtiuctu == "0194766468/0600" &&
															a.Datum == new DateTime(2019, 1, 21) &&
															a.Castka == 100 &&
															a.ZpravaProPrijemce == "Dar" &&
															a.VS == "0000000001" &&
															a.AddId == "2007" &&
															a.NazevProtiuctu == "SEDLÁČEK VLADIMÍR"));
		}

		[TestMethod]
		public void TryToReadAllAccounts()
		{
			foreach (var item in AllAccounts)
			{
				Console.WriteLine($"Testing account {item.Item1}");
				RunBasicTest(item.Item1, item.Item2, a => a.FirstOrDefault());
			}
		}

		private readonly Tuple<string, string>[] AllAccounts = new[] {
			new Tuple<string, string>("266241143/0300","https://www.csob.cz/portal/podnikatele-firmy-a-instituce/produkty/ucty-a-platebni-styk/bezne-platebni-ucty/transparentni-ucet/ucet/-/tu/266241143"),
			new Tuple<string, string>("237198561/0300","https://www.csob.cz/portal/podnikatele-firmy-a-instituce/produkty/ucty-a-platebni-styk/bezne-platebni-ucty/transparentni-ucet/ucet/-/tu/237198561"),
			new Tuple<string, string>("177917404/0300","https://www.csob.cz/portal/podnikatele-firmy-a-instituce/produkty/ucty-a-platebni-styk/bezne-platebni-ucty/transparentni-ucet/ucet/-/tu/177917404"),
			new Tuple<string, string>("204040987/0300","https://www.csob.cz/portal/podnikatele-firmy-a-instituce/produkty/ucty-a-platebni-styk/bezne-platebni-ucty/transparentni-ucet/ucet/-/tu/204040987"),
			new Tuple<string, string>("276829741/0300","https://www.csob.cz/portal/podnikatele-firmy-a-instituce/produkty/ucty-a-platebni-styk/bezne-platebni-ucty/transparentni-ucet/ucet/-/tu/276829741")
		};

		// prevod z PDF do textu vytvori soubor, ktere neni mozne naparsovat (napr. rozhazene pozice castek, spojene radky, ...)
		private readonly Tuple<string, string>[] NotWorkingAccounts = new[] {
			new Tuple<string, string>("117527703/0300","https://www.csob.cz/portal/podnikatele-firmy-a-instituce/produkty/ucty-a-platebni-styk/bezne-platebni-ucty/transparentni-ucet/ucet/-/tu/117527703"),
			new Tuple<string, string>("478648033/0300","https://www.csob.cz/portal/podnikatele-firmy-a-instituce/produkty/ucty-a-platebni-styk/bezne-platebni-ucty/transparentni-ucet/ucet/-/tu/478648033"),
			new Tuple<string, string>("117527893/0300","https://www.csob.cz/portal/podnikatele-firmy-a-instituce/produkty/ucty-a-platebni-styk/bezne-platebni-ucty/transparentni-ucet/ucet/-/tu/117527893"),
			new Tuple<string, string>("117831473/0300","https://www.csob.cz/portal/podnikatele-firmy-a-instituce/produkty/ucty-a-platebni-styk/bezne-platebni-ucty/transparentni-ucet/ucet/-/tu/117831473"),
		};

	}
}
