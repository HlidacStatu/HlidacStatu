using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Devmasters;
using Devmasters.Enums;

namespace HlidacStatu.Lib.Data.External.Zabbix
{
    public class ZabTools
    {

        static Devmasters.Cache.LocalMemory.AutoUpdatedLocalMemoryCache<List<ZabHost>> webyList =
                new Devmasters.Cache.LocalMemory.AutoUpdatedLocalMemoryCache<List<ZabHost>>(
                TimeSpan.FromMinutes(30), "statniweby_list", (obj) =>
                {
                    try
                    {
                        List<ZabHost> d = new List<ZabHost>();
                        using (var zr = new ZabbixReader())
                        {
                            d.AddRange(zr.RefreshHosts(0) ?? new List<ZabHost>());
                            d.AddRange(zr.RefreshHosts(1) ?? new List<ZabHost>());
                            d.AddRange(zr.RefreshHosts(2) ?? new List<ZabHost>());
                            d.AddRange(zr.RefreshHosts(3) ?? new List<ZabHost>());
                            HlidacStatu.Util.Consts.Logger.Info("Cache statniweby_list refreshnuta");
                        }
                        return d;

                    }
                    catch (Exception e)
                    {
                        HlidacStatu.Util.Consts.Logger.Fatal("ZabTools webyList cache", e);
						return new List<ZabHost>(); //null;

					}
                });

        static Devmasters.Cache.LocalMemory.AutoUpdatedLocalMemoryCache<List<ZabHostAvailability>> webyData =
        new Devmasters.Cache.LocalMemory.AutoUpdatedLocalMemoryCache<List<ZabHostAvailability>>(
                TimeSpan.FromSeconds(140), "statniweby_data", (obj) =>
                {
                    List<ZabHostAvailability> d = null;
                    using (var zr = new ZabbixReader())
                    {
                        d = zr.GetAvailability_Raw(25, ZabTools.Weby(), true)
                            ?.ToList();
                        HlidacStatu.Util.Consts.Logger.Info("Cache statniweby_data refreshnuta");
                        return d;
                    }
                });


        static Devmasters.Cache.LocalMemory.AutoUpdatedLocalMemoryCache<List<ZabHostAvailability>> webyDataLastHour =
        new Devmasters.Cache.LocalMemory.AutoUpdatedLocalMemoryCache<List<ZabHostAvailability>>(
                TimeSpan.FromSeconds(140), "statniweby_data_Lasthour", (obj) =>
                {
                    List<ZabHostAvailability> d = null;
                    using (var zr = new ZabbixReader())
                    {
                        d = zr.GetAvailability_Raw(1, ZabTools.Weby(), true)
                            ?.ToList();
                        HlidacStatu.Util.Consts.Logger.Info("Cache statniweby_data last hour refreshnuta");
                        return d;
                    }
                });
        static Devmasters.Cache.LocalMemory.AutoUpdatedLocalMemoryCache<List<ZabHostSslStatus>> webySslData =
        new Devmasters.Cache.LocalMemory.AutoUpdatedLocalMemoryCache<List<ZabHostSslStatus>>(
                TimeSpan.FromHours(6), "statniweby_ssl_data", (obj) =>
                {
                    List<ZabHostSslStatus> d = null;
                    using (var zr = new ZabbixReader())
                    {
                        d = zr.GetSsl(24*20, ZabTools.WebySslItems())
                            ?.ToList();
                        HlidacStatu.Util.Consts.Logger.Info("Cache statniweby_ssl_data refreshnuta");
                        return d;
                    }
                });


        public static void WebyRefresh()
        {
            webyList.ForceRefreshCache();
        }


        public static IEnumerable<ZabHost> Weby()
        {
            return Weby(null);
        }

        public static IEnumerable<ZabHost> Weby(string group)
        {
            if (!string.IsNullOrEmpty(group))
                return webyList.Get().Where(w => w.groups.Contains(group));
            else
                return webyList.Get();


        }

