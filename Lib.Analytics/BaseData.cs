using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Analytics
{
    public abstract class BaseData<T>
    {
        public T Data { get; set; }

        public abstract decimal TheValue { get; }





    }
}
