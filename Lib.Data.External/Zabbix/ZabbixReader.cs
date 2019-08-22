using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Devmasters.Core;
using Devmasters.Core.Collections;
using HlidacStatu.Lib;

namespace HlidacStatu.Lib.Data.External.Zabbix
{
    public class ZabbixReader
        : IDisposable
    {

        static string zabbixServer = Devmasters.Core.Util.Config.GetConfigValue("ZabbixServerUrl"); 

        ZabbixApi.Zabbix zabbix = new ZabbixApi.Zabbix(Devmasters.Core.Util.Config.GetConfigValue("ZabbixAPIUser"), Devmasters.Core.Util.Config.GetConfigValue("ZabbisAPIPassword"), zabbixServer + "/api_jsonrpc.php");

        public ZabbixReader()
        {
            zabbix.login();
        }

        public void Dispose()
        {
            zabbix.logout();
        }

        public IEnumerable<string> GetItemIdssFromGroup(string groupname)
        {
            return GetHostsFromGroup(groupname)?.Select(m => m.itemIdResponseTime);
        }
        public IEnumerable<string> GetHostIdsFromGroup(string groupname)
        {
            return GetHostsFromGroup(groupname)?.Select(m => m.hostid);
        }
        public IEnumerable<ZabHost> GetHostsFromGroup(string groupname)
        {
            if (zabbix.loggedOn == false)
                return null;

            try
            {

                var command = "hostgroup.get";
                var res = zabbix.objectResponse(command, new
                {
                    output = "shorten",
                    filter = new { name = new string[] { groupname } }
                }
                );
                if (res.result == null)
                    return null;
                string groupId = res.result[0].groupid;

                command = "item.get";
                Dictionary<string, string> keysearch = new Dictionary<string, string>();
                keysearch.Add("key_", "web.test.time[CheckWeb,HP,resp]");

                res = zabbix.objectResponse(command, new
                {
                    selectHosts = new string[] { "hostid", "host", "name", "description" },
                    output = "extend",
                    webitems = true,
                    groupids = groupId,
                    search = keysearch
                });
                var tmp = new List<ZabHost>();

                foreach (var r in res.result)
                {
                    if (r.lastclock != "0")
                    {
                        ZabHost h = new ZabHost(r.hosts[0].hostid, r.hosts[0].host, r.hosts[0].name, r.hosts[0].description);
                        h.itemIdResponseTime = r.itemid;
                        tmp.Add(h);
                    }
                }

                return tmp;
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Fatal("ZabTools webyList cache", e);
                return null;
            }
        }

        public List<ZabHost> RefreshHosts(int priority)
        {
            if (zabbix.loggedOn == false)
                return null;

            try
            {

                var command = "hostgroup.get";
                var res = zabbix.objectResponse(command, new
                {
                    output = "shorten",
                    filter = new { name = new string[] { priority + ". priorita" } }
                }
                );
                string groupId = res.result[0].groupid;

                command = "item.get";
                Dictionary<string, string> keysearch = new Dictionary<string, string>();
                keysearch.Add("key_", "web.test.time[CheckWeb,HP,resp]");

                //keysearch.Add("key_", "ssl.json");

                res = zabbix.objectResponse(command, new
                {
                    selectHosts = new string[] { "hostid", "host", "name", "description" },
                    output = "extend",
                    webitems = true,
                    groupids = groupId,
                    search = keysearch
                });
                var tmp = new List<ZabHost>();

                foreach (var r in res.result)
                {
                    if (true || r.lastclock != "0")
                    {
                        ZabHost h = new ZabHost(r.hosts[0].hostid, r.hosts[0].host, r.hosts[0].name, r.hosts[0].description, new string[] { priority.ToString() });

                        h.itemIdResponseTime = r.itemid;

                        tmp.Add(h);
                    }
                }

                //read response code
                command = "item.get";
                keysearch.Clear(); keysearch.Add("key_", "web.test.rspcode[CheckWeb,HP]");
                res = zabbix.objectResponse(command, new
                {
                    selectHosts = new string[] { "hostid", "host", "name", "description" },
                    output = "extend",
                    webitems = true,
                    groupids = groupId,
                    search = keysearch
                });
                foreach (var r in res.result)
                {
                    if (true || r.lastclock != "0")
                    {
                        var hostid = r.hosts[0].hostid;
                        var exist = tmp.Where(m => m.hostid == hostid).FirstOrDefault();
                        if (exist != null)
                        {
                            exist.itemIdResponseCode = r.itemid;
                        }
                        else
                        {
                            ZabHost h = new ZabHost(r.hosts[0].hostid, r.hosts[0].host, r.hosts[0].name, r.hosts[0].description, new string[] { priority.ToString() });
                            h.itemIdResponseCode = r.itemid;
                            tmp.Add(h);
                        }
                    }
                }

                //read Ssl.Json
                command = "item.get";
                keysearch.Clear(); keysearch.Add("key_", "ssl.json");
                res = zabbix.objectResponse(command, new
                {
                    selectHosts = new string[] { "hostid", "host", "name", "description" },
                    output = "extend",
                    groupids = groupId,
                    search = keysearch
                });
                foreach (var r in res.result)
                {
                    if (true || r.lastclock != "0")
                    {
                        var hostid = r.hosts[0].hostid;
                        var exist = tmp.Where(m => m.hostid == hostid).FirstOrDefault();
                        if (exist != null)
                        {
                            exist.itemIdSsl = r.itemid;
                        }
                        else
                        {
                            ZabHost h = new ZabHost(r.hosts[0].hostid, r.hosts[0].host, r.hosts[0].name, r.hosts[0].description, new string[] { priority.ToString() });
                            h.itemIdSsl = r.itemid;
                            tmp.Add(h);
                        }
                    }
                }



                if (priority == 0)
                {
                    //add EET, NEN API
                    res = zabbix.objectResponse("item.get", new
                    {
                        selectHosts = new string[] { "hostid", "host", "name", "description" },
                        output = "extend",
                        itemids = new string[] { "27939", "30053" }
                    });


                    foreach (var r in res.result)
                    {
                        if (r.lastclock != "0")
                        {
                            ZabHost h = new ZabHost(r.hosts[0].hostid, r.hosts[0].host, r.hosts[0].name, r.hosts[0].description, new string[] { priority.ToString() });
                            h.itemIdResponseTime = r.itemid;

                            tmp.Add(h);
                        }
                    }

                }
                return tmp;
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Fatal("ZabTools webyList cache", e);
                return null;
            }
        }

        public List<ZabHost> GetHostValue(string hostid)
        {
            if (zabbix.loggedOn == false)
                return null;

            try
            {

                var command = "host.get";
                var res = zabbix.objectResponse(command, new
                {
                    output = "shorten",
                    hostids = new { name = new string[] { hostid } }
                }
                );
                if (res.result != null && res.result.Count() > 0)
                    return res.result[0].host;
                else
                    return null;
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Fatal("ZabTools webyList cache", e);
                return null;
            }
        }


        private List<ZabHistoryItem> ReadWebResponseDataHistory(int hoursBack, IEnumerable<string> itemIds, int historyType = 0)
        {
            if (zabbix.loggedOn == false)
                return null;

            int limitForNumOfItems = 60;
            var res = new List<ZabHistoryItem>();
            foreach (var chunk in itemIds.Chunk(limitForNumOfItems))
            {
                res.AddRange(_coreReadData(hoursBack, chunk, historyType));
            }
            return res;
        }

        private List<ZabHistoryItem> ReadSslData(int hoursBack, IEnumerable<string> itemIds)
        {
            if (zabbix.loggedOn == false)
                return null;

            int limitForNumOfItems = 60;
            var res = new List<ZabHistoryItem>();

            try
            {
                foreach (var chunk in itemIds.Chunk(limitForNumOfItems))
                {


                    var command = "history.get";
                    var p = new
                    {
                        output = "extend",
                        //itemids = new String[] { "25859","26117","26879"},
                        itemids = itemIds.Distinct().ToArray(),
                        history = 4,
                        sortfield = "clock",
                        sortorder = "ASC",
                        time_from = DateTimeUtil.ToEpochTimeFromUTC(DateTime.UtcNow.AddHours(-1 * hoursBack)),
                        time_till = DateTimeUtil.ToEpochTimeFromUTC(DateTime.UtcNow)


                    };
                    var zres = zabbix.objectResponse(command, p);
                    List<ZabHistoryItem> history = new List<ZabHistoryItem>();
                    foreach (var r in zres.result)
                    {
                        var h = new ZabHistoryItem()
                        {
                            itemId = r.itemid,
                            clock = DateTimeUtil.FromEpochTimeToUTC(Convert.ToInt64(r.clock)),
                            svalue = r.value
                        };
                        history.Add(h);
                    }
                    res.AddRange(history);
                }

                return res;
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Fatal("ZabTools webyList cache", e);
                return null;
            }

        }

        private List<ZabHistoryItem> _coreReadData(int hoursBack, IEnumerable<string> itemIds, int historyType)
        {
            if (zabbix.loggedOn == false)
                return null;

            /*
             Output can be “shorten”, “refer”, “extend” and array of db field names.

            -shorten: only ids of elements are returned. (for hosts it's hostid, for items itemid etc)
            -refer: element ids are returned with element ids that were used in query. 
            For example request item.get(output: refer, hostids: [1,2,3])) will return items with itemids and hostids.
            -extend: all db fields of element are returned (select *)
            -array of db fields: listed db fields are returned
             */

            try
            {

                var command = "history.get";
                var p = new
                {
                    output = "extend",
                    //itemids = new String[] { "25859","26117","26879"},
                    itemids = itemIds.Distinct().ToArray(),
                    history = historyType,
                    sortfield = "clock",
                    sortorder = "ASC",
                    time_from = DateTimeUtil.ToEpochTimeFromUTC(DateTime.UtcNow.AddHours(-1 * hoursBack)),
                    time_till = DateTimeUtil.ToEpochTimeFromUTC(DateTime.UtcNow)


                };
                var res = zabbix.objectResponse(command, p);
                List<ZabHistoryItem> history = new List<ZabHistoryItem>();
                foreach (var r in res.result)
                {
                    var h = new ZabHistoryItem()
                    {
                        itemId = r.itemid,
                        clock = DateTimeUtil.FromEpochTimeToUTC(Convert.ToInt64(r.clock)).ToLocalTime() ,
                        value = HlidacStatu.Util.ParseTools.ToDecimal(r.value)
                    };
                    history.Add(h);
                }

                return history;
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Fatal("ZabTools webyList cache", e);
                return null;
            }
        }

        public List<ZabHostAvailability_Intervals> GetAvailability_Intervals(int hoursBack, IEnumerable<string> itemIds)
        {
            if (zabbix.loggedOn == false)
                return null;

            var data = ReadWebResponseDataHistory(hoursBack, itemIds);
            if (data == null)
                return null;

            List<ZabHostAvailability_Intervals> ret = new List<ZabHostAvailability_Intervals>();
            DateTime now = DateTime.Now;
            foreach (var host in data.Select(m => m.itemId).Distinct().Select(m => ZabTools.Weby().First(h => h.itemIdResponseTime == m)))
            {
                var hint = data.Where(h => h.itemId == host.itemIdResponseTime).ToArray();
                ret.Add(new ZabHostAvailability_Intervals(host, hint, now));
            }
            return ret;
        }

        public List<ZabHostAvailability_PerMin> GetAvailability_PerMin(int hoursBack, IEnumerable<string> itemIds)
        {
            if (zabbix.loggedOn == false)
                return null;

            var data = ReadWebResponseDataHistory(hoursBack, itemIds);
            if (data == null)
                return null;

            List<ZabHostAvailability_PerMin> ret = new List<ZabHostAvailability_PerMin>();
            DateTime now = DateTime.Now;
            foreach (var host in data.Select(m => m.itemId).Distinct().Select(m => ZabTools.Weby().First(h => h.itemIdResponseTime == m)))
            {
                var hint = data.Where(h => h.itemId == host.itemIdResponseTime).ToArray();
                ret.Add(new ZabHostAvailability_PerMin(host, hint));
            }
            return ret;
        }

        public List<ZabHostAvailability> GetAvailability_Raw(int hoursBack, IEnumerable<ZabHost> hosts, bool fillMissingWithNull = false)
        {
            if (zabbix.loggedOn == false)
                return null;

            var dataResponseTime = ReadWebResponseDataHistory(hoursBack,
                hosts.Select(m => m.itemIdResponseTime)
                    .Where(m => !string.IsNullOrEmpty(m))
                );
            if (dataResponseTime == null)
                return null;


            List<ZabHostAvailability> ret = new List<ZabHostAvailability>();
            DateTime now = DateTime.Now;
            foreach (var host in dataResponseTime.Select(m => m.itemId).Distinct().Select(m => ZabTools.Weby().First(h => h.itemIdResponseTime == m)))
            {
                var hint = dataResponseTime.Where(h => h.itemId == host.itemIdResponseTime).OrderBy(m=>m.clock).ToArray();
                ret.Add(new ZabHostAvailability(host, hint, fillMissingWithNull));
            }

            var dataResponseCode = ReadWebResponseDataHistory(hoursBack,
                    hosts.Select(m => m.itemIdResponseCode)
                        .Where(m => !string.IsNullOrEmpty(m))
                    , 3);

            foreach (var host in dataResponseCode.Select(m => m.itemId).Distinct().Select(m => ZabTools.Weby().First(h => h.itemIdResponseCode == m)))
            {
                var hint = dataResponseCode.Where(h => h.itemId == host.itemIdResponseCode).ToArray();
                //ret.Add(new ZabHostAvailability(host, hint, fillMissingWithNull));
                var foundHost = ret.Where(m => m.Host.hostid == host.hostid).FirstOrDefault();
                if (foundHost != null)
                {
                    //merge status code with response time data
                    foreach (var status in hint)
                    {
                        var foundHistoryItem = foundHost.Data.Where(m => Math.Abs((m.Time - status.clock).TotalSeconds) < 15).FirstOrDefault();
                        if (foundHistoryItem != null)
                        {
                            int httpstatus = (int)status.value;
                            foundHistoryItem.HttpStatusCode = httpstatus;
                            if (httpstatus > 399)
                                foundHistoryItem.Response = ZabAvailability.BadHttpCode;                                

                        }

                    }

                }
            }


            return ret;
        }


        public List<ZabHostSslStatus> GetSsl(int hoursBack, IEnumerable<string> itemIds)
        {
            if (zabbix.loggedOn == false)
                return null;

            var data = ReadSslData(hoursBack, itemIds);
            if (data == null)
                return null;

            List<ZabHostSslStatus> ret = new List<ZabHostSslStatus>();
            DateTime now = DateTime.Now;
            foreach (var host in data.Select(m => m.itemId).Distinct().Select(m => ZabTools.Weby().First(h => h.itemIdSsl == m)))
            {
                var hint = data.Where(h => h.itemId == host.itemIdSsl).ToArray();
                var last = hint.OrderByDescending(h => h.clock).FirstOrDefault();
                if (last != null)
                {
                    ZabHostSslStatus.EndpointStatus[] statuses = Newtonsoft.Json.JsonConvert.DeserializeObject<ZabHostSslStatus.EndpointStatus[]>(last.svalue);
                    statuses = statuses
                        .Where(m =>
                                {
                                    System.Net.IPAddress ip;
                                    if (System.Net.IPAddress.TryParse(m.ipAddress, out ip))
                                    {
                                        return ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
                                    }
                                    else
                                        return false;
                                })
                        .ToArray();

                    ret.Add(new ZabHostSslStatus(host, statuses, last.clock));
                }
            }
            return ret;
        }

    }
}