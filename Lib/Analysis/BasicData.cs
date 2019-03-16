namespace HlidacStatu.Lib.Analysis
{
    public class BasicData
    {
        public static BasicData Empty() { return new BasicData(); }
        public BasicData() { }
        public BasicData(BasicData bd)
        {
            this.Pocet = bd.Pocet;
            this.CelkemCena = bd.CelkemCena;
        }
        public BasicData(BasicDataPerYear bdy)
        {
            this.Pocet = bdy.Summary.Pocet;
            this.CelkemCena = bdy.Summary.CelkemCena;
        }

        public long Pocet { get; set; } = 0;
        public decimal CelkemCena { get; set; } = 0;

        public void Add(long pocet, decimal cena)
        {
            this.Pocet = this.Pocet + pocet;
            this.CelkemCena = this.CelkemCena + cena;
        }

        public string ToNiceString(Data.Bookmark.IBookmarkable item, bool html = true, string customUrl = null, bool twoLines = false)
        {

            if (html)
            {
                var s = "<a href='" + (customUrl ?? (item?.GetUrl(false) ?? "")) + "'>" +
                            HlidacStatu.Util.PluralForm.Get((int)this.Pocet, "{0} smlouva;{0} smlouvy;{0} smluv") +
                        "</a>" + (twoLines ? "<br />" : " za ") +
                        "celkem " +
                        HlidacStatu.Lib.Data.Smlouva.NicePrice(this.CelkemCena, html: true, shortFormat: true);
                return s;
            }
            else
                return HlidacStatu.Util.PluralForm.Get((int)this.Pocet, "{0} smlouva;{0} smlouvy;{0} smluv") +
                    " za celkem " + HlidacStatu.Lib.Data.Smlouva.NicePrice(this.CelkemCena, html: false, shortFormat: true);

        }
    }

    public class BasicData<T> : BasicData
    {
        public BasicData()
        {
        }

        public BasicData(T data)
        {
            this.Item = data;
        }
        public BasicData(T data, BasicDataPerYear bdy) : base(bdy)
        {
            this.Item = data;
        }

        public BasicData(T data, BasicData bd) : base(bd)
        {
            this.Item = data;
        }

        public T Item { get; set; } = default(T);
    }


    public class BasicDataForSubject<T> : BasicData<string>
        where T : new()
    {
        public T Detail { get; set; } = new T();

        public string Ico
        {
            get { return this.Item; }
            set { this.Item = value; }
        }
    }

}
