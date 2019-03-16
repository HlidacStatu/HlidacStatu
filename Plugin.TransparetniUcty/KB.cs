using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Devmasters.Core;
using HlidacStatu.Util;
using HtmlAgilityPack;

namespace HlidacStatu.Plugin.TransparetniUcty
{
    public class KB : BaseTransparentniUcetParser
    {
        public override string Name { get { return "KB"; } }

        public KB(IBankovniUcet ucet) : base(MapOldUrls(ucet))
        { }

        protected override IEnumerable<IBankovniPolozka> DoParse(DateTime? fromDate = null, DateTime? toDate = null)
        {
            TULogger.Info($"Zpracovavam ucet {Ucet.CisloUctu} s url {Ucet.Url}");
            var polozky = new List<IBankovniPolozka>();
            var page = 0;
            var duplications = 0;
            var httpClient = new HttpClient();

            do
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(MakeRequest(++page, httpClient));

                var rows = GetTransactionItems(doc);
                if (rows == null || rows.Length == 0)
                {
                    TULogger.Warning($"Nenalezeny zadne zaznamy pro ucet {Ucet.CisloUctu}");
                    return polozky;
                }

                foreach (var row in rows)
                {
                    var cells = row.Descendants("td").Select(c => c.InnerHtml).ToArray();
                    if (cells.Length == 0) continue; //skip this, it's not row with data

                    IBankovniPolozka p = new SimpleBankovniPolozka();
                    p.CisloUctu = Ucet.CisloUctu;
                    p.Datum = ParseDate(cells[0]);
                    p.Castka = ParsePrice(cells[1], p.Datum);

                    var symbols = cells[2].Split('/').Select(TextUtil.NormalizeToBlockText).ToArray();
                    p.VS = symbols.Length > 0 && symbols[0] != "—" ? symbols[0] : string.Empty;
                    p.KS = symbols.Length > 1 && symbols[1] != "—" ? symbols[1] : string.Empty;
                    p.SS = symbols.Length > 2 && symbols[2] != "—" ? symbols[2] : string.Empty;

                    var descriptions = cells[3].Split(new[] {"<br>"}, StringSplitOptions.None)
                        .Select(d => TextUtil.NormalizeToBlockText(WebUtility.HtmlDecode(d))).ToArray();

                    if (descriptions.Length > 0)
                    {
                        var account = descriptions[0].Split(new[] {"(", ")"}, StringSplitOptions.None)
                            .Select(TextUtil.NormalizeToBlockText)
                            .ToArray();
                        p.NazevProtiuctu = account.Length > 0 ? account[0] : string.Empty;
                        p.CisloProtiuctu = account.Length > 1 ? account[1] : string.Empty;
                    }
                    p.PopisTransakce = descriptions.Length > 1 ? descriptions[1] : string.Empty;
                    p.ZpravaProPrijemce = descriptions.Length > 2 ? string.Join("; ", descriptions.Skip(2)) : string.Empty;

                    p.ZdrojUrl = Ucet.Url;

                    if (fromDate.HasValue && p.Datum < fromDate) return polozky;
                    if (IsAlreadyExist(polozky, p))
                    {
                        duplications++;
                        if (duplications > 5)
                        {
                            return polozky;
                        }
                    }
                    else if (!(toDate.HasValue && p.Datum > toDate.Value))
                    {
                        duplications = 0;
                        polozky.Add(p);
                    }
                }
                TULogger.Debug($"[{page}] {Ucet.CisloUctu} - {polozky.Last().Datum} / celkem {polozky.Count}");
                Console.WriteLine($"[{page}] {Ucet.CisloUctu} - {polozky.Last().Datum} / celkem {polozky.Count}");

            } while (true);
        }

        private bool IsAlreadyExist(List<IBankovniPolozka> polozky, IBankovniPolozka p)
        {
            return polozky.Exists(i => i.Castka == p.Castka && i.Datum == p.Datum &&
                                       i.CisloProtiuctu == p.CisloProtiuctu
                                       && i.VS == p.VS && i.KS == p.KS && i.SS == p.SS &&
                                       i.ZpravaProPrijemce == p.ZpravaProPrijemce);
        }

        private decimal ParsePrice(string value, DateTime date)
        {
            var price = ParseTools.ToDecimal(WebUtility.HtmlDecode(value).Replace(" CZK", "").Replace(" ", ""));
            if (price.HasValue)
            {
                return price.Value;
            }

            TULogger.Error($"KB: chybejici castka pro ucet {Ucet.CisloUctu} dne {date}");
            throw new ApplicationException($"KB: chybejici castka pro ucet {Ucet.CisloUctu} dne {date}");
        }

        private DateTime ParseDate(string value)
        {
            var dat = ParseTools.ToDateTime(value, "d. M. yyyy");
            if (dat.HasValue)
            {
                return dat.Value;
            }

            TULogger.Error($"KB: chybejici datum pro ucet {Ucet.CisloUctu}");
            throw new ApplicationException($"KB: chybejici datum pro ucet {Ucet.CisloUctu}");
        }

        private static HtmlNode[] GetTransactionItems(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("table")
                .FirstOrDefault(t => t.InnerHtml.Contains("Datum zaúčtování"))?.Descendants("tr")?.ToArray();
        }

