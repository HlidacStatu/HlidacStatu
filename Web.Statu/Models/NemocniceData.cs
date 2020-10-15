using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;

using Nest;

using Newtonsoft.Json.Linq;


namespace HlidacStatu.Web.Models
{

        public class NemocniceOnlyData
    {
        public NemocniceOnlyData() { }

        [Nest.Date]
        public DateTime lastUpdated { get; set; }
        public Hospital[] hospitals { get; set; }

        public class Hospital
        {
            
            public string regionFull()
            {
                switch (this.region)
                {
                    case "HKK": return "Královéhradecký kraj";
                    case "JHC": return "Jihočeský kraj";
                    case "JHM": return "Jihomoravský kraj";
                    case "KVK": return "Karlovarský kraj";
                    case "LBK": return "Liberecký kraj";
                    case "MSK": return "Moravskoslezský kraj";
                    case "OLK": return "Olomoucký kraj";
                    case "PAK": return "Pardubický kraj";
                    case "PHA": return "Praha";
                    case "PLK": return "Plzeňský kraj";
                    case "STC": return "Středočeský kraj";
                    case "ULK": return "Ústecký kraj";
                    case "VYS": return "Kraj Vysočina";
                    case "ZLK": return "Zlínský kraj";
                    case "CR": return "Česká republika";
                    default:
                        return "";

                }
            }
            public int nemocniceID { get; set; }
            public string name { get; set; }

            [Nest.Keyword]
            public string region { get; set; }
            [Nest.Date]
            public DateTime lastModified { get; set; }

            //ECMO plicni rizeni, podpora nejvaznejsich pripadu, dela i metabolicke veci
            public int ECMO_volna { get; set; }
            public int ECMO_celkem { get; set; }
            public decimal ECMO_perc() => (ECMO_celkem == 0 ? 0 : (decimal)ECMO_volna / (decimal)ECMO_celkem);

            //Uplna plicni ventilace
            public int UPV_volna { get; set; }
            public int UPV_celkem { get; set; }
            public decimal UPV_perc() => (UPV_celkem == 0 ? 0 : (decimal)UPV_volna / (decimal)UPV_celkem);

            //CRRT - kontinualni dialyza, tezke pripady, selhani ledvin
            public int CRRT_volna { get; set; }
            public int CRRT_celkem { get; set; }
            public decimal CRRT_perc() => (CRRT_celkem == 0 ? 0 : (decimal)CRRT_volna / (decimal)CRRT_celkem);

            //IHD - intermitetni dialyza, co nejede kontinualne
            public int IHD_volna { get; set; }
            public int IHD_celkem { get; set; }
            public decimal IHD_perc() => (IHD_volna == 0 ? 0 : (decimal)IHD_volna / (decimal)IHD_celkem);


            public int AROJIP_luzka_celkem { get; set; }
            public int AROJIP_luzka_covid { get; set; }
            public int AROJIP_luzka_necovid { get; set; }
            public decimal AROJIP_perc() => (AROJIP_luzka_celkem == 0 ? 0 : ((decimal)AROJIP_luzka_covid + (decimal)AROJIP_luzka_necovid) / (decimal)AROJIP_luzka_celkem);

            public int Standard_luzka_s_kyslikem_celkem { get; set; }
            public int Standard_luzka_s_kyslikem_covid { get; set; }
            public int Standard_luzka_s_kyslikem_necovid { get; set; }
            public decimal Standard_luzka_s_kyslikem_perc() => (Standard_luzka_s_kyslikem_celkem == 0 ? 0 : ((decimal)Standard_luzka_s_kyslikem_covid + (decimal)Standard_luzka_s_kyslikem_necovid) / (decimal)Standard_luzka_s_kyslikem_celkem);



            public int Lekari_AROJIP_celkem { get; set; }
            public int Sestry_AROJIP_celkem { get; set; }
            public int Ventilatory_prenosne_celkem { get; set; }
            public int Ventilatory_operacnisal_celkem { get; set; }
            public int Standard_luzka_celkem { get; set; }
            public int Standard_luzka_s_monitor_celkem { get; set; }
        }

        public static NemocniceOnlyData Diff(NemocniceOnlyData f, NemocniceOnlyData l)
        {
            NemocniceOnlyData d = new NemocniceOnlyData();
            d.lastUpdated = new DateTime((l.lastUpdated - f.lastUpdated).Ticks);
            List<NemocniceOnlyData.Hospital> hs = new List<Hospital>();
            foreach (var fh in f.hospitals)
            {
                Hospital h = new Hospital();
                Hospital lh = l.hospitals.FirstOrDefault(m => m.nemocniceID == fh.nemocniceID);
                if (lh != null)
                {

                    hs.Add(Diff(fh, lh));
                }

            }
            d.hospitals = hs.ToArray();
            return d;
        }

