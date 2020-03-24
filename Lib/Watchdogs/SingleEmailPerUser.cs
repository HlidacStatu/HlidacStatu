using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib.Data;
using static HlidacStatu.Lib.Data.WatchDog;

namespace HlidacStatu.Lib.Watchdogs
{
    public class SingleEmailPerUser
    {
        public static void SendWatchdogs(IEnumerable<WatchDog> watchdogs,
            bool force = false, string[] specificContacts = null,
            DateTime? fromSpecificDate = null, DateTime? toSpecificDate = null,
            string openingText = null,
            int? maxDegreeOfParallelism = 20,
            Action<string> logOutputFunc = null,
            Action<Devmasters.Core.Batch.ActionProgressData> progressOutputFunc = null
            )
        {
            bool saveWatchdogStatus =
                force == false
                && fromSpecificDate.HasValue == false
                && toSpecificDate.HasValue == false;

            Util.Consts.Logger.Info($"SingleEmailPerUser Start processing {watchdogs.Count()} watchdogs.");

            Dictionary<string, WatchDog[]> groupedByUserNoSpecContact = watchdogs
                .Where(w => w != null)
                .Where(m => string.IsNullOrEmpty(m.SpecificContact))
                .GroupBy(k => k.UnconfirmedUser().Id,
                        v => v,
                        (k, v) => new { key = k, val = v.ToArray() }
                        )
                .ToDictionary(k => k.key, v => v.val);

            Util.Consts.Logger.Info($"SingleEmailPerUser {groupedByUserNoSpecContact.Count()} emails.");


            Devmasters.Core.Batch.Manager.DoActionForAll<KeyValuePair<string, WatchDog[]>>(groupedByUserNoSpecContact,
                (kv) =>
                {
                    WatchDog[] userWatchdogs = kv.Value;

                    AspNetUser user = null;
                    using (Lib.Data.DbEntities db = new DbEntities())
                    {
                        user = db.AspNetUsers
                        .Where(m => m.Id == kv.Key)
                        .FirstOrDefault();
                    }

                    var res = Mail.SendWatchdogsInOneEmail(userWatchdogs, user,
                        force, specificContacts, fromSpecificDate, toSpecificDate, openingText);

                    Util.Consts.Logger.Info($"SingleEmailPerUser {kv.Key} sent result {res.ToString()}.");

                    return new Devmasters.Core.Batch.ActionOutputData();
                },
                logOutputFunc, progressOutputFunc,
                true, maxDegreeOfParallelism: maxDegreeOfParallelism
                );

            Util.Consts.Logger.Info($"SingleEmailPerUser Done processing {watchdogs.Count()} watchdogs.");

        }
    }
}
