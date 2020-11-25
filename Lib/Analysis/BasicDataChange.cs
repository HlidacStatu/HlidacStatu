namespace HlidacStatu.Lib.Analysis
{
    //public class BasicDataChange
    //{
    //    public decimal PocetChange { get; set; } = 0;
    //    public decimal CenaChange { get; set; } = 0;
    //    public decimal PocetChangePerc { get; set; } = 0;
    //    public decimal CenaChangePerc { get; set; } = 0;
    //    public BasicDataChange(BasicData prev, BasicData curr)
    //    {
    //        if (prev == null)
    //            throw new System.ArgumentNullException("prev");
    //        if (curr == null)
    //            throw new System.ArgumentNullException("curr");

    //        this.PocetChange = curr.Pocet - prev.Pocet;
    //        if (prev.Pocet>0)
    //            PocetChangePerc =  ((PocetChange) / prev.Pocet);

    //        this.CenaChange = curr.CelkemCena - prev.CelkemCena;
    //        if (prev.CelkemCena > 0)
    //            CenaChangePerc = ((CenaChange) / prev.CelkemCena);
    //    }

    //    public string PocetChangePercentToSymbolString(bool html = false)
    //    {
    //        return RenderChangeSymbol(html, this.PocetChangePerc);
    //    }
    //    public string CenaChangePercentToSymbolString(bool html = false)
    //    {
    //        return RenderChangeSymbol(html, this.CenaChangePerc);
    //    }
    //    private string RenderChangeSymbol(bool html, decimal change)
    //    {

    //        string symbol = "";
    //        if (-0.001m < change && change < 0.001m)
    //            symbol = "↔";
    //        else if (change <= -0.001m)
    //            symbol = "↓";
    //        else
    //            symbol = "↑";
    //        if (html)
    //        {
    //            if (symbol == "↓")
    //                return $"<span class=\"text-danger\">{change.ToString("P2")}&nbsp;&darr;</span>";
    //            else if (symbol == "↑")
    //                return $"<span class=\"text-success\">{change.ToString("P2")}&nbsp;&uarr;</span>";
    //            else
    //                return $"<span class=\"\">{change.ToString("P2")}&nbsp;=</span>";
    //        }
    //        else
    //        {
    //            return $"{change.ToString("P2")} {symbol}";
    //        }
    //    }

    //    public string RenderChangeWord(bool html, decimal change, 
    //        string decreaseTxt = "pokles o {0:P2}", 
    //        string equallyTxt = "bezezměny", 
    //        string increaseTxt = "nárůst o {0:P2}"
    //        )
    //    {
    //        if (-0.001m < change && change < 0.001m)
    //            return equallyTxt.Contains("{0") ? string.Format(equallyTxt, change) : equallyTxt;
    //        else if (change <= -0.001m)
    //            return decreaseTxt.Contains("{0") ? string.Format(decreaseTxt, change) : decreaseTxt;
    //        else
    //            return increaseTxt.Contains("{0") ? string.Format(increaseTxt, change) : increaseTxt;
    //    }
    //}

}
