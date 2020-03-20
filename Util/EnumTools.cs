using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public static class EnumTools
    {


        public static TEnum ConvertToEnum<TEnum>(this int input) where TEnum : Enum
        {
            return (TEnum)Enum.Parse(typeof(TEnum), input.ToString());
        }

        public static IEnumerable<T> GetUniqueFlags<T>(this Enum flags) where T : Enum
        {
            foreach (Enum value in Enum.GetValues(flags.GetType()))
                if (flags.HasFlag(value))
                    yield return (T)value;
        }

    }
}
