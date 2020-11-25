//namespace HlidacStatu.Lib.Analysis
//{
//    public class SubjectStatisticWithDetail<T>
//           where T : new()
//    {
//        public SubjectStatisticWithDetail(string ico)
//        {
//            Statistic = new SubjectStatistic(ico);
//        }
//        public SubjectStatisticWithDetail(Lib.Data.Firma subject)
//        {
//            Statistic = new SubjectStatistic(subject.ICO);
//        }

//        public SubjectStatistic Statistic { get; set; }
//        public T Detail { get; set; } = new T();

//    }

//    public class SubjectStatistic : ComplexStatistic<string>
//    {
//        private Data.Firma _subj = null;

//        public Lib.Data.Firma Subject()
//        {
//            if (_subj == null)
//            {
//                _subj = Data.Firmy.Get(this.Item);
//            }
//            return _subj;
//        }

//        public SubjectStatistic() { }
//        public SubjectStatistic(string ico) : this(Lib.Data.Firmy.Get(ico))
//        { }

//        public SubjectStatistic(Lib.Data.Firma subject)
//            : base(subject.ICO)
//        {
//        }

//        protected override BasicDataPerYear getBasicStat() => ACore.GetBasicStatisticForICO(this.Item);
//        protected override RatingDataPerYear getRating() => ACore.GetRatingForICO(this.Item);

//        public override string ToNiceString(bool html = true, string delimiter = " - ", params string[] otherparams)
//        {
//            return base.ToNiceString(this.Subject(), html, delimiter, otherparams);
//        }

//    }
//}
