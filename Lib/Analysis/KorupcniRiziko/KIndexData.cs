using System;
using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{

    [Nest.ElasticsearchType(IdProperty = nameof(Ico))]
    public partial class KIndexData
        : Util.ISocialInfo
    {

        public static KIndexData Empty(string ico, string jmeno = null)
        {
            KIndexData kidx = new KIndexData();
            kidx.Ico = ico;
            kidx.Jmeno = jmeno ?? Lib.Data.Firmy.GetJmeno(ico);
            kidx.roky =  Consts.CalculationYears
                .Select(rok=>Annual.Empty(rok))
                .ToList();
            return kidx;
        }

        public static int[] KIndexLimits = { 0, 3, 6, 9, 12, 15 };



        /// <summary>
        /// Returns IMG html tag with path to icon.
        /// </summary>
        public static string KindexImageIcon(KIndexLabelValues label, string style, bool showNone = false, string title = "")
        {
            if (string.IsNullOrEmpty(title))
            {
                return $"<img title='K–Index {label.ToString()} - Index korupčního rizika' src='{KIndexLabelIconUrl(label, showNone: showNone)}' class='kindex' style='{style}'>";
            }
            return $"<img title='{title}' src='{KIndexLabelIconUrl(label, showNone: showNone)}' class='kindex' style='{style}'>";
        }

        //todo: neměl by tenhle být deprecated a používat se lastReadyKindex
        public Annual LastKIndex()
        {
            return LastReadyKIndex() ?? roky?.OrderByDescending(m => m.Rok)?.FirstOrDefault();
        }

        public Annual LastReadyKIndex()
        {
            return roky?.Where(m => m.KIndexReady)?.OrderByDescending(m => m.Rok)?.FirstOrDefault();
        }

        public decimal? LastKIndexValue()
        {
            return LastReadyKIndex()?.KIndex;
        }
        public KIndexLabelValues LastKIndexLabel()
        {
            return LastKIndexLabel(out int? tmp);
        }
        public KIndexLabelValues LastKIndexLabel(out int? rok)
        {

            var val = LastKIndex();
            rok = null;
            if (val == null)
                return KIndexLabelValues.None;
            else 
            {
                rok = val.Rok;
                return val.KIndexLabel;
            }
        }

        public static string GetMetodikaURL() => "https://texty.hlidacstatu.cz/k-index-index-korupcniho-rizika-metodika/";
        public static string GetKratkaMetodikaURL() => "https://texty.hlidacstatu.cz/co-je-to-k-index/";

        public string GetUrl(bool local = true)
        {
            return GetUrl(local, null);
        }
        public string GetUrl(bool local, int? rok = null)
        {
            string url = "/kindex/detail/" + this.Ico;
            if (rok.HasValue)
                url = url + $"?rok={rok.Value}";
            if (!local)
                url = "https://www.hlidacstatu.cz" + url;

            return url;
        }

        List<Annual> _roky = new List<Annual>();
        public List<Annual> roky { 
            get { return _roky; }

            set {
                _roky = value
                    .Where(m=>m != null)
                    .Select(m => { m.Ico = this.Ico; return m; })
                    .ToList();
            }
        } 

        public Annual ForYear(int year)
        {
            return roky.Where(m => m != null && m.Rok == year).FirstOrDefault();
        }

        public string Ico { get; set; }
        public string Jmeno { get; set; }
        public UcetniJednotkaInfo UcetniJednotka { get; set; } = new UcetniJednotkaInfo();

        [Nest.Date]
        public DateTime LastSaved { get; set; }

        public void Save()
        {
            //calculate fields before saving
            this.LastSaved = DateTime.Now;
            var res = ES.Manager.GetESClient_KIndex().Index<KIndexData>(this, o => o.Id(this.Ico)); //druhy parametr musi byt pole, ktere je unikatni
            if (!res.IsValid)
            {
                throw new ApplicationException(res.ServerError?.ToString());
            }
        }

        public static KIndexLabelValues CalculateLabel(decimal? value)
        {
            KIndexLabelValues label = KIndexLabelValues.None;
            for (int i = 0; i < KIndexLimits.Length; i++)
            {
                if (value > KIndexLimits[i])
                    label = (KIndexLabelValues)i;
            }

            return label;
        }

        public static KIndexData GetDirect(string ico)
        {
            var res = ES.Manager.GetESClient_KIndex().Get<KIndexData>(ico);
            if (res.Found == false)
                return null;
            else if (!res.IsValid)
            {
                throw new ApplicationException(res.ServerError?.ToString());
            }
            else
            {
                KIndexData f = res.Source;
                //fill Annual
                foreach (var r in f.roky)
                {
                    if (r != null)
                        r.Ico = ico;
                }
                return f;
            }
        }



        public static string KIndexLabelColor(KIndexLabelValues label)
        {
            switch (label)
            {
                case KIndexLabelValues.None:
                    return "#000000";
                case KIndexLabelValues.A:
                    return "#00A5FF";
                case KIndexLabelValues.B:
                    return "#0064B4";
                case KIndexLabelValues.C:
                    return "#002D5A";
                case KIndexLabelValues.D:
                    return "#9099A3";
                case KIndexLabelValues.E:
                    return "#F2B41E";
                case KIndexLabelValues.F:
                    return "#D44820";
                default:
                    return "#000000";
            }
        }
  

        public static string KIndexLabelIconUrl(KIndexLabelValues value, bool local = true, bool showNone = false)
        {
            string url = "";
            if (local == false)
                url = "https://www.hlidacstatu.cz";

            bool hranate = Devmasters.Core.Util.Config.GetConfigValue("KIdxIconStyle") == "hranate";
            switch (value)
            {
                case KIndexLabelValues.None:
                    if (showNone)
                        return url + $"/Content/kindex/{(hranate ? "hranate" : "kulate")}/icon-.svg";
                    else
                        return url + "/Content/Img/1x1.png ";
                default:
                    return url + $"/Content/kindex/{(hranate ? "hranate" : "kulate")}/icon{value.ToString()}.svg";

            }
        }


    }
}
