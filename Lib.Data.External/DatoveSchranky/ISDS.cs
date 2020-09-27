using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Devmasters;
using Devmasters.Enums;

namespace HlidacStatu.Lib.Data.External.DatoveSchranky
{
    public class ISDS
    {

        public class Osoba
        {
            public class DSAdresa
            {
                public string DS { get; set; }
                public string AdresaFull { get; set; }
                public string Okres { get; set; }
                public string Obec { get; set; }
                public string CastObce { get; set; }
                public string Ulice { get; set; }
                public string PSC { get; set; }
                public string TypCisla { get; set; }
                public string CisloDomovni { get; set; }
                public string CisloOrientacni { get; set; }
                public string CisloOrientacniPismeno { get; set; }
            }

            [ShowNiceDisplayName()]
            public enum TypSubjektu
            {
                [NiceDisplayName("DS běžného OVM vzniklá ze zákona – vzniká a edituje se pouze z externího podnětu z evidence Seznam OVM - §6 odst. 1 resp. Z Rejstříku OVM.")]
                OVM,
                [NiceDisplayName("DS právnické osoby zřízené zákonem ale ne v OR (hlášená z ROS) - §5 odst. 1.")]
                PO,
                [NiceDisplayName("DS podnikající fyzické osoby vzniklá na žádost - §4 odst. 1.")]
                PFO,
                [NiceDisplayName("DS fyzické osoby, vzniklá na žádost - §3 odst. 1")]
                FO,
                [NiceDisplayName("Vzniká změnou typu poté, co byl subjekt odpovídající existující schránce typu PO nebo PO_REQ zapsán do Rejstříku OVM.PO případném výmazu z Rejstříku OVM se opět typ změní na původní PO nebo PO_REQ (odpovídá tzv. „povyšování schránek“ před novelou zákona o ISDS).")]
                OVM_PO,
                [NiceDisplayName("Vzniká poté, co subjekt PFO byl zapsán do Rejstříku OVM")]
                OVM_PFO,
                [NiceDisplayName("Vzniká poté, co je fyzická osoba(tj.bez IČO) zapsána do Rejstříku OVM.")]
                OVM_FO

            }

            public Osoba()
            { }

            public string[] DS()
            {
                return Adresy.Select(m => m.DS).Where(m => !string.IsNullOrEmpty(m)).ToArray();
            }
            public string ICO { get; set; }
            public string Nazev { get; set; }
            public TypSubjektu Typ { get; set; }
            public bool PovolenyPrijemDS { get; set; }

            public DSAdresa[] Adresy { get; set; } = new DSAdresa[] { };


            public static Osoba GetOsoba(IEnumerable<GetInfoResponseOsoba> xmlOsoba)
            {
                if (xmlOsoba == null)
                    return null;
                if (xmlOsoba.Count() == 0)
                    return null;

                var o = new Osoba();
                var a = new List<Osoba.DSAdresa>();
                var of = xmlOsoba.First();
                o.ICO = of.Ico;
                o.Nazev = of.NazevOsoby;
                o.PovolenyPrijemDS = of.PDZ;
                Osoba.TypSubjektu oTyp;
                if (Enum.TryParse<Osoba.TypSubjektu>(of.TypSubjektu, out oTyp))
                    o.Typ = oTyp;

                foreach (var xmlo in xmlOsoba)
                {
                    if (xmlo.AdresaSidla != null)
                    {
                        var ad = new Osoba.DSAdresa();
                        ad.AdresaFull = xmlo.AdresaSidla.AdresaTextem;
                        ad.CastObce = xmlo.AdresaSidla.CastObceNazev;
                        ad.CisloDomovni = xmlo.AdresaSidla.CisloDomovni;
                        ad.CisloOrientacni = xmlo.AdresaSidla.CisloOrientacni;
                        ad.CisloOrientacniPismeno = xmlo.AdresaSidla.CisloOrientacniPismeno;
                        ad.DS = xmlo.ISDS;
                        ad.Obec = xmlo.AdresaSidla.ObecNazev;
                        ad.Okres = xmlo.AdresaSidla.OkresNazev;
                        ad.PSC = xmlo.AdresaSidla.PostaKod;
                        ad.TypCisla = xmlo.AdresaSidla.TypCislaDomovnihoKod;
                        ad.Ulice = xmlo.AdresaSidla.UliceNazev;
                        a.Add(ad);
                    }
                }
                o.Adresy = a.ToArray();

                return o;

            }


        }


        public static string[] GetDatoveSchrankyForIco(string ico)
        {
            return GetSubjektyForIco(ico)?.DS() ?? new string[] { };
        }

        public static Osoba GetSubjektyForIco(string ico)
        {

            string req = @"<GetInfoRequest xmlns=""http://seznam.gov.cz/ovm/ws/v1""><Ico>{0}</Ico></GetInfoRequest>";
            var resp = GetResponse(string.Format(req, ico));
            return Osoba.GetOsoba(resp?.Osoba);
        }

        public static string[] GetDatoveSchrankyForDS(string ds)
        {
            return GetSubjektyForDS(ds)?.DS() ?? new string[] { };
        }

        public static Osoba GetSubjektyForDS(string ds)
        {

            string req = @"<GetInfoRequest xmlns=""http://seznam.gov.cz/ovm/ws/v1""><DataboxId>{0}</DataboxId></GetInfoRequest>";
            var resp = GetResponse(string.Format(req, ds));
            return Osoba.GetOsoba(resp?.Osoba);
        }

        private static GetInfoResponse GetResponse(string request)
        {
            try
            {
                using (Devmasters.Net.HttpClient.URLContent net = new Devmasters.Net.HttpClient.URLContent("https://www.mojedatovaschranka.cz/sds/ws/call"))
                {
                    net.Method = Devmasters.Net.HttpClient.MethodEnum.POST;
                    net.RequestParams.RawContent = request;
                    var resp = net.GetContent().Text;

                    XmlSerializer serializer = new XmlSerializer(typeof(GetInfoResponse));
                    GetInfoResponse obj = serializer.Deserialize(new System.IO.StringReader(resp)) as GetInfoResponse;
                    return obj;
                }

            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("GetDatoveSchranky request error", e);
                return null;
            }

        }



    }
}
