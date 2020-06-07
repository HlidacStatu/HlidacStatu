using System;
using System.Linq;
using System.Collections.Generic;
using HlidacStatu.Lib.Data;
using System.Xml.Linq;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public partial class KIndexData
    {
        public class UcetniJednotkaInfo
        {
            public static UcetniJednotkaInfo Load(string ico)
            {
                using (DbEntities db = new DbEntities())
                {
                    var i = db.UcetniJednotka.Where(m => m.ico == ico)
                        .ToArray()
                        .OrderByDescending(m => (m.end_date ?? DateTime.MaxValue))
                        .ToArray()
                        .FirstOrDefault();

                    if (i == null)
                        return null;

                    UcetniJednotkaInfo uji = new UcetniJednotkaInfo();
                    uji.Cofog = COFOG.Parse(i.cofog_id.ToString());
                    uji.DruhUcetniJednotky = i.druhuj_id ?? 0;
                    uji.PodDruhUcetniJednotky = i.poddruhuj_id ?? 0;
                    uji.FormaUcetniJednotky = i.forma_id ?? 0;
                    uji.InstitucionalniSektor = i.isektor_id ?? 0;
                    uji.PocetObyvatelObce = i.pocob ?? 0;
                    return uji;

                }
            }


            //https://monitor.statnipokladna.cz/datovy-katalog/ciselniky/prohlizec/9
            public int DruhUcetniJednotky { get; set; }
            public string DruhUcetniJednotkyPopis()
            {
                return GetPopis("druhuj", this.DruhUcetniJednotky);
            }




            //https://monitor.statnipokladna.cz/datovy-katalog/ciselniky/prohlizec/24

            public int PodDruhUcetniJednotky { get; set; }
            //public string PodDruhUcetniJednotkyPopis()
            //{
            //    return GetPopis("poddruhuj", this.PodDruhUcetniJednotky);
            //}

            //https://monitor.statnipokladna.cz/datovy-katalog/ciselniky/prohlizec/12
            public int FormaUcetniJednotky { get; set; }
            public string FormaUcetniJednotkyPopis()
            {
                return GetPopis("forma", this.FormaUcetniJednotky);
            }


            //https://monitor.statnipokladna.cz/datovy-katalog/ciselniky/prohlizec/17
            public int InstitucionalniSektor { get; set; }
            public string InstitucionalniSektorPopis()
            {
                return GetPopis("isektor", this.InstitucionalniSektor);
            }

            //KLASIFIKACE FUNKCÍ VLÁDNÍCH INSTITUCÍ
            //https://monitor.statnipokladna.cz/datovy-katalog/ciselniky/prohlizec/15
            //https://www.czso.cz/csu/czso/klasifikace_funkci_vladnich_instituci_-cz_cofog-
            public COFOG Cofog { get; set; }

            public int PocetObyvatelObce { get; set; } = 0;


            static private object lockObj = new object();
            static Dictionary<string, Dictionary<int, string>> _popisy = new Dictionary<string, Dictionary<int, string>>();
            static private string GetPopis(string prefix, int value)
            {
                if (_popisy.ContainsKey(prefix) == false)
                {
                    lock (lockObj)
                    {
                        if (_popisy.ContainsKey(prefix) == false)
                        {

                            var val = GetCiselnik(prefix);
                            if (val != null)
                                _popisy.Add(prefix, val);
                        }
                    }
                }
                return _popisy[prefix][value];
            }


            private static Dictionary<int, string> GetCiselnik(string propertyPrefix)
            {
                string url = $"https://monitor.statnipokladna.cz/data/xml/{propertyPrefix}.xml";
                try
                {
                    using (Devmasters.Net.Web.URLContent net = new Devmasters.Net.Web.URLContent(url))
                    {
                        net.Timeout = 1000 * 180;
                        var d = net.GetContent();
                        XElement xe = XElement.Parse(d.Text);

                        return xe.Elements()
                            .Select(m => new
                            {
                                k = Convert.ToInt32(m.Element($"{propertyPrefix}_id").Value),
                                v = m.Element($"{propertyPrefix}_nazev").Value
                            })
                            .ToDictionary(m => m.k, m => m.v);

                    }

                }
                catch (Exception e)
                {

                    return null;
                }

            }
        }

    }

}

