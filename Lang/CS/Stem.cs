using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lang
{
    public class Stem
    {
        public Stem(IEnumerable<string> stems, int position)
        {
            this.Stems = stems.OrderBy(s=>s).ToArray();
            this.Position = position;
        }
        public Stem(string stem, int position)
        {
            this.Stems = new string[] {stem};
            this.Position = position;
        }

        public string First { get {
            if (NumOfStems > 0)
                return Stems[0];
            else
                return string.Empty;
        } }
        public string[] Stems { get; set; }

        public string GetStem(int position)
        {
            if (position < 1)
                position = 1;
            if (position > NumOfStems)
                position = NumOfStems;
            return Stems[position - 1];
        }

        public int Position { get; set; }

        public int NumOfStems { get { return Stems.Length; } }
        public override string ToString()
        {
            return this.First;
        }
    }
}
