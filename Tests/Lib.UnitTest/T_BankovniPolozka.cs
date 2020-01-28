using System;
using HlidacStatu.Lib.Data.TransparentniUcty;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HlidacStatu.Lib.Tests
{
    [TestClass]
    public class T_BankovniPolozka
    {
        [TestMethod]
        public void NormalizeCisloUctu()
        {

            Assert.AreEqual(BankovniUcet.NormalizeCisloUctu("000000-0020091122/0800"), "20091122/0800", "BU1 error");
            Assert.AreEqual(BankovniUcet.NormalizeCisloUctu("0-0020091122/0800"), "20091122/0800", "BU2 error");
            Assert.AreEqual(BankovniUcet.NormalizeCisloUctu("-0020091122/0800"), "20091122/0800", "BU3 error");
            Assert.AreEqual(BankovniUcet.NormalizeCisloUctu("0020091122/0800"), "20091122/0800", "BU4 error");
            Assert.AreEqual(BankovniUcet.NormalizeCisloUctu("20091122/0800"), "20091122/0800", "BU5 error");


            Assert.AreEqual(BankovniUcet.NormalizeCisloUctu("002000-0020091122/0800"), "2000-20091122/0800", "BU6 error");

        }
    }
}
