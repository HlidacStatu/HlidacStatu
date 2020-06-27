using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public interface IFlattenedExport
    {
        System.Dynamic.ExpandoObject FlatExport();
    }
}
