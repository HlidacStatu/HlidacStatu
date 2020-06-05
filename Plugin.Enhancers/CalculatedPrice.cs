using Devmasters.Core.Collections;
using HlidacStatu.Lib;
using HlidacStatu.Lib.Enhancers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Plugin.Enhancers
{


    public class CalculatedPrice : IEnhancer
    {
        public string Description
        {
            get
            {
                return "CalculatedPrice updated";
            }
        }

        public string Name
        {
            get
            {
                return "CalculatedPrice updated";
            }
        }
        public void SetInstanceData(object data)
        {
        }

        public bool Update(ref Lib.Data.Smlouva item)
        {
            Base.Logger.Debug("Starting CalculatedPrice for " + item.Id);
            bool changed = false;
            if (item.hodnotaVcetneDph.HasValue && item.hodnotaVcetneDph > 0)
            {
                item.CalculatedPriceWithVATinCZK = item.hodnotaVcetneDph.Value;
                item.CalcutatedPriceQuality = DataQualityEnum.Exact;
                changed = true;
            }
            else if (item.hodnotaBezDph.HasValue && item.hodnotaBezDph > 0)
            {
                item.CalculatedPriceWithVATinCZK = item.hodnotaBezDph.Value * 1.21m;
                item.CalcutatedPriceQuality = DataQualityEnum.Calculated;
                changed = true;
            }
            else if (item.ciziMena != null && item.ciziMena.hodnota > 0)
            {
                //preved Menu na CZK
                if (item.ciziMena.mena.ToUpper() == "CZK")
                {
                    item.CalculatedPriceWithVATinCZK = item.ciziMena.hodnota;
                    item.CalcutatedPriceQuality = DataQualityEnum.Exact;
                    changed = true;
                }
                else
                {
                    DateTime date = item.datumUzavreni;
                    decimal exr = GetCNBExchangeRate(date, item.ciziMena.mena);
                    if (exr > 0)
                    {
                        item.CalculatedPriceWithVATinCZK = item.ciziMena.hodnota * exr;
                        item.CalcutatedPriceQuality = DataQualityEnum.Calculated;
                        item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Dopočítána cena v CZK ze zahraniční měny", "","", "", "", this));
                        changed = true;
                    }
                    else
                        Base.Logger.Warning("Date: " + date.ToShortDateString() + " " + item.ciziMena.mena + " is unknown");

                }

            }
            return changed;
        }


        private decimal GetCNBExchangeRate(DateTime date, string currency)
        {
            //https://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.txt?date=12.08.2008
            currency = currency.ToUpper();
            string sdate = date.Date.ToString("dd.MM.yyyy");
            string url = "https://www.cnb.cz/cs/financni-trhy/devizovy-trh/kurzy-devizoveho-trhu/kurzy-devizoveho-trhu/denni_kurz.txt?date=" + sdate;

// old URL "https://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.txt?date=" + sdate; 

            try
            {

                System.Net.WebClient wc = new System.Net.WebClient();
                string table = wc.DownloadString(url);
                /*
    04.08.2008 #151
    země|měna|množství|kód|kurz
    Austrálie|dolar|1|AUD|14,366
    Brazílie|real|1|BRL|9,864
    Bulharsko|lev|1|BGN|12,266
    Čína|renminbi|1|CNY|2,249
    Dánsko|koruna|1|DKK|3,216
    EMU|euro|1|EUR|23,990
    Estonsko|koruna|1|EEK|1,533
                 */
                foreach (var line in table.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] cols = line.Split('|');
                    if (cols.Length == 5)
                    {
                        if (cols[3] == currency)
                        {
                            decimal amount = decimal.Parse(cols[2]);
                            decimal exr = decimal.Parse(cols[4], System.Globalization.CultureInfo.GetCultureInfo("cs-CZ"));
                            return exr / amount;
                        }
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                Base.Logger.Error("Date: " + date.ToShortDateString() + " " + currency + " is unknown",e);

                return 0;
            }
        }

    }
}