        public static IEnumerable<string> WebySslItems()
        {
            return WebySslItems(null);
        }
        public static IEnumerable<string> WebySslItems(string group)
        {
            return Weby(group)
                .Select(m => m.itemIdSsl)
                .Where (m=> !string.IsNullOrWhiteSpace(m));
        }

        public static IEnumerable<string> WebyItems()
        {
            return WebyItems(null);
        }
        public static IEnumerable<string> WebyItems(string group)
        {
            return Weby(group)
                .Select(m => m.itemIdResponseTime)
                .Where(m => !string.IsNullOrWhiteSpace(m));
        }


        public static IEnumerable<ZabHostAvailability> WebyDataLastHour(string group)
        {
            return WebyDataLastHour(WebyItems(group));
        }

        public static IEnumerable<ZabHostAvailability> WebyData(string group)
        {
            return WebyData(WebyItems(group));
        }

        public static IEnumerable<ZabHostAvailability> WebyData(IEnumerable<string> items)
        {
            if (items == null)
                return null;

            return webyData.Get()?
                .Where(m => items.Contains(m.Host.itemIdResponseTime));

        }

        public static IEnumerable<ZabHostAvailability> WebyDataLastHour(IEnumerable<string> items)
        {
            if (items == null)
                return null;

            return webyData.Get()?
                .Where(m => items.Contains(m.Host.itemIdResponseTime));

        }

        public static IEnumerable<ZabHostSslStatus> SslStatuses()
        {
            return webySslData.Get();
        }

        public static ZabHostSslStatus SslStatusForHostId(string hostid)
        {
            return webySslData.Get().Where(m => m.Host.hostid == hostid).FirstOrDefault();
        }
        public static string StatusStyleColor(SSLLabsGrades? grade)
        {
            string color = "";
            if (grade.HasValue)
            {
                switch (grade)
                {
                    case SSLLabsGrades.Aplus:
                    case SSLLabsGrades.A:
                    case SSLLabsGrades.Aminus:
                        color = "success";
                        break;
                    case SSLLabsGrades.B:
                    case SSLLabsGrades.C:
                    case SSLLabsGrades.D:
                    case SSLLabsGrades.E:
                        color = "warning";
                        break;
                    case SSLLabsGrades.F:
                    case SSLLabsGrades.T:
                    case SSLLabsGrades.M:
                    case SSLLabsGrades.X:
                        color = "danger";
                        break;
                }
            }
            else
                color = "muted";

            return color;
        }
        public static string StatusDescription(SSLLabsGrades? grade, bool longVersion = false)
        {
            string txt = "";
            if (grade.HasValue)
            {
                switch (grade)
                {
                    case SSLLabsGrades.Aplus:
                    case SSLLabsGrades.A:
                    case SSLLabsGrades.Aminus:
                        txt = "Všechno je v nejlepším pořádku.";
                        if (longVersion)
                            txt = "Všechno je v nejlepším pořádku a web se drží doporučených postupů.";
                        break;
                    case SSLLabsGrades.B:
                    case SSLLabsGrades.C:
                    case SSLLabsGrades.D:
                    case SSLLabsGrades.E:
                        txt = "Služba se nedrží doporučených postupů.";
                        if (longVersion)
                            txt = "Služba se nedrží doporučených postupů a jeho nastavení je zastaralé. Sice to neznamená bezprostřední a snadno zneužitelné ohrožení, ale je to znak špatně spravovaného serveru a útok je za určitých okolností možný.";
                        break;
                    case SSLLabsGrades.F:
                        txt = "Web sice podporuje HTTPS, ale špatně.";
                        if (longVersion)
                            txt = "Web sice podporuje HTTPS, ale jeho parametry jsou nastavené tak špatně, že je skoro spíš na škodu, protože vzbuzuje falešný pocit bezpečí.";
                        break;
                    case SSLLabsGrades.T:
                        txt = "HTTPS používá nedůveryhodný certifikát";
                        if (longVersion)
                            txt = "Služba je chráněna certifikátem od certifikační autority, kterou hlavní prohlížeče neznají hlavní prohlížeče neznají nebo jí nedůvěřují. V českých podmínkách to znamená nejspíše certifikáty vydané našimi vnitrostátními autoritami (ICA, PostSignum a eIdentity), které sice stát zákonem prohlásil za důvěryhodné, ale které nesplňují mezinárodní podmínky nutné proto, aby je za důvěryhodné pokládali i autoři prohlížečů.";
                        break;
                    case SSLLabsGrades.M:
                        txt = "Služba používá certifikát pro jiný server.";
                        if (longVersion)
                            txt = "Služba používá certifikát, který byl vystaven pro jiný název serveru. Může se jednat o chybu v konfiguraci serveru, ale také o to, že na dané IP adrese běží jiný HTTPS web, než jaký testujeme.";
                        break;
                    case SSLLabsGrades.X:
                        txt = "Služba HTTPS nepodporuje vůbec.";
                        break;
                }
            }
            else
                txt = "muted";

            return txt;
        }

