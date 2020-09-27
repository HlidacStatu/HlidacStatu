using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Devmasters;
using Devmasters.Net.HttpClient;
using HlidacStatu.Util;
using HtmlAgilityPack;

namespace HlidacStatu.Plugin.TransparetniUcty
{
	public class Moneta : BaseTransparentniUcetParser
	{
		public override string Name { get { return "Moneta"; } }

		public Moneta(IBankovniUcet ucet) : base(ucet)
		{
		}

		protected override IEnumerable<IBankovniPolozka> DoParse(DateTime? fromDate = null, DateTime? toDate = null)
		{
			var polozky = new List<IBankovniPolozka>();
			var onPage = new List<IBankovniPolozka>();
			var currentUrl = Ucet.Url;

			do
			{
				onPage = new List<IBankovniPolozka>();

				using (var url = new URLContent(currentUrl))
				{
					url.Referer = Ucet.Url;
					url.IgnoreHttpErrors = true;
					var s = url.GetContent(Encoding.UTF8).Text;
					var doc = new XPath(s);
					var rows = doc.GetNodes("//table[@id='transparentAccountTable']/tbody/tr") 
						?? doc.GetNodes("//tr")
						?? new List<HtmlNode>();

					foreach (var row in rows)
					{
						var cols = row.ChildNodes.Where(n => n.Name == "td").Select(n => n.InnerHtml).ToArray();
						var p = new SimpleBankovniPolozka();
						p.CisloUctu = Ucet.CisloUctu;
						var date = Devmasters.DT.Util.ToDateTime(cols[0], "d.M.yyyy");
						if (!date.HasValue || (fromDate.HasValue && date.Value < fromDate.Value) || (toDate.HasValue && date.Value > toDate.Value))
						{
							continue; //skip this, it's not row with data
						}
						p.Datum = date.Value;

						var parts = cols[1].Split(new string[] { "</br>", "<br>" }, StringSplitOptions.None)?.Select(v => WebUtility.HtmlDecode(v)).ToArray() ?? new string[] { "" };
						p.NazevProtiuctu = TextUtil.NormalizeToBlockText(WebUtility.HtmlDecode(parts[0]));

						if (parts.Length == 3)
						{
							p.ZpravaProPrijemce = TextUtil.NormalizeToBlockText(parts[2]);
						}

						p.VS = TextUtil.NormalizeToBlockText(cols[3]);
						if (p.VS.Contains("---------")) p.VS = "";


						p.Castka = ParseTools.ToDecimal(
							WebUtility.HtmlDecode(cols[4])
								.Replace(" CZK", "")
								.Replace(" ", "")
							).Value;
						p.ZdrojUrl = Ucet.Url;

						onPage.Add(p);
					}
					polozky.AddRange(onPage);

					var lineWithLastTransactionDate = s.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(l => l.Contains("lastTransactionDate"));
					if (lineWithLastTransactionDate != null)
					{
						var parts = lineWithLastTransactionDate.Trim().Split(':');
						if (parts.Length >= 2)
						{
							var lastTransactionDate = parts[1].Replace("'", "").Trim();
							currentUrl = $"https://transparentniucty.moneta.cz/homepage?p_p_id=TransparentAccountPortlet_WAR_monetaportletsportlet&p_p_lifecycle=2&p_p_state=normal&p_p_mode=view&p_p_resource_id=serveTableData&p_p_cacheability=cacheLevelPage&p_p_col_id=column-8&p_p_col_count=1&_TransparentAccountPortlet_WAR_monetaportletsportlet_accountNumber={Ucet.CisloUctu.Replace("/0600", "")}&_TransparentAccountPortlet_WAR_monetaportletsportlet_reverse=NORMAL&_TransparentAccountPortlet_WAR_monetaportletsportlet_transactionNumber=1&_TransparentAccountPortlet_WAR_monetaportletsportlet_transactionDate={lastTransactionDate}&_={(long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds}";
						}
					}
				}
			} while (onPage.Count > 0);

			return polozky;
		}
	}
}