        public static Hospital Diff(Hospital fh, Hospital lh)
        {
            Hospital h = new Hospital();
            h.lastModified = new DateTime(Math.Abs((lh.lastModified - fh.lastModified).Ticks));

            h.AROJIP_luzka_celkem = lh.AROJIP_luzka_celkem - fh.AROJIP_luzka_celkem;
            h.AROJIP_luzka_covid = lh.AROJIP_luzka_covid - fh.AROJIP_luzka_covid;
            h.AROJIP_luzka_necovid = lh.AROJIP_luzka_necovid - fh.AROJIP_luzka_necovid;
            h.CRRT_celkem = lh.CRRT_celkem - fh.CRRT_celkem;
            h.CRRT_volna = lh.CRRT_volna - fh.CRRT_volna;
            h.ECMO_celkem = lh.ECMO_celkem - fh.ECMO_celkem;
            h.ECMO_volna = lh.ECMO_volna - fh.ECMO_volna;

            h.IHD_celkem = lh.IHD_celkem - fh.IHD_celkem;
            h.IHD_volna = lh.IHD_volna - fh.IHD_volna;
            h.Lekari_AROJIP_celkem = lh.Lekari_AROJIP_celkem - fh.Lekari_AROJIP_celkem;
            h.name = lh.name;
            h.nemocniceID = lh.nemocniceID;
            h.region = lh.region;
            h.Sestry_AROJIP_celkem = lh.Sestry_AROJIP_celkem - fh.Sestry_AROJIP_celkem;
            h.Standard_luzka_celkem = lh.Standard_luzka_celkem - fh.Standard_luzka_celkem;
            h.Standard_luzka_s_kyslikem_celkem = lh.Standard_luzka_s_kyslikem_celkem - fh.Standard_luzka_s_kyslikem_celkem;
            h.Standard_luzka_s_kyslikem_covid = lh.Standard_luzka_s_kyslikem_covid - fh.Standard_luzka_s_kyslikem_covid;
            h.Standard_luzka_s_kyslikem_necovid = lh.Standard_luzka_s_kyslikem_necovid - fh.Standard_luzka_s_kyslikem_necovid;
            h.Standard_luzka_s_monitor_celkem = lh.Standard_luzka_s_monitor_celkem - fh.Standard_luzka_s_monitor_celkem;
            h.UPV_celkem = lh.UPV_celkem - fh.UPV_celkem;
            h.UPV_volna = lh.UPV_volna - fh.UPV_volna;
            h.Ventilatory_operacnisal_celkem = lh.Ventilatory_operacnisal_celkem - fh.Ventilatory_operacnisal_celkem;
            h.Ventilatory_prenosne_celkem = lh.Ventilatory_prenosne_celkem - fh.Ventilatory_prenosne_celkem;
            return h;
        }

        public static Hospital Aggregate(IEnumerable<Hospital> hospitals)
        {
            Hospital h = new Hospital();

            h.AROJIP_luzka_celkem = hospitals.Sum(m => m.AROJIP_luzka_celkem);
            h.AROJIP_luzka_covid = hospitals.Sum(m => m.AROJIP_luzka_covid);
            h.AROJIP_luzka_necovid = hospitals.Sum(m => m.AROJIP_luzka_necovid);
            h.CRRT_celkem = hospitals.Sum(m => m.CRRT_celkem);
            h.CRRT_volna = hospitals.Sum(m => m.CRRT_volna);
            h.ECMO_celkem = hospitals.Sum(m => m.ECMO_celkem);
            h.ECMO_volna = hospitals.Sum(m => m.ECMO_volna);


            h.lastModified = hospitals.Max(m => m.lastModified);
            h.IHD_celkem = hospitals.Sum(m => m.IHD_celkem);
            h.IHD_volna = hospitals.Sum(m => m.IHD_volna);
            h.Lekari_AROJIP_celkem = hospitals.Sum(m => m.Lekari_AROJIP_celkem);
            h.name = "";
            h.nemocniceID = 0;
            h.region = hospitals.First().region;
            h.Sestry_AROJIP_celkem = hospitals.Sum(m => m.Sestry_AROJIP_celkem);
            h.Standard_luzka_celkem = hospitals.Sum(m => m.Standard_luzka_celkem);
            h.Standard_luzka_s_kyslikem_celkem = hospitals.Sum(m => m.Standard_luzka_s_kyslikem_celkem);
            h.Standard_luzka_s_kyslikem_covid = hospitals.Sum(m => m.Standard_luzka_s_kyslikem_covid);
            h.Standard_luzka_s_kyslikem_necovid = hospitals.Sum(m => m.Standard_luzka_s_kyslikem_necovid);
            h.Standard_luzka_s_monitor_celkem = hospitals.Sum(m => m.Standard_luzka_s_monitor_celkem);
            h.UPV_celkem = hospitals.Sum(m => m.UPV_celkem);
            h.UPV_volna = hospitals.Sum(m => m.UPV_volna);
            h.Ventilatory_operacnisal_celkem = hospitals.Sum(m => m.Ventilatory_operacnisal_celkem);
            h.Ventilatory_prenosne_celkem = hospitals.Sum(m => m.Ventilatory_prenosne_celkem);

            return h;
        }
        public static DateTime? ToDateTime(string value, params string[] formats)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            foreach (var f in formats)
            {
                var dt = ToDateTime(value, f);
                if (dt.HasValue)
                    return dt;
            }
            return null;
        }

