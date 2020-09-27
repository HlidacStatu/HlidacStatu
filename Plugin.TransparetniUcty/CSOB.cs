using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using HlidacStatu.Util;

namespace HlidacStatu.Plugin.TransparetniUcty
{
	public class CSOB : BaseTransparentniUcetParser
	{
		private readonly string[] pdf2txt = new[] { "pdftotext", "-table -clip -nopgbrk -enc UTF-8 {0} {1}" };
		private readonly StringComparison SC = StringComparison.Ordinal;

		public override string Name => "ČSOB";

		public CSOB(IBankovniUcet ucet) : base(ucet)
		{ }

		protected override IEnumerable<IBankovniPolozka> DoParse(DateTime? fromDate = default(DateTime?), DateTime? toDate = default(DateTime?))
		{
			var statementItems = new List<IBankovniPolozka>();

			var tmpPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(tmpPath);

			try
			{
				var pdfUrls = GetBankStatementLinks();

				foreach (var statementUrl in pdfUrls.Keys)
				{
					var monthOfPdf = pdfUrls[statementUrl];
					var localRoot = Path.Combine(tmpPath, monthOfPdf.ToString("MM-yyyy"));
					Directory.CreateDirectory(localRoot);

					var pdfFile = DownloadStatement(statementUrl, localRoot);
					var textFile = ConvertStatementToText(pdfFile, localRoot);
					statementItems.AddRange(ParseStatement(textFile, monthOfPdf, statementUrl));
				}

			}
			catch (Exception e)
			{
				TULogger.Error("CSOB Parse", e);
			}
			finally
			{
				Directory.Delete(tmpPath, true);
			}
			return statementItems;
		}

		private static readonly Regex StatementItemPattern = new Regex("^\\d{2}\\.\\d{2}\\. ", RegexOptions.Compiled);
		private static readonly Regex AccountNumberPattern = new Regex(@"\d*\-?\d+/\d{4}", RegexOptions.Compiled);
		private static readonly Regex ItemsPattern = new Regex(@" *(\d+) *", RegexOptions.Compiled);
		private const string OpeningBalanceText = "Počáteční zůstatek:";
		private const string FinalBalanceText = "Konečný zůstatek:";
		private readonly PositionIndex CreditItemsPosition = new PositionIndex(25, 40);
		private readonly PositionIndex DebitItemsPosition = new PositionIndex(25, 40);
		private List<IBankovniPolozka> ParseStatement(string textFile, DateTime date, string sourceUrl)
		{
			var statementItems = new List<IBankovniPolozka>();
			var data = File.ReadAllLines(textFile);
			var item = (StatementItemRecord)null;
			var overview = new StatementOverview();
			var positions = new StatementItemsPositions();

			foreach (var line in data)
			{
				if (string.IsNullOrEmpty(line)) continue;
				if (line.Contains("Změna úrokové sazby")) continue;

				if (line.StartsWith("Počet kreditních položek"))
				{
					overview.CreditItems = int.Parse(ItemsPattern.Match(GetValue(line, CreditItemsPosition)).Groups[1].Value);
					if (!line.Contains(OpeningBalanceText)) continue;
				}
				if (line.StartsWith("Počet debetních položek"))
				{
					overview.DebitItems = int.Parse(ItemsPattern.Match(GetValue(line, DebitItemsPosition)).Groups[1].Value);
					continue;
				}
				if (line.Contains(OpeningBalanceText))
				{
					overview.OpeningBalance = ParseTools.FromTextToDecimal(GetValue(line, new PositionIndex(line.IndexOf(OpeningBalanceText, SC) + OpeningBalanceText.Length + 1, default(int?)))) ?? 0;
					continue;
				}
				if (line.Trim().StartsWith(FinalBalanceText))
				{
					overview.FinalBalance = ParseTools.FromTextToDecimal(GetValue(line, new PositionIndex(line.IndexOf(FinalBalanceText, SC) + FinalBalanceText.Length + 1, default(int?)))) ?? 0;
					continue;
				}

				if (line.StartsWith("Datum"))
				{
					DefineItemsPositionsForFirstLine(positions, line);
					continue;
				}
				if (line.StartsWith("Valuta"))
				{
					DefineItemsPositionsForSecondLine(positions, line);
					continue;
				}

				if (item != null && (line[0] != ' ' || line.Trim().StartsWith("Převádí se") || line.Trim().StartsWith("Konec výpisu")))
				{
					statementItems.Add(item.ToBankPolozka(date, Ucet, sourceUrl));
					item = null;
				}

				if (StatementItemPattern.IsMatch(line))
				{
					item = CreateStatementItem(line, positions);
				}
				else if (item != null && string.IsNullOrEmpty(item.ZpravaProPrijemce) && AccountNumberPattern.IsMatch(line))
				{
					UpdateStatementItem(item, line, positions);
				}
				else if (item != null)
				{
					item.ZpravaProPrijemce += (string.IsNullOrEmpty(item.ZpravaProPrijemce)
												  ? string.Empty
												  : Environment.NewLine) + line.Trim();
				}
			}

			ValidateParsedItems(sourceUrl, overview, statementItems);

			return statementItems;
		}

