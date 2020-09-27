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

        public static char Ch = 'Ȼ';

        public static Devmasters.Batch.MultiOutputWriter outputWriter =
            new Devmasters.Batch.MultiOutputWriter(
                Devmasters.Batch.Manager.DefaultOutputWriter,
                new Devmasters.Batch.LoggerWriter(HlidacStatu.Util.Consts.Logger).OutputWriter
            );

        public static Devmasters.Batch.MultiProgressWriter progressWriter =
            new Devmasters.Batch.MultiProgressWriter(
                new Devmasters.Batch.ActionProgressWriter(0.1f).Write,
                new Devmasters.Batch.ActionProgressWriter(10, new Devmasters.Batch.LoggerWriter(HlidacStatu.Util.Consts.Logger).ProgressWriter).Write
            );

        public static RegexOptions DefaultRegexQueryOption = RegexOptions.IgnoreCase 
                                                            | RegexOptions.IgnorePatternWhitespace 
                                                            | RegexOptions.Multiline;

        public static System.Globalization.CultureInfo enCulture = System.Globalization.CultureInfo.InvariantCulture; //new System.Globalization.CultureInfo("en-US");
        public static System.Globalization.CultureInfo czCulture = System.Globalization.CultureInfo.GetCultureInfo("cs-CZ");
        public static System.Globalization.CultureInfo csCulture = System.Globalization.CultureInfo.GetCultureInfo("cs");
        public static Random Rnd = new Random();

        public static Devmasters.Logging.Logger Logger = new Devmasters.Logging.Logger("HlidacStatu");

    }
}
