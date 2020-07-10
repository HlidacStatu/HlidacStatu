using System;
using System.Collections.Generic;

namespace HlidacStatu.Lib.Data.External.RPP
{
        public class ResultList<T>
        {
        public long pocetCelkem { get; set; }
        public IEnumerable<T> seznam { get; set; }

    }
    
}