		private static string DownloadStatement(string statementUrl, string root)
		{
			var pdfTmpFile = Path.Combine(root, "statement.pdf");
			using (var net = new Devmasters.Net.HttpClient.URLContent(statementUrl))
			{
				File.WriteAllBytes(pdfTmpFile, net.GetBinary().Binary);
			}
			return pdfTmpFile;
		}

		private string ConvertStatementToText(string pdfTmpFile, string root)
		{
			var txtTmpFile = Path.Combine(root, "statement.txt");
			var psi = new System.Diagnostics.ProcessStartInfo(pdf2txt[0],
				string.Format(pdf2txt[1], pdfTmpFile, txtTmpFile));
			var pe = new Devmasters.ProcessExecutor(psi, 60);
			pe.Start();
			return txtTmpFile;
		}

		private Dictionary<string, DateTime> GetBankStatementLinks()
		{
			using (var url = new Devmasters.Net.HttpClient.URLContent(Ucet.Url))
			{
				var doc = new XPath(url.GetContent().Text);
				return doc.GetNodes(
							   "//div[@class='npw-transaction-group']/ul[@class='npw-documents']//a[text()[contains(.,'Transakce')]]")
						   ?.Select(n => new
						   {
							   url = "https://www.csob.cz" + n.Attributes["href"].Value,
							   month = "01-" + n.InnerText.Replace("Transakce ", "").Replace("/", "-").Trim()
						   }
						   )
						   ?.ToDictionary(k => k.url,
							   v => DateTime.ParseExact(v.month, "dd-MM-yyyy", Consts.czCulture))
					   ?? new Dictionary<string, DateTime>();
				;
			}
		}

		private void DefineItemsPositionsForFirstLine(StatementItemsPositions positions, string line)
		{
			if (line.IndexOf("Identifikace", SC) > 0) positions.AddId = new PositionIndex(line.IndexOf("Identifikace", SC), 14);
			else if (line.IndexOf("Reference banky", SC) > 0) positions.AddId = new PositionIndex(line.IndexOf("Reference banky", SC), 20);
			else if (line.IndexOf("Sekv.", SC) > 0) positions.AddId = new PositionIndex(line.IndexOf("Sekv.", SC), 5);
			var protiucetPosition = line.IndexOf("Název protiúčtu", SC);
			positions.NazevProtiuctu = new PositionIndex(protiucetPosition, 25);
			var popisPosition = Math.Max(line.IndexOf("Označení platby", SC), line.IndexOf("Označení operace", SC));
			positions.PopisTransakce = new PositionIndex(popisPosition, protiucetPosition - popisPosition - 1);
			positions.Castka = new PositionIndex(line.IndexOf("Částka", SC), 15);
			positions.KS = new PositionIndex(line.IndexOf("KS", SC), 4);
		}

		private void DefineItemsPositionsForSecondLine(StatementItemsPositions positions, string line)
		{
			positions.CisloProtiuctu = new PositionIndex(line.IndexOf("Protiúčet nebo poznámka", SC), 50);
			positions.SS = new PositionIndex(line.IndexOf("SS", SC), default(int?));
			if (line.IndexOf("KS", SC) > 0)
			{
				positions.KS = new PositionIndex(line.IndexOf("KS", SC), 4);
			}
			var vsPosition = line.IndexOf("VS", SC);
			positions.VS = new PositionIndex(vsPosition, positions.KS.Start - vsPosition - 1);
		}
		private StatementItemRecord CreateStatementItem(string line, StatementItemsPositions positions)
		{
			return new StatementItemRecord
			{
				AddId = GetValue(line, positions.AddId).Trim(),
				Datum = GetValue(line, positions.Datum).Trim(),
				PopisTransakce = GetValue(line, positions.PopisTransakce).Trim(),
				NazevProtiuctu = GetValue(line, positions.NazevProtiuctu).Trim(),
				Castka = ParseAmount(line, positions)
			};
		}

		private decimal ParseAmount(string line, StatementItemsPositions positions)
		{
			var amount = GetValue(line, positions.Castka).Trim();
			return ParseTools.FromTextToDecimal(amount.Trim())
				   ?? throw new ApplicationException(
					   $"Amount on line '{line}' on position {positions.Castka} could not be read ({amount})");
		}

		private void UpdateStatementItem(StatementItemRecord item, string line, StatementItemsPositions positions)
		{
			item.CisloProtiuctu = GetValue(line, positions.CisloProtiuctu).Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).First();
			item.VS = GetValue(line, positions.VS).Trim();
			item.KS = GetValue(line, positions.KS).Trim();
			ParseSS(item, line, positions);
			item.ZpravaProPrijemce = string.Empty;
		}

