using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.External.DataSets
{
    public partial class DataSet
    {
        public class Property
        {
            public string Name { get; set; }
            public Type Type { get; set; }
            public string Description { get; set; }

            public string TypePlainly()
            {

                if (this.Type == typeof(Nullable<decimal>)
                    || this.Type == typeof(Nullable<long>)
                    || this.Type == typeof(decimal)
                    || this.Type == typeof(long)
                    )
                    return "číslo";

                if (this.Type == typeof(Nullable<DateTime>)
                    || this.Type == typeof(DateTime)
                    )
                    return "datum a čas";

                if (this.Type == typeof(Nullable<Devmasters.DT.Date>)
                    || this.Type == typeof(Devmasters.DT.Date)
                    )
                    return "datum";
                if (this.Type == typeof(string)
                    )
                    return "text";
                if (this.Type == typeof(Nullable<bool>)
                    || this.Type == typeof(bool)
                    )
                    return "boolean, logická hodnota";
                if (this.Type == typeof(object)
                    )
                    return "objekt";

                if (this.Type == typeof(object[])
                    )
                    return "pole hodnot";

                return "neznámý";
            }

            public List<(string q, string desc)> TypeSamples()
            {
                List<(string q, string desc)> samples = new List<(string q, string desc)>();

                if (this.Type == typeof(Nullable<decimal>)
                    || this.Type == typeof(Nullable<long>)
                    || this.Type == typeof(decimal)
                    || this.Type == typeof(long)
                    )
                {
                    samples.Add(("10", "přesná hodnota 10"));
                    samples.Add((">10", "větší než 10"));
                    samples.Add((">=10", "rovno nebo větší než 10"));
                    samples.Add(("[10 TO *]", "rovno nebo větší než 10"));
                    samples.Add(("[10 TO 20]", "větší nebo rovno než 10 a menší nebo rovno než 20"));
                    samples.Add(("[10 TO 20}", "rovno nebo větší než 10 a menší než 20"));
                }
                if (this.Type == typeof(Nullable<DateTime>)
                    || this.Type == typeof(DateTime)
                    )
                {                     
                    samples.Add(("2020-04-23", "hodnota rovna 23. dubna 2020"));
                    samples.Add(("rok-mesic-den", "přesný den"));
                    samples.Add(("[2020-04-23 TO *]", "od 23. dubna 2020 včetně"));
                    samples.Add(("[* TO 2020-04-23]", "do 23. dubna 2020 23:59:59"));
                    samples.Add(("[* TO 2020-04-23}", "do 23. dubna 2020 00:00, tzn. bez tohoto dne"));
                    samples.Add(("[2020-04-23 TO 2020-04-30]", "hodnota mezi 23 až 30. dubna včetně 30.dubna"));
                }
                if (this.Type == typeof(Nullable<Devmasters.DT.Date>)
                    || this.Type == typeof(Devmasters.DT.Date)
                    )
                {                     
                    samples.Add(("2020-04-23", "hodnota rovna 23. dubna 2020"));
                    samples.Add(("rok-mesic-den", "přesný den"));
                    samples.Add(("[2020-04-23 TO *]", "od 23. dubna 2020 včetně"));
                    samples.Add(("[* TO 2020-04-23]", "do 23. dubna 2020 23:59:59"));
                    samples.Add(("[* TO 2020-04-23}", "do 23. dubna 2020 00:00, tzn. bez tohoto dne"));
                    samples.Add(("[2020-04-23 TO 2020-04-30]", "hodnota mezi 23 až 30. dubna včetně 30.dubna"));
                }
                if (this.Type == typeof(string)
                    )
                {                     
                    samples.Add(("podpora", "parametr obsahuje slovo podpora, všechny tvary slova"));
                    samples.Add(("\"premiér Babiš\"", "obsahuje sousloví 'premiér Babiš', všechny tvary jednotlivých slov, ale pevném pořadí"));
                }
                if (this.Type == typeof(Nullable<bool>)
                    || this.Type == typeof(bool)
                    )
                {                     
                    samples.Add(("true", "logický parametr obsahuje hodnotu 'true'"));
                    samples.Add(("false", "logický parametr obsahuje hodnotu 'false'"));
                }
                if (this.Type == typeof(object)
                    )
                {
                    //samples.Add(("", "jde se ptát pouze na jednotlivé "));
                }

                if (this.Type == typeof(object[])
                    )
                {
                    //samples.Add(("", "jde se ptát pouze na jednotlivé "));
                }

                return samples;
            }

        }

    }
}
