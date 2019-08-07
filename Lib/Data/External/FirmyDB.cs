using Devmasters.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.External
{
    public partial class FirmyDB
    {

        static string cnnStr = Devmasters.Core.Util.Config.GetConfigValue("CnnString");

        public static Firma FromIco(string ico)
        {
            Firma f = new Data.Firma();
            using (PersistLib p = new PersistLib())
            {
                string sql = @"select * from Firma where ico = @ico";

                var res = p.ExecuteDataset(cnnStr, System.Data.CommandType.Text, sql, new IDataParameter[] {
                        new System.Data.SqlClient.SqlParameter("ico", ico)
                        });

                if (res.Tables[0].Rows.Count > 0)
                {
                    return FromDataRow(res.Tables[0].Rows[0]);
                }
                else
                {
                    return Firma.NotFound;
                }
            }
        }
        public static Firma FromName(string jmeno)
        {
            var res = AllFromName(jmeno);
            if (res.Count() == 0)
                return Firma.NotFound;
            else
                return res.First();
        }
        public static IEnumerable<Firma> AllFromName(string jmeno)
        {
            using (PersistLib p = new PersistLib())
            {
                string sql = @"select * from Firma where jmeno = @jmeno";

                var res = p.ExecuteDataset(cnnStr, System.Data.CommandType.Text, sql, new IDataParameter[] {
                        new System.Data.SqlClient.SqlParameter("jmeno", jmeno)
                        });

                if (res.Tables.Count > 0 && res.Tables[0].Rows.Count > 0)
                {
                    return res.Tables[0]
                        .AsEnumerable()
                        .Where(r => Devmasters.Core.TextUtil.IsNumeric((string)r["ICO"]))
                        .Select(m => FromDataRow(m));
                }
                else
                    return new Firma[] { };
            }
        }


        public static IEnumerable<Firma> AllFromNameWildcards(string jmeno)
        {
            using (PersistLib p = new PersistLib())
            {
                var sql = @"select * from Firma where jmeno like @jmeno";

                var res = p.ExecuteDataset(cnnStr, System.Data.CommandType.Text, sql, new IDataParameter[] {
                        new System.Data.SqlClient.SqlParameter("jmeno", Firma.JmenoBezKoncovky(jmeno)+ "%")
                        });
                var found = new List<Firma>();
                if (res.Tables.Count > 0 && res.Tables[0].Rows.Count > 0)
                {
                    found.AddRange(res.Tables[0]
                        .AsEnumerable()
                        .Select(m => FromDataRow(m))
                        );
                    return found;
                }
                else
                    return new Firma[] { };
            }
        }

        public static Firma FromDS(string ds)
        {
            Firma f = new Data.Firma();
            using (PersistLib p = new PersistLib())
            {
                string sql = @"select firma.* from Firma_ds fds inner join firma on firma.ico = fds.ico where DatovaSchranka = @ds";

                var res = p.ExecuteDataset(cnnStr, System.Data.CommandType.Text, sql, new IDataParameter[] {
                        new System.Data.SqlClient.SqlParameter("ds", ds)
                        });

                if (res.Tables[0].Rows.Count > 0)
                {
                    return FromDataRow(res.Tables[0].Rows[0]);
                }
                else
                {
                    return Firma.NotFound;
                }
            }

        }
        public static IEnumerable<string> AllIcoInRS(bool includedIcosInHoldings = false)
        {
            using (PersistLib p = new PersistLib())
            {
                string sql = @"select ico from Firma where IsInRS = 1";

                var res = p.ExecuteDataset(cnnStr, System.Data.CommandType.Text, sql, null);

                if (res.Tables.Count > 0 && res.Tables[0].Rows.Count > 0)
                {
                    var allIcos = res.Tables[0]
                        .AsEnumerable()
                        .Where(r => Devmasters.Core.TextUtil.IsNumeric((string)r["ICO"]))
                        .Select(r => (string)r["ICO"])
                        .ToArray();
                    if (includedIcosInHoldings == false)
                        return allIcos;
                    else
                    {
                        HashSet<string> holdingIcos = new HashSet<string>();
                        foreach (var i in allIcos)
                        {
                            if (!holdingIcos.Contains(i))
                                holdingIcos.Add(i);
                            Firma f = Firmy.Get(i);
                            foreach (var hi in f.IcosInHolding( Relation.AktualnostType.Nedavny))
                            {
                                if (!holdingIcos.Contains(hi))
                                    holdingIcos.Add(hi);
                            }
                        }
                        return holdingIcos;

                    }
                    
                }
                else
                    return new string[] { };
            }
        }
        public static IEnumerable<Firma> AllFirmyInRS(bool skipDS_Nace = false)
        {
            using (PersistLib p = new PersistLib())
            {
                string sql = @"select * from Firma where IsInRS = 1";

                var res = p.ExecuteDataset(cnnStr, System.Data.CommandType.Text, sql, null);

                if (res.Tables.Count > 0 && res.Tables[0].Rows.Count > 0)
                {
                    return res.Tables[0]
                        .AsEnumerable()
                        .Where(r => Devmasters.Core.TextUtil.IsNumeric((string)r["ICO"]))
                        .Select(r => FromDataRow(r, skipDS_Nace));
                }
                else
                    return new Firma[] { };
            }
        }

        private static Firma FromDataRow(DataRow dr, bool skipDS_Nace=false)
        {
            Firma f = new Data.Firma();
            f.ICO = (string)dr["ico"];
            f.DIC = (string)PersistLib.IsNull(dr["dic"], string.Empty);
            f.Datum_Zapisu_OR = (DateTime?)PersistLib.IsNull(dr["datum_zapisu_or"], null);
            f.Stav_subjektu = Convert.ToInt32(PersistLib.IsNull(dr["Stav_subjektu"], 1));
            f.Jmeno = (string)PersistLib.IsNull(dr["jmeno"], string.Empty);
            f.JmenoAscii = (string)PersistLib.IsNull(dr["jmenoascii"], string.Empty);
            f.Kod_PF = (int?)PersistLib.IsNull(dr["Kod_PF"], null);
            f.VersionUpdate = (int)dr["VersionUpdate"];
            //f.VazbyRaw = (string)PersistLib.IsNull(dr["vazbyRaw"], (string)"[]");
            f.IsInRS = (short?)PersistLib.IsNull(dr["IsInRS"], null);

            if (skipDS_Nace == false)
            {
                using (PersistLib p = new PersistLib())
                {
                    f.DatovaSchranka = p.ExecuteDataset(cnnStr, System.Data.CommandType.Text, "select DatovaSchranka from firma_DS where ico=@ico", new IDataParameter[] {
                        new System.Data.SqlClient.SqlParameter("ico", f.ICO)
                        }).Tables[0]
                        .AsEnumerable()
                        .Select(m => m[0].ToString())
                        .ToArray();

                    f.NACE = p.ExecuteDataset(cnnStr, System.Data.CommandType.Text, "select NACE from firma_Nace where ico=@ico", new IDataParameter[] {
                        new System.Data.SqlClient.SqlParameter("ico", f.ICO)
                        }).Tables[0]
                        .AsEnumerable()
                        .Select(m => m[0].ToString())
                        .ToArray();
                }
            }
            return f;

        }



        public static string NameFromIco(string ico, bool IcoIfNotFound = false)
        {
            string cnnStr = Devmasters.Core.Util.Config.GetConfigValue("CnnString");
            using (PersistLib p = new PersistLib())
            {
                string sql = @"select jmeno from Firma where ico = @ico";

                var res = p.ExecuteScalar(cnnStr, System.Data.CommandType.Text, sql, new IDataParameter[] {
                        new System.Data.SqlClient.SqlParameter("ico", ico)
                        });

                if (PersistLib.IsNull(res) || string.IsNullOrEmpty(res as string))
                {
                    if (IcoIfNotFound)
                        return "IČO:" + ico;
                    else
                        return string.Empty;
                }
                else
                    return (string)res;

            }
        }





    }
}
