using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace HlidacStatu.Lib.Data
{

    public partial class WatchDog
    {
        public static class Send
        {
            public enum WatchdogTypeForSend
            {
                smlouvy,
                zakazky,
                dataset,
                insolvence,
                global
            }
            public static void SendWatchDogs(WatchdogTypeForSend? type, string[] ids = null, string userid = null, string date = null, string specificemail = null, bool forceSend = false, Expression<Func<WatchDog, bool>> predicate = null, bool debug = false)
            {
                SendWatchDogs(type?.ToString(), ids, userid, date, specificemail, forceSend, predicate,debug);
            }
            public static void SendWatchDogs(string type, string[] ids = null, string userid = null, string date = null, string specificemail = null, bool forceSend = false, Expression<Func<WatchDog, bool>> predicate = null, bool debug = false)
            {
                if (!string.IsNullOrEmpty(type))
                {
                    type = type.ToLower();
                    if (type == WatchdogTypeForSend.smlouvy.ToString())
                    {
                        _SendWatchDogs(typeof(Smlouva).Name, ids, userid, date, specificemail, forceSend, predicate,debug);
                        return;
                    }
                    else if (type == WatchdogTypeForSend.zakazky.ToString())
                    {
                        _SendWatchDogs(typeof(VZ.VerejnaZakazka).Name, ids, userid, date, specificemail, forceSend, predicate, debug);
                        return;
                    }
                    else if (type == WatchdogTypeForSend.insolvence.ToString())
                    {
                        _SendWatchDogs(typeof(Insolvence.Rizeni).Name, ids, userid, date, specificemail, forceSend, predicate, debug);
                        return;
                    }
                    else if (type == WatchdogTypeForSend.dataset.ToString())
                    {
                        _SendWatchDogs(typeof(DataSet).Name, ids, userid, date, specificemail, forceSend, predicate, debug);
                        return;
                    }
                    else if (type == WatchdogTypeForSend.global.ToString())
                    {
                        _SendWatchDogs(WatchDog.AllDbDataType, ids, userid, date, specificemail, forceSend, predicate, debug);
                        return;
                    }
                    else
                    {
                        throw new System.ArgumentOutOfRangeException("type", "ERROR: invalid type. Nothing sent");
                        return;
                    }
                }

                _SendWatchDogs("", ids, userid, date, specificemail, forceSend, predicate, debug);
                //SendVerejneZakazkyWatchDogs(ids, userid, date, specificemail, forceSend);
                //SendDatasetWatchDogs(ids, userid, date, specificemail, forceSend);
            }


            private static void _SendWatchDogs(string datatype, string[] ids, string userid, string date, string specificemail, bool forceSend = false, Expression<Func<WatchDog, bool>> predicate = null, bool debug=false)
            {
                HlidacStatu.Util.Consts.Logger.Info("Starting SendWatchDogs type:" + datatype);

                IEnumerable<int> watchdogs = null;

                using (Lib.Data.DbEntities db = new DbEntities())
                {
                    var data = db.WatchDogs.AsNoTracking()
                        .Where(m => m.StatusId > 0);


                    if (!string.IsNullOrEmpty(userid))
                        data = data.Where(m => m.UserId == userid);

                    if (!string.IsNullOrEmpty(datatype))
                        data = data.Where(m => m.dataType == datatype);

                    if (predicate != null)
                    {
                        data = data.Where(predicate);
                    }

                    if (ids != null)
                        data = data.Where(m => ids.Contains(m.Id.ToString()));

                    watchdogs = data
                        .Select(m => m.Id)
                        .ToArray();
                }


                System.Collections.Concurrent.ConcurrentBag<string> alreadySendInfo = new System.Collections.Concurrent.ConcurrentBag<string>();
                try
                {

                Devmasters.Core.Batch.Manager.DoActionForAll<int, object>(watchdogs,
                    (wdid, obj) =>
                    {
                        string log = "";
                        var mainWatchdog = WatchDog.Load(wdid);
                        var mainWDBs = WatchDog.GetWatchDogBase(mainWatchdog);

                        var notifResult = true;


                        foreach (var wdb in mainWDBs)
                        {
                            HlidacStatu.Util.Consts.Logger.Debug($"Starting watchdog {wdb.ToString()}");


                            var mainDatatype = wdb.OrigWD.dataType;
                            var user = wdb.OrigWD.User();
                            if (user == null) //is not confirmed
                            {
                                var uuser = wdb.OrigWD.UnconfirmedUser();
                                wdb.OrigWD.DisableWDBySystem(DisabledBySystemReasons.NoConfirmedEmail,
                                    alreadySendInfo.Contains(uuser.Id)
                                    );
                                alreadySendInfo.Add(uuser.Id);
                                break; //exit for
                            }
                            if (user != null && wdb.OrigWD.StatusId > 0) //check again, some of them could change to disabled
                            {
                                try
                                {
                                    notifResult = notifResult | _processIndividualWD(wdb, date, null, specificemail, forceSend,debug);


                                }
                                catch (Exception e)
                                {
                                    HlidacStatu.Util.Consts.Logger.Error("WatchDog search error", e);
                                    log += "WatchDog search error " + e.ToString();
                                    //throw;
                                }
                            }
                        }
                        if (notifResult && date == null && forceSend == false)
                        {
                            DateTime toDate = WatchDogProcessor.DefaultRoundWatchdogTime(mainWatchdog.Period, DateTime.Now);

                            if (mainWatchdog.LastSearched.HasValue == false || mainWatchdog.LastSearched < toDate)
                                mainWatchdog.LastSearched = toDate;
                            mainWatchdog.RunCount++;
                            var latestRec = toDate;
                            if (mainWatchdog.LatestRec.HasValue == false)
                                mainWatchdog.LatestRec = latestRec;
                            else if (latestRec > mainWatchdog.LatestRec)
                                mainWatchdog.LatestRec = latestRec;

                            mainWatchdog.LastSent = mainWatchdog.LastSearched;

                            mainWatchdog.Save();
                        }



                        return new Devmasters.Core.Batch.ActionOutputData() { CancelRunning = false, Log = log };
                    },
                    null,
                    HlidacStatu.Util.Consts.outputWriter.OutputWriter,
                    HlidacStatu.Util.Consts.progressWriter.ProgressWriter,
                    System.Diagnostics.Debugger.IsAttached ? false : true);
                }
                catch (Exception e)
                {
                    HlidacStatu.Util.Consts.Logger.Fatal($"Watchdoga DoActionForAll error", e);

                }
            }

            private static bool _processIndividualWD(WatchDogProcessor wd2, string date, string order,
                string specificemail, bool forceSend, bool debug = false)
            {
                HlidacStatu.Util.Consts.Logger.Debug($"Processing specific watchdog ({wd2.OrigWD.dataType}) id {wd2.OrigWD.Id}, date {date?.ToString() ?? "current"}");
                WatchDogProcessor.Result res = null;
                try
                {
                    if (!string.IsNullOrWhiteSpace(date))
                    {
                        res = wd2.DoSearch(date, order);
                        if (debug)
                        {
                            System.IO.File.AppendAllText(
                                @"c:\!\wd.debug.txt",
                                $"{wd2.OrigWD.Id}\t{res.Total}\t{res.IsValid}\t{res.RawQuery}\n"
                                );
                            return false;
                        }

                    }
                    else if (wd2.ReadyToRun() || forceSend || debug)
                    {
                        res = wd2.DoSearch(order);
                        if (debug)
                        {
                            System.IO.File.AppendAllText(
                                @"c:\!\wd.debug.txt",
                                $"{wd2.OrigWD.Id}\t{res.Total}\t{res.IsValid}\t{res.RawQuery}\n"
                                );
                            return false;
                        }
                    }
                }
                catch (Exception e)
                {
                    //wd2.OrigWD.DisableWDBySystem(DisabledBySystemReasons.InvalidQuery);
                    HlidacStatu.Util.Consts.Logger.Warning($"Watchdog search error, watchdog ({wd2.OrigWD.dataType}) id {wd2.OrigWD.Id}, date {date?.ToString() ?? "current"}", e);
                }

                if (res != null)
                {
                    if (res.IsValid == false)
                    {
                        //log += "INVALID QUERY: " + res?.RawQuery ?? "null";
                    }
                    else if (res.Results.Count() > 0)
                    {

                        //create email
                        return wd2.SendNotification(res, specificemail);
                    }
                    else if (res.Total == 0 && wd2.OrigWD.StatusId == 2) //send always, included no results
                    {
                        return wd2.SendNotification(res, specificemail);

                    }

                }
                return false;

            }

        }



    }


}
