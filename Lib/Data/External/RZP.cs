using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devmasters;
using System.Net.Http;

namespace HlidacStatu.Lib.Data.External
{
    public partial class RZP
    {

        public static Firma FromIco(string ico)
        {

            var resp = Manager.RawSearchIco(ico);
            if (resp == null)
                return Firma.LoadError;

            if (resp.Items
                    .Where(m => m.GetType() == typeof(TPodnikatelSeznam))
                    .Count() > 0)
            {
                TPodnikatelSeznam xf = resp.Items
                    .Where(m => m.GetType() == typeof(TPodnikatelSeznam))
                    .First() as TPodnikatelSeznam;

                Firma f = new Data.Firma();
                f.Jmeno = xf.ObchodniJmenoSeznam.Value;
                f.ICO = ico;

                var detail = Manager.RawSearchDetail(xf.PodnikatelID);
                if (detail == null)
                {
                    f.Source = "RZP";
                    f.Popis = "Živnostník";
                    return f;
                }

                TPodnikatelVypis vypis = detail.Items
                    .Where(m => m.GetType() == typeof(TPodnikatelVypis))
                    .First() as TPodnikatelVypis;

                if (vypis.PodnikatelDetail?.SeznamZivnosti?.Zivnost?.Count() > 0)
                {
                    f.Datum_Zapisu_OR = vypis.PodnikatelDetail.SeznamZivnosti.Zivnost
                        .Where(m => !string.IsNullOrEmpty(m.Vznik.Value))
                        .Select(m => DateTime.ParseExact(m.Vznik.Value, "dd.MM.yyyy", HlidacStatu.Util.Consts.czCulture))
                        .Min();

                    f.Source = "RZP";
                    f.Popis = "Živnostník";
                }
                f.RefreshDS();
                f.Save();
                return f;

            }
            else
            return Firma.NotFound;

        }



    }
}