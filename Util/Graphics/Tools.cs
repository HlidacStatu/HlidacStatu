using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Util.Graphics
{
    public partial class Tools
    {

  
        public static RGBA GetGradientOfTwoColor(decimal percent, RGBA start, RGBA end)
        {
            if (percent > 1)
                percent = 1;
            if (percent < 0)
                percent = 0;
            int step = (int)(percent * 100) * 10;

            decimal steps = 1000;
            decimal stepR = ((end.R - start.R) / (steps));
            decimal stepG = ((end.G - start.G) / (steps));
            decimal stepB = ((end.B - start.B) / (steps));
            decimal stepA = ((end.A - start.A) / (steps));

            return new RGBA()
            {
                R = (byte)(start.R + (stepR * step)),
                G = (byte)(start.G + (stepG * step)),
                B = (byte)(start.B + (stepB * step)),
                A = (byte)(start.A + (stepA * step)),
            };

        }

        public static RGBA GetGradientColor(decimal percent, params RGBA[] colors)
        {
            if (percent > 1)
                percent = 1;
            if (percent < 0)
                percent = 0;

            // 10 / (100/(3-1)) = 10/50 = 0
            // 40 / 50 = 0
            // 70 / 50 = 1
            // 100/ 50 = 2
            decimal borderLenght = (1m / (colors.Length - 1));
            int activePart = (int)(percent / borderLenght);
            if (activePart >= colors.Length - 1)
                activePart = colors.Length - 2;

            //decimal percentInPart = borderLenght * activePart + (percent / (colors.Length - 1));
            //parts = 2, active part = 1
            //percent = 0.75
            //percentInPart = 50%

            //parts =3
            //active part = 2
            //percent = 0.67
            //percentInPart = kolem 1%

            decimal percentBase = activePart * borderLenght;
            decimal percentInBase = (percent - percentBase) / borderLenght;

            return GetGradientOfTwoColor(percentInBase, colors[activePart], colors[activePart + 1]);

        }

    }
}