        public static string StatusOrigColor(SSLLabsGrades? grade)
        {
            string color = "";
            if (grade.HasValue)
            {
                switch (grade)
                {
                    case SSLLabsGrades.Aplus:
                    case SSLLabsGrades.A:
                    case SSLLabsGrades.Aminus:
                        color = "#7ed84d";
                        break;
                    case SSLLabsGrades.B:
                    case SSLLabsGrades.C:
                    case SSLLabsGrades.D:
                    case SSLLabsGrades.E:
                        color = "#ffa100";
                        break;
                    case SSLLabsGrades.F:
                    case SSLLabsGrades.T:
                    case SSLLabsGrades.M:
                    case SSLLabsGrades.X:
                        color = "#ef251e";
                        break;
                }
            }
            else
                color = "#777777";

            return color;
        }

        public static string StatusHtml(SSLLabsGrades? grade, int size = 40)
        {
            string color = StatusOrigColor(grade);

            return string.Format(
                @"<div class='center-block' style='background:{2};font-family: Arial, Helvetica, sans-serif;text-align: center;width: {0}px;height: {0}px;font-size: {1}px;line-height: {0}px;font-weight: bold;color: #ffffff;'>"
                + (grade?.ToNiceDisplayName() ?? "?")
                + "</div>", size, size / 2, color);

        }


        public static ZabHostAvailability GetHostAvailabilityLong(ZabHost host)
        {
            Devmasters.Cache.LocalMemory.LocalMemoryCache<ZabHostAvailability> webData =
new Devmasters.Cache.LocalMemory.LocalMemoryCache<ZabHostAvailability>(
        TimeSpan.FromMinutes(10), "statniweby_host_" + host.hostid, (obj) =>
        {
            ZabHostAvailability d = null;
            using (var zr = new ZabbixReader())
            {
                d = zr.GetAvailability_Raw(7 * 24 + 1, new ZabHost[] { host }, true).FirstOrDefault();
                HlidacStatu.Util.Consts.Logger.Info("Cache statniweby_host_" + host.hostid + " refreshnuta");
                return d;
            }
        });

            try
            {
                return webData.Get();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static SSLLabsGrades ToSLLabsGrades(string textGrade)
        {
            if (string.IsNullOrEmpty(textGrade))
                return SSLLabsGrades.X;

            switch (textGrade.ToUpper())
            {
                case "A+":
                    return SSLLabsGrades.Aplus;
                case "A":
                    return SSLLabsGrades.A;
                case "A-":
                    return SSLLabsGrades.Aminus;
                case "B":
                    return SSLLabsGrades.B;
                case "C":
                    return SSLLabsGrades.C;
                case "D":
                    return SSLLabsGrades.D;
                case "E":
                    return SSLLabsGrades.E;
                case "F":
                    return SSLLabsGrades.F;
                case "T":
                    return SSLLabsGrades.T;
                case "M":
                    return SSLLabsGrades.M;
                default:
                    return SSLLabsGrades.X;
            }
        }
    }
}