﻿using Dapper;
using Npgsql;
using HlidacStatu.Lib.Data.Dotace;
using Newtonsoft.Json;
using System.Configuration;
using System.Collections.Generic;

namespace DotaceLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.AppSettings["postgres"];

            //string sqlQuery = @"Select * from export.dotacejson;";

            var dotaceService = new DotaceService();

            string sqlCursor = @"DECLARE export_cur CURSOR FOR Select * from dotace.dotace;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    connection.Execute(sqlCursor);

                    //var dotaceExport = connection.Query<ExportDotace>(sqlQuery);
                    while (true)
                    {
                        List<ExportDotace> dotaceExport = connection.Query<ExportDotace>("FETCH 1000 FROM export_cur;").AsList();
                        if (dotaceExport.Count == 0)
                        {
                            break;
                        }

                        var dotaceList = new List<Dotace>();

                        foreach (var record in dotaceExport)
                        {
                            string constructedId = record.IdDotace;
                            constructedId = Devmasters.TextUtil.NormalizeToURL(constructedId);

                            Dotace dotace = JsonConvert.DeserializeObject<Dotace>(record.Data);
                            dotace.IdDotace = constructedId;
                            //dotace.Prijemce.Ico = FullIcoFix(dotace);

                            //dotace.CalculateTotals(); moved to bulksave

                            dotaceList.Add(dotace);
                        }

                        bool anyErrorDuringImport = dotaceService.BulkSave(dotaceList);

                        if (anyErrorDuringImport)
                        {
                            System.Console.WriteLine($"Error during import.");
                        }

                    }

                    transaction.Commit();

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
