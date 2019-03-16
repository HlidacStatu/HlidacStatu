using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HlidacStatu.Plugin.TransparetniUcty
{
	public abstract class TestBase
	{
		protected void RunBasicTest(string accountNumber, string url, Func<IBankovniPolozka[], IBankovniPolozka> referenceItemFinder)
		{
			var ucet = new SimpleBankovniUcet { CisloUctu = accountNumber, Url = url };
			var parser = new AutoParser(ucet);
			var actual = parser.GetPolozky(DateTime.Now.AddYears(-1)).ToArray();

			Assert.IsTrue(actual.Length > 0);

			var referenceItem = referenceItemFinder(actual);

			Assert.IsNotNull(referenceItem);
		}
	}
}
