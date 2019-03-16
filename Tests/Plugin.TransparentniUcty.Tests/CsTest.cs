using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Plugin.TransparetniUcty
{
	[TestClass]
	public class CsTest : TestBase
	{
		[TestMethod]
		public void VerifyReferenceItem()
		{
			RunBasicTest(
				"7251232/0800",
				"https://www.csas.cz/banka/appmanager/portal/banka?_nfpb=true&_pageLabel=transacc&accNumber=000000-0007251232&docid=internet/cs/transparentni_ucet_0007251232.xml",
				actual => actual.FirstOrDefault(a => a.Datum == new DateTime(2018, 5, 23) &&
														  a.PopisTransakce == "DOŠLÁ PLATBA" &&
														  a.Castka == 10000 &&
														  a.NazevProtiuctu == "Ondřej Brdíčko" &&
														  a.ZpravaProPrijemce == "Brdíčko - Dar na komunální volby2018 Karviná" &&
														  a.VS == "6611100364"));
		}
	}

}
