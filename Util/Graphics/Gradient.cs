using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Util.Graphics
{
    public class Gradient
    {
        RGBA[] colors = null;
        public Gradient(params RGB[] colors)
        {
            this.colors = colors.Select(m => new RGBA(m)).ToArray();
        }

        public RGB Color(decimal val, decimal minvalue, decimal maxvalue)
        {
            if (minvalue == maxvalue)
                return Color(0m);
            //100 - 200
            // 150
            decimal perc = (val - minvalue) / (maxvalue - minvalue);
            return Color(perc);
        }
        public RGB Color(decimal percent)
        {
            return Tools.GetGradientColor(percent, this.colors);
        }

    }
}