        public static DateTime? ToDateTime(string value, string format)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            DateTime tmp;
            if (DateTime.TryParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal | System.Globalization.DateTimeStyles.AllowWhiteSpaces, out tmp))
                return new DateTime?(tmp);
            else
                return null;
        }

    }

    public class NemocniceData
    {


        public NemocniceData() { }

        [Nest.Date]
        public DateTime lastUpdated { get; set; }
        public Region[] regions { get; set; }

        public class Region
        {
            public string name { get; set; }

            [Nest.Keyword]
            public string region { get; set; }
            [Nest.Date]
            public DateTime lastModified { get; set; }

            //ECMO plicni rizeni, podpora nejvaznejsich pripadu, dela i metabolicke veci
            public int ECMO_volna { get; set; }
            public int ECMO_celkem { get; set; }
            public decimal ECMO_perc() => (ECMO_celkem == 0 ? 0 : (decimal)ECMO_volna / (decimal)ECMO_celkem);

            //Uplna plicni ventilace
            public int UPV_volna { get; set; }
            public int UPV_celkem { get; set; }
            public decimal UPV_perc() => (UPV_celkem == 0 ? 0 : (decimal)UPV_volna / (decimal)UPV_celkem);

            //CRRT - kontinualni dialyza, tezke pripady, selhani ledvin
            public int CRRT_volna { get; set; }
            public int CRRT_celkem { get; set; }
            public decimal CRRT_perc() => (CRRT_celkem == 0 ? 0 : (decimal)CRRT_volna / (decimal)CRRT_celkem);

            //IHD - intermitetni dialyza, co nejede kontinualne
            public int IHD_volna { get; set; }
            public int IHD_celkem { get; set; }
            public decimal IHD_perc() => (IHD_volna == 0 ? 0 : (decimal)IHD_volna / (decimal)IHD_celkem);


            public int AROJIP_luzka_celkem { get; set; }
            public int AROJIP_luzka_covid { get; set; }
            public int AROJIP_luzka_necovid { get; set; }
            public decimal AROJIP_perc() => (AROJIP_luzka_celkem == 0 ? 0 : ((decimal)AROJIP_luzka_covid + (decimal)AROJIP_luzka_necovid) / (decimal)AROJIP_luzka_celkem);

            public int Standard_luzka_s_kyslikem_celkem { get; set; }
            public int Standard_luzka_s_kyslikem_covid { get; set; }
            public int Standard_luzka_s_kyslikem_necovid { get; set; }
            public decimal Standard_luzka_s_kyslikem_perc() => (Standard_luzka_s_kyslikem_celkem == 0 ? 0 : ((decimal)Standard_luzka_s_kyslikem_covid + (decimal)Standard_luzka_s_kyslikem_necovid) / (decimal)Standard_luzka_s_kyslikem_celkem);



            public int Lekari_AROJIP_celkem { get; set; }
            public int Lekari_AROJIP_dostupni { get; set; }
            public int Sestry_AROJIP_celkem { get; set; }
            public int Sestry_AROJIP_dostupni { get; set; }
            public int Ventilatory_prenosne_celkem { get; set; }
            public int Ventilatory_operacnisal_celkem { get; set; }
            public int Standard_luzka_celkem { get; set; }
            public int Standard_luzka_s_monitor_celkem { get; set; }

            public string regionFull()
            {
                switch (this.region)
                {
                    case "HKK": return "Královéhradecký kraj";
                    case "JHC": return "Jihočeský kraj";
                    case "JHM": return "Jihomoravský kraj";
                    case "KVK": return "Karlovarský kraj";
                    case "LBK": return "Liberecký kraj";
                    case "MSK": return "Moravskoslezský kraj";
                    case "OLK": return "Olomoucký kraj";
                    case "PAK": return "Pardubický kraj";
                    case "PHA": return "Praha";
                    case "PLK": return "Plzeňský kraj";
                    case "STC": return "Středočeský kraj";
                    case "ULK": return "Ústecký kraj";
                    case "VYS": return "Kraj Vysočina";
                    case "ZLK": return "Zlínský kraj";
                    case "CR": return "Česká republika";
                    default:
                        return "";

                }
            }
        }

        public static NemocniceData Diff(NemocniceData f, NemocniceData l)
        {
            NemocniceData d = new NemocniceData();
            d.lastUpdated = new DateTime((l.lastUpdated - f.lastUpdated).Ticks);
            List<NemocniceData.Region> hs = new List<Region>();
            foreach (var fh in f.regions)
            {
                Region h = new Region();
                Region lh = l.regions.FirstOrDefault(m => m.region == fh.region);

                if (lh != null)
                {

                    hs.Add(Diff(fh, lh));
                }

            }
            d.regions = hs.ToArray();
            return d;
        }

        public static Region Diff(Region fh, Region lh)
        {
            Region h = new Region();
            h.lastModified = new DateTime(Math.Abs((lh.lastModified - fh.lastModified).Ticks));

            h.AROJIP_luzka_celkem = lh.AROJIP_luzka_celkem - fh.AROJIP_luzka_celkem;
            h.AROJIP_luzka_covid = lh.AROJIP_luzka_covid - fh.AROJIP_luzka_covid;
            h.AROJIP_luzka_necovid = lh.AROJIP_luzka_necovid - fh.AROJIP_luzka_necovid;
            h.CRRT_celkem = lh.CRRT_celkem - fh.CRRT_celkem;
            h.CRRT_volna = lh.CRRT_volna - fh.CRRT_volna;
            h.ECMO_celkem = lh.ECMO_celkem - fh.ECMO_celkem;
            h.ECMO_volna = lh.ECMO_volna - fh.ECMO_volna;

            h.IHD_celkem = lh.IHD_celkem - fh.IHD_celkem;
            h.IHD_volna = lh.IHD_volna - fh.IHD_volna;
            h.Lekari_AROJIP_celkem = lh.Lekari_AROJIP_celkem - fh.Lekari_AROJIP_celkem;
            h.Lekari_AROJIP_dostupni = lh.Lekari_AROJIP_dostupni- fh.Lekari_AROJIP_dostupni;
            h.name = lh.name;
            //h.regionId = lh.regionId;
            h.region = lh.region;
            h.Sestry_AROJIP_celkem = lh.Sestry_AROJIP_celkem - fh.Sestry_AROJIP_celkem;
            h.Sestry_AROJIP_dostupni = lh.Sestry_AROJIP_dostupni - fh.Sestry_AROJIP_dostupni;
            h.Standard_luzka_celkem = lh.Standard_luzka_celkem - fh.Standard_luzka_celkem;
            h.Standard_luzka_s_kyslikem_celkem = lh.Standard_luzka_s_kyslikem_celkem - fh.Standard_luzka_s_kyslikem_celkem;
            h.Standard_luzka_s_kyslikem_covid = lh.Standard_luzka_s_kyslikem_covid - fh.Standard_luzka_s_kyslikem_covid;
            h.Standard_luzka_s_kyslikem_necovid = lh.Standard_luzka_s_kyslikem_necovid - fh.Standard_luzka_s_kyslikem_necovid;
            h.Standard_luzka_s_monitor_celkem = lh.Standard_luzka_s_monitor_celkem - fh.Standard_luzka_s_monitor_celkem;
            h.UPV_celkem = lh.UPV_celkem - fh.UPV_celkem;
            h.UPV_volna = lh.UPV_volna - fh.UPV_volna;
            h.Ventilatory_operacnisal_celkem = lh.Ventilatory_operacnisal_celkem - fh.Ventilatory_operacnisal_celkem;
            h.Ventilatory_prenosne_celkem = lh.Ventilatory_prenosne_celkem - fh.Ventilatory_prenosne_celkem;
            return h;
        }

        public static Region Aggregate(IEnumerable<Region> hospitals)
        {
            Region h = new Region();

            h.AROJIP_luzka_celkem = hospitals.Sum(m => m.AROJIP_luzka_celkem);
            h.AROJIP_luzka_covid = hospitals.Sum(m => m.AROJIP_luzka_covid);
            h.AROJIP_luzka_necovid = hospitals.Sum(m => m.AROJIP_luzka_necovid);
            h.CRRT_celkem = hospitals.Sum(m => m.CRRT_celkem);
            h.CRRT_volna = hospitals.Sum(m => m.CRRT_volna);
            h.ECMO_celkem = hospitals.Sum(m => m.ECMO_celkem);
            h.ECMO_volna = hospitals.Sum(m => m.ECMO_volna);


            h.lastModified = hospitals.Max(m => m.lastModified);
            h.IHD_celkem = hospitals.Sum(m => m.IHD_celkem);
            h.IHD_volna = hospitals.Sum(m => m.IHD_volna);
            h.Lekari_AROJIP_celkem = hospitals.Sum(m => m.Lekari_AROJIP_celkem);
            h.Lekari_AROJIP_dostupni= hospitals.Sum(m => m.Lekari_AROJIP_dostupni);
            h.name = "";
            //h.regionId = 0;
            h.region = hospitals.First().region;
            h.Sestry_AROJIP_celkem = hospitals.Sum(m => m.Sestry_AROJIP_celkem);
            h.Sestry_AROJIP_dostupni= hospitals.Sum(m => m.Sestry_AROJIP_dostupni);
            h.Standard_luzka_celkem = hospitals.Sum(m => m.Standard_luzka_celkem);
            h.Standard_luzka_s_kyslikem_celkem = hospitals.Sum(m => m.Standard_luzka_s_kyslikem_celkem);
            h.Standard_luzka_s_kyslikem_covid = hospitals.Sum(m => m.Standard_luzka_s_kyslikem_covid);
            h.Standard_luzka_s_kyslikem_necovid = hospitals.Sum(m => m.Standard_luzka_s_kyslikem_necovid);
            h.Standard_luzka_s_monitor_celkem = hospitals.Sum(m => m.Standard_luzka_s_monitor_celkem);
            h.UPV_celkem = hospitals.Sum(m => m.UPV_celkem);
            h.UPV_volna = hospitals.Sum(m => m.UPV_volna);
            h.Ventilatory_operacnisal_celkem = hospitals.Sum(m => m.Ventilatory_operacnisal_celkem);
            h.Ventilatory_prenosne_celkem = hospitals.Sum(m => m.Ventilatory_prenosne_celkem);

            return h;
        }


        public NemocniceData PoKrajich()
        {
            NemocniceData krajF = new NemocniceData();
            List<Region> krajFH = new List<Region>();
            //int krajId = 0;
            List<string> kraje = new string[] { "PHA", "STC", "JHM", "MSK" }.Union(this.regions.Select(m => m.region).Distinct()).ToList();
            foreach (var kraj in kraje.OrderBy(o => kraje.IndexOf(o)))
            {
                var hsF = this.regions.Where(m => m.region == kraj).ToArray();
                var fd = NemocniceData.Aggregate(hsF); //fd.regionId = ++krajId;
                fd.name = hsF.First().regionFull();
                krajFH.Add(fd);
            }
            krajF.regions = krajFH.ToArray(); krajF.lastUpdated = this.lastUpdated;

            return krajF;
        }
        public NemocniceData CelaCR()
        {
            NemocniceData cr = new NemocniceData();
            List<Region> cdH = new List<Region>();
            var fd = NemocniceData.Aggregate(this.regions); //fd.regionId = 0;
            fd.name = "Celá ČR";
            fd.region = "CR";
            cdH.Add(fd);
            cr.regions = cdH.ToArray();
            cr.lastUpdated = this.lastUpdated;

            return cr;
        }

        public static DateTime? ToDateTime(string value, params string[] formats)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            foreach (var f in formats)
            {
                var dt = ToDateTime(value, f);
                if (dt.HasValue)
                    return dt;
            }
            return null;
        }

        public static DateTime? ToDateTime(string value, string format)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            DateTime tmp;
            if (DateTime.TryParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal | System.Globalization.DateTimeStyles.AllowWhiteSpaces, out tmp))
                return new DateTime?(tmp);
            else
                return null;
        }


        static ElasticClient _client = null;
        static object _clientLockObj = new object();
        public static ElasticClient Client()
        {
            if (_client == null)
            {
                lock (_clientLockObj)
                {
                    if (_client == null)
                    {
                        ConnectionSettings sett = Lib.ES.Manager.GetElasticSearchConnectionSettings("disp_intens_pece");
                        _client = new Nest.ElasticClient(sett);
                    }
                }

            }
            return _client;

        }




    }
}
