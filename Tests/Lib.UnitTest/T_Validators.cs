using HlidacStatu.Lib;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HlidacStatu.Lib.Tests
{
    [TestClass()]
    public class Validators
    {
        [TestMethod()]
        public void FirmaInTextTest()
        {
            var fn = "STRATA S.R.O.";
            var f = HlidacStatu.Lib.Validators.FirmaInText(fn);
            Assert.AreEqual(f?.ICO, "27963888","Strata error");

        }
        [TestMethod()]
        public void OsobaInTextTest()
        {
            var fn = "Michal Bláha";
            var f = HlidacStatu.Lib.Validators.JmenoInText(fn);
            Assert.AreEqual(f?.Jmeno, "Michal", "Osoba1 error");
            Assert.AreEqual(f?.Prijmeni, "Bláha", "Osoba1 error");


            fn = "Bláha Michal";
            f = HlidacStatu.Lib.Validators.JmenoInText(fn);
            Assert.AreEqual(f?.Jmeno, "Michal", "Osoba2 error");
            Assert.AreEqual(f?.Prijmeni, "Bláha", "Osoba2 error");

            fn = "Doskocilova";
            f = HlidacStatu.Lib.Validators.JmenoInText(fn);
            Assert.AreEqual(f, null, "Osoba3 error");

        }
    }
}

