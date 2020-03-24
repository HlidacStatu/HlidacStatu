using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib.Data;
using static HlidacStatu.Lib.Data.WatchDog;

namespace HlidacStatu.Lib.Watchdogs
{
    public class SingleEmailPerWatchdog
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

            Util.Consts.Logger.Info($"SingleEmailPerWatchdog Start processing {watchdogs.Count()} watchdogs.");

            Devmasters.Core.Batch.Manager.DoActionForAll<WatchDog>(watchdogs,
                (userWatchdog) =>
                {
                    AspNetUser user = userWatchdog.UnconfirmedUser();

                    var res = Mail.SendWatchdog(userWatchdog, user,
                        force, specificContacts, fromSpecificDate, toSpecificDate, openingText);

                    Util.Consts.Logger.Info($"SingleEmailPerWatchdog watchdog {userWatchdog.Id} sent result {res.ToString()}.");

                    return new Devmasters.Core.Batch.ActionOutputData();
                },
                logOutputFunc, progressOutputFunc,
                true, maxDegreeOfParallelism: maxDegreeOfParallelism
                );

            Util.Consts.Logger.Info($"SingleEmailPerWatchdog Done processing {watchdogs.Count()} watchdogs.");

        }
    }
}