		private void ParseSS(StatementItemRecord item, string line, StatementItemsPositions positions)
		{
			var ss = GetValue(line, positions.SS);
			if (!string.IsNullOrEmpty(ss) && !ss.StartsWith(" "))
			{
				item.SS = ss.IndexOf(" ", SC) > 0
					? ss.Substring(0, ss.IndexOf(" ", SC))
					: ss;
			}
		}

		private static void ValidateParsedItems(string sourceUrl, StatementOverview overview, List<IBankovniPolozka> statementItems)
		{
			var currentCreditItems = statementItems.Count(i => i.Castka > 0);
			if (overview.CreditItems != currentCreditItems)
			{
				throw new ApplicationException(
					$"Invalid count of credit items (expected {overview.CreditItems}, found {currentCreditItems}) - {sourceUrl}");
			}
			var currentDebitItems = statementItems.Count(i => i.Castka < 0);
			if (overview.DebitItems != currentDebitItems)
			{
				throw new ApplicationException(
					$"Invalid count of debit items (expected {overview.DebitItems}, found {currentDebitItems}) - {sourceUrl}");
			}
			var currentFinalBalance = overview.OpeningBalance + statementItems.Sum(i => i.Castka);
			if (overview.FinalBalance != currentFinalBalance)
			{
				throw new ApplicationException(
					$"Invalid final balance (expected {overview.FinalBalance}, found {currentFinalBalance}) - {sourceUrl}");
			}
		}

		private const int DefaultLength = 100;
		private static string GetValue(string line, PositionIndex pos)
		{
			if (line.Length <= pos.Start || pos.Start < 0 || pos.Length <= 0)
			{
				return string.Empty;
			}
			else if (line.Length <= pos.Start + (pos.Length ?? DefaultLength))
			{
				return line.Substring(pos.Start);
			}
			return line.Substring(pos.Start, pos.Length ?? DefaultLength);
		}

		private class StatementItemRecord
		{
			public string Datum { get; set; }
			public string PopisTransakce { get; set; }
			public string NazevProtiuctu { get; set; }
			public string CisloProtiuctu { get; set; }
			public string ZpravaProPrijemce { get; set; }
			public string VS { get; set; }
			public string KS { get; set; }
			public string SS { get; set; }
			public decimal Castka { get; set; }
			public string AddId { get; set; }
			public IBankovniPolozka ToBankPolozka(DateTime month, IBankovniUcet ucet, string zdroj)
			{
				var datum = Datum.Split('.'); // 25.04.
				return new SimpleBankovniPolozka
				{
					AddId = Devmasters.TextUtil.NormalizeToBlockText(AddId),
					Castka = Castka,
					CisloProtiuctu = Devmasters.TextUtil.NormalizeToBlockText(CisloProtiuctu),
					CisloUctu = Devmasters.TextUtil.NormalizeToBlockText(ucet.CisloUctu),
					KS = Devmasters.TextUtil.NormalizeToBlockText(KS),
					NazevProtiuctu = Devmasters.TextUtil.NormalizeToBlockText(NazevProtiuctu),
					PopisTransakce = Devmasters.TextUtil.NormalizeToBlockText(PopisTransakce),
					SS = Devmasters.TextUtil.NormalizeToBlockText(SS),
					VS = Devmasters.TextUtil.NormalizeToBlockText(VS),
					ZpravaProPrijemce = Devmasters.TextUtil.NormalizeToBlockText(ZpravaProPrijemce),
					ZdrojUrl = zdroj,
					Datum = new DateTime(month.Year, Convert.ToInt32(datum[1]), Convert.ToInt32(datum[0]))
				};
			}
		}

		private class PositionIndex
		{
			public PositionIndex(int start, int? length)
			{
				Start = start;
				Length = length;
			}

			public int Start { get; private set; }
			public int? Length { get; private set; }

			public override string ToString()
			{
				return Length.HasValue
					? $"{Start} ({Length.Value})"
					: Start.ToString();
			}
		}

		private class StatementItemsPositions
		{
			public StatementItemsPositions()
			{
				Datum = new PositionIndex(0, 6);
			}

			public PositionIndex AddId { get; set; }
			public PositionIndex Datum { get; set; }
			public PositionIndex PopisTransakce { get; set; }
			public PositionIndex NazevProtiuctu { get; set; }
			public PositionIndex Castka { get; set; }
			public PositionIndex CisloProtiuctu { get; set; }
			public PositionIndex VS { get; set; }
			public PositionIndex KS { get; set; }
			public PositionIndex SS { get; set; }
		}

		private class StatementOverview
		{
			public int CreditItems { get; set; }
			public int DebitItems { get; set; }
			public decimal OpeningBalance { get; set; }
			public decimal FinalBalance { get; set; }

		}
	}
}
