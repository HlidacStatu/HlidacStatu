using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public static class Consts
    {

        public static Devmasters.Core.Batch.MultiOutputWriter outputWriter =
            new Devmasters.Core.Batch.MultiOutputWriter(
                Devmasters.Core.Batch.Manager.DefaultOutputWriter,
                new Devmasters.Core.Batch.LoggerWriter(HlidacStatu.Util.Consts.Logger).OutputWriter
            );

        public static Devmasters.Core.Batch.MultiProgressWriter progressWriter =
            new Devmasters.Core.Batch.MultiProgressWriter(
                new Devmasters.Core.Batch.ActionProgressWriter(0.1f).Write,
                new Devmasters.Core.Batch.ActionProgressWriter(10, new Devmasters.Core.Batch.LoggerWriter(HlidacStatu.Util.Consts.Logger).ProgressWriter).Write
            );

        public static RegexOptions DefaultRegexQueryOption = RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline;

        public static System.Globalization.CultureInfo enCulture = System.Globalization.CultureInfo.InvariantCulture; //new System.Globalization.CultureInfo("en-US");
        public static System.Globalization.CultureInfo czCulture = System.Globalization.CultureInfo.GetCultureInfo("cs-CZ");
        public static System.Globalization.CultureInfo csCulture = System.Globalization.CultureInfo.GetCultureInfo("cs");
        public static Random Rnd = new Random();

        public static Devmasters.Core.Logging.Logger Logger = new Devmasters.Core.Logging.Logger("HlidacStatu");

    }
}
