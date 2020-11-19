using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class Firma
    {
        public partial class Statistics
        {
            public partial class VZ : HlidacStatu.Lib.Analytics.StatisticsSubjectPerYear<VZ.Data>
            {
                static VZ nullObj = new VZ() { ICO = "--------" };

                private static Util.Cache.CouchbaseCacheManager<VZ, string> instanceByIco
                    = Util.Cache.CouchbaseCacheManager<VZ, string>.GetSafeInstance("Firma_SmlouvyStatistics", Create, TimeSpan.FromHours(12));

                public VZ() : base() { }

                public VZ(string ico, Dictionary<int, VZ.Data> data)
                    : base(ico, data)
                {

                }
                public static VZ Get(string ico)
                {
                    return instanceByIco.Get(ico);

                }
                public static VZ Get(Firma f)
                {
                    //add cache logic

                    return instanceByIco.Get(f.ICO);
                }
                private static VZ Create(string ico)
                {
                    Firma f = Firmy.Get(ico);
                    if (f.Valid == false)
                        return nullObj;
                    else
                        return Create(f);
                }
                private static VZ Create(Firma f)
                {

                    Dictionary<int, Lib.Analysis.BasicData> _calc_SeZasadnimNedostatkem = Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} and chyby:zasadni", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_UzavrenoOVikendu = Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} AND (hint.denUzavreni:>0)", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_ULimitu = Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} AND ( hint.smlouvaULimitu:>0 )", Lib.Analytics.Consts.RegistrSmluvYearsList);

                    Dictionary<int, Lib.Analysis.BasicData> _calc_NovaFirmaDodavatel = Lib.ES.QueryGrouped.SmlouvyPerYear($"ico:{f.ICO} AND ( hint.pocetDniOdZalozeniFirmy:>-50 AND hint.pocetDniOdZalozeniFirmy:<30 )", Lib.Analytics.Consts.RegistrSmluvYearsList);


                    Dictionary<int, VZ.Data> data = new Dictionary<int, VZ.Data>();
                    foreach (var year in Lib.Analytics.Consts.RegistrSmluvYearsList)
                    {
                        var stat = f.Statistic().RatingPerYear[year];
                        data.Add(year, new VZ.Data()
                        {
                        }
                        );
                    }

                    return new VZ(f.ICO, data);
                }
            }
        }
    }
}
