using Dapper;
using Npgsql;
using HlidacStatu.Lib.Data.Dotace;
using Newtonsoft.Json;
using System.Configuration;

namespace DotaceLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.AppSettings["postgres"];

            string sqlQuery = @"Select * from export.dotacejson;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var dotaceExport = connection.Query<ExportDotace>(sqlQuery);

                foreach (var record in dotaceExport)
                {
                    string constructedId = $"{record.NazevZdroje}-{record.IdDotace}";
                    constructedId = Devmasters.Core.TextUtil.NormalizeToURL(constructedId);

                    Dotace dotace = JsonConvert.DeserializeObject<Dotace>(record.Data);
                    dotace.IdDotace = constructedId;
                    dotace.Prijemce.Ico = NormalizeIco(dotace.Prijemce.Ico);
                    dotace.Hash = record.Hash;

                    dotace.Save();
                }

                


            }
        }

        public static string NormalizeIco(string ico)
        {
            if (ico == null)
                return string.Empty;
            else if (ico.Contains("cz-"))
                return MerkIcoToICO(ico);
            else
                return ico.PadLeft(8, '0');
        }

        public static string MerkIcoToICO(string merkIco)
        {
            if (merkIco.ToLower().Contains("cz-"))
                merkIco = merkIco.Replace("cz-", "");

            return merkIco.PadLeft(8, '0');
        }
    }
}
