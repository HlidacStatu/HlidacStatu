using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Enhancers
{


    public interface IEnhancer
    {
        string Name { get; }
        string Description { get; }
        bool Update(ref Data.Smlouva item);

        int Priority { get; }
        void SetInstanceData(object data);
    }


}
