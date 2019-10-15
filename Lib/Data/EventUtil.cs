using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public static class EventUtil
    {

        public static string RenderDatum(this FirmaEvent oEvent, string txtOd = "", string txtDo = " - ", string dateFormat = "yyyy", string template = "{0}")
        {
            return RenderDatum(oEvent.DatumOd, oEvent.DatumDo, txtOd, txtDo, dateFormat, template);
        }
        public static string RenderDatum(this OsobaEvent oEvent, string txtOd = "", string txtDo = " - ", string dateFormat = "yyyy", string template = "{0}")
        {
            return RenderDatum(oEvent.DatumOd, oEvent.DatumDo, txtOd, txtDo, dateFormat, template);
        }
        public static string RenderDatum(DateTime? DatumOd, DateTime? DatumDo, string txtOd = "", string txtDo = " - ", string dateFormat = "yyyy", string template = "{0}")
        {
            if (DatumOd.HasValue && DatumDo.HasValue)
            {
                //check whole year.
                if (DatumOd.Value.Year == DatumDo.Value.Year
                    && DatumOd.Value.Month == 1 && DatumDo.Value.Month == 12
                    && DatumOd.Value.Day == 1 && DatumDo.Value.Day == 31
                    )
                    return string.Format(template, DatumOd.Value.Year.ToString());
                else
                {
                    return string.Format(template, 
                        string.Format("{0} {1} {2} {3}",
                           txtOd,
                           DatumOd.Value.ToString(dateFormat),
                           txtDo,
                           DatumDo.Value.ToString(dateFormat)
                           ).Trim()
                        );
                }
            }
            else if (DatumOd.HasValue)
            {
                return string.Format(template, 
                        string.Format("{0} {1}",
                            txtOd,
                            DatumOd.Value.ToString(dateFormat)
                            ).Trim()
                        );
            }
            else if (DatumDo.HasValue)
            {
                return string.Format(template, 
                        string.Format("{0} {1}",
                        txtDo,
                        DatumDo.Value.ToString(dateFormat)
                        ).Trim()
                    );
            }
            else
                return string.Empty;
        }

    }
}
