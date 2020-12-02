namespace HlidacStatu.Lib.Analysis
{
    public class BasicDataChange
    {
        public decimal PocetChange { get; set; } = 0;
        public decimal CenaChange { get; set; } = 0;
        public decimal PocetChangePerc { get; set; } = 0;
        public decimal CenaChangePerc { get; set; } = 0;
        public BasicDataChange(BasicData prev, BasicData curr)
        {
            if (prev == null)
                throw new System.ArgumentNullException("prev");
            if (curr == null)
                throw new System.ArgumentNullException("curr");

            this.PocetChange = curr.Pocet - prev.Pocet;
            if (prev.Pocet > 0)
                PocetChangePerc = ((PocetChange) / prev.Pocet);

            this.CenaChange = curr.CelkemCena - prev.CelkemCena;
            if (prev.CelkemCena > 0)
                CenaChangePerc = ((CenaChange) / prev.CelkemCena);
        }

        public string PocetChangePercentToSymbolString(bool html = false)
        {
            return RenderChangeSymbol(html, this.PocetChangePerc);
        }
        public string CenaChangePercentToSymbolString(bool html = false)
        {
            return RenderChangeSymbol(html, this.CenaChangePerc);
        }

    }

}
