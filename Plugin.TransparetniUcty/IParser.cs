using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Plugin.TransparetniUcty
{
    public interface IParser
    {
        string Name { get; }
        IEnumerable<IBankovniPolozka> GetPolozky(DateTime? fromDate, DateTime? toDate);
    }
}