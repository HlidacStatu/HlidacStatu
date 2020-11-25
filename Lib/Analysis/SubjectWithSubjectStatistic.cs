//using System;
//using System.Linq;


//namespace HlidacStatu.Lib.Analysis
//{


//    public class SubjectWithSubjectStatistic : ComplexStatistic<string[]>
//    {

//        public SubjectWithSubjectStatistic() { }
//        public SubjectWithSubjectStatistic(string icoPlatce, string icoPrijemce)
//            : base(new string[] { icoPlatce, icoPrijemce })
//        {
//        }

//        protected override BasicDataPerYear getBasicStat() => ACore.GetBasicStatisticForSimpleQuery($"icoPlatce:{Item[0]} AND icoPrijemce:{Item[1]}");
//        protected override RatingDataPerYear getRating() => getMyRating();

//        private RatingDataPerYear getMyRating()
//        {
//            var stat = ACore.GetBezCenyStatForSimpleQuery($"icoPlatce:{Item[0]} AND icoPrijemce:{Item[1]}");
//            RatingDataPerYear rdpy = new RatingDataPerYear(
//                    stat.Data.ToDictionary(k => k.Key, v => new RatingData() { NumBezCeny = v.Value.Pocet }),
//                    this.BasicStatPerYear
//                );
//            return rdpy;
//        }

//        public override string ToNiceString(bool html = true, string delimiter = " - ", params string[] otherparams)
//        {
//            return base.ToNiceString(null, html, delimiter, otherparams);
//        }

//    }
//}
