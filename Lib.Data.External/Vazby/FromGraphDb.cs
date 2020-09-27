using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.External.Vazby
{
    public class FromGraphDb
    {

        public static IEnumerable<string> ForIco(string ico, DateTime? from = null)
        {
            ico = HlidacStatu.Util.ParseTools.IcoToMerkIco(ico);
            from = from ?? new DateTime(1900, 1, 1);
            string sFrom = from.Value.ToString("yyyyMMdd");

            using (var db = GraphDatabase.Driver(Devmasters.Config.GetWebConfigValue("Neo4jUrl"), 
                                    AuthTokens.Basic(Devmasters.Config.GetWebConfigValue("Neo4jUser"), 
                                                    Devmasters.Config.GetWebConfigValue("Neo4jPassword"))
                                    )
                    )
            {
                using (var session = db.Session())
                {
                    var query = @"MATCH (n:company {regno: """ + ico + @"""})-[rels1*0..50]->(prev:company)
WHERE ALL(r in rels1 WHERE (NOT EXISTS(r.to_date)) OR r.to_date >= " + sFrom + @")
WITH prev
MATCH(prev) -[rel2]->(this:company)
WHERE(NOT EXISTS(rel2.to_date) OR rel2.to_date >= " + sFrom + @")
RETURN DISTINCT this.regno";
                    var result = session.Run(query);
                    return result.Select(m => HlidacStatu.Util.ParseTools.MerkIcoToICO(m.Values["this.regno"].ToString()));
                }
            }
        }
    }
}
