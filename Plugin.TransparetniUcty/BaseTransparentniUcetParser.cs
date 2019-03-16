using System;
using System.Collections.Generic;
using System.Linq;
using Devmasters.Core.Logging;

namespace HlidacStatu.Plugin.TransparetniUcty
{
    public abstract class BaseTransparentniUcetParser : IParser
    {

        public static Logger TULogger = new Logger("HlidacStatu.Plugin.TransparetniUcty");

        static BaseTransparentniUcetParser()
        {
            TULogger.Info("Starting parsers");
        }

        protected IBankovniUcet Ucet = null;

        protected BaseTransparentniUcetParser(IBankovniUcet ucet)
        {
            Ucet = ucet;
            TULogger.Info($"Vytvoren parser pro ucet {ucet.CisloUctu} ({ucet.Nazev})");
        }

        public abstract string Name { get; }

        public IEnumerable<IBankovniPolozka> GetPolozky(DateTime? fromDate=null, DateTime? toDate = null)
        {
            TULogger.Info($"Zahajeni zpracovani uctu {Ucet.CisloUctu} ({fromDate?.ToString() ?? "null"} - {toDate?.ToString() ?? "null"})");
            var accountItems = DoParse(fromDate, toDate).ToArray();
            TULogger.Info($"Nacteno {accountItems.Length} polozek z uctu {Ucet.CisloUctu}");
            return accountItems;
        }

        protected abstract IEnumerable<IBankovniPolozka> DoParse(DateTime? fromDate = null, DateTime? toDate = null);
        
    }
}