        private string MakeRequest(int page, HttpClient httpClient)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>(
                    "p_lt_WebPartZone6_zoneContent_contentPH_p_lt_WebPartZone3_zoneContentTop_TransactionsActum_plcUp_repItems_pager_cpage",
                    page.ToString())
            });
            content.Headers.Add("X-Requested-With", "XMLHttpRequest");
            content.Headers.Add("X-MicrosoftAjax", "Delta=true");
            var s = httpClient.PostAsync(Ucet.Url, content).Result.Content.ReadAsStringAsync().Result;
            return s;
        }

        private static IBankovniUcet MapOldUrls(IBankovniUcet ucet)
        {
            if (OldUrlMapping.ContainsKey(ucet.CisloUctu))
            {
                ucet.Url = OldUrlMapping[ucet.CisloUctu];
            }
            return ucet;
        }

        private static Dictionary<string, string> OldUrlMapping = new Dictionary<string, string>
        {
            { "115-5587300207/0100","https://www.kb.cz/cs/transparentni-ucty/hozk-hozk-czk" },
            { "115-5750690277/0100","https://www.kb.cz/cs/transparentni-ucty/karlovaraci-karlovaraci-czk" },
            { "115-5223490257/0100","https://www.kb.cz/cs/transparentni-ucty/sdruzeni-nezavislych-obcanu-prahy-sdruzeni-nezavi" },
            { "115-4449010207/0100","https://www.kb.cz/cs/transparentni-ucty/snk-evropsti-demokrate-snk-evropsti-demokrate-czk" },
            { "115-5547140237/0100","https://www.kb.cz/cs/transparentni-ucty/b-u-prav-os-s-volyne-v-dolyne-czk" },
            { "115-2573210207/0100","https://www.kb.cz/cs/transparentni-ucty/dobra-volba-2016-dobra-volba-2016-czk" },
            { "115-5471580227/0100","https://www.kb.cz/cs/transparentni-ucty/bilinsti-socialni-demokrate-bilinsti-socialni-dem" },
            { "115-5087400287/0100","https://www.kb.cz/cs/transparentni-ucty/radostne-cesko-transparentni-2-radostne-cesko-c" },
            { "115-4035960237/0100","https://www.kb.cz/cs/transparentni-ucty/plus-plus-czk" },
            { "35-7608650257/0100","https://www.kb.cz/cs/transparentni-ucty/nestranici-nestranici-czk-(1)" },
            { "115-5551500207/0100","https://www.kb.cz/cs/transparentni-ucty/moderni-spolecnost-moderni-spolecnost-czk" },
            { "115-2638450237/0100","https://www.kb.cz/cs/transparentni-ucty/prokraj-prokraj-czk" },
            { "107-8866160247/0100","https://www.kb.cz/cs/transparentni-ucty/profi-ucet-prozmenu-czk" },
            { "107-7430660297/0100","https://www.kb.cz/cs/transparentni-ucty/ods-transparentni-ucet-obcanska-demokraticka-stra" },
            { "115-3887350207/0100","https://www.kb.cz/cs/transparentni-ucty/severocesi-cz-severocesi-cz-czk" },
            { "115-5510480297/0100","https://www.kb.cz/cs/transparentni-ucty/ceska-pravice-michal-simkanic-czk" },
            { "115-6659080237/0100","https://www.kb.cz/cs/transparentni-ucty/ostravak-mudr-tomas-malek-czk" },
            { "115-5527330287/0100","https://www.kb.cz/cs/transparentni-ucty/kozy-ucet-pro-prispevky-adela-sochurkova-czk" },
            { "51-7126270227/0100","https://www.kb.cz/cs/transparentni-ucty/volba-pro-mladou-boleslav-volba-pro-mladou-bolesl" },
            { "115-3902720297/0100","https://www.kb.cz/cs/transparentni-ucty/strana-soukromniku-ceske-republiky-strana-soukrom" },
            { "4070217/0100","https://www.kb.cz/cs/transparentni-ucty/ano-2011-ano-2011-czk" },
            { "115-5010530227/0100","https://www.kb.cz/cs/transparentni-ucty/novy-impuls-novy-impuls-czk" },
            { "115-5000730217/0100","https://www.kb.cz/cs/transparentni-ucty/hnuti-praha-kunratice-hnuti-praha-kunratice" },
            { "115-5087380247/0100","https://www.kb.cz/cs/transparentni-ucty/radostne-cesko-transparentni-1-radostne-cesko-c" },
            { "115-4933560227/0100","https://www.kb.cz/cs/transparentni-ucty/jirkov-21-stoleti-jirkov-21-stoleti-czk" },
            { "107-7678680267/0100","https://www.kb.cz/cs/transparentni-ucty/toryove-toryove-czk" },
            { "115-5053730237/0100","https://www.kb.cz/cs/transparentni-ucty/dobra-volba-2016-transp-volebni-ucet-dobra-vol" },
            { "115-5646610257/0100","https://www.kb.cz/cs/transparentni-ucty/obcane-spolu-nezavisli-obcane-spolu-nezavisli" },
            { "4080247/0100","https://www.kb.cz/cs/transparentni-ucty/ano-2011-volebni-ucet-ano-2011-czk" },
        };
    }
}
