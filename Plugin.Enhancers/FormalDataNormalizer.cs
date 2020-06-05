using Devmasters.Core.Collections;
using HlidacStatu.Lib;
using HlidacStatu.Lib.Enhancers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Plugin.Enhancers
{


    public class FormalDataNormalizer : IEnhancer
    {
        public string Description
        {
            get
            {
                return "FormalNormalizer updater";
            }
        }

        public string Name
        {
            get
            {
                return "FormalNormalizer";
            }
        }

        List<string> ciziStaty = new List<string>();
        public void SetInstanceData(object data)
        { 
            var test = data as List<string>;
            if (test != null)
                ciziStaty = test;
        }
        bool changed = false;
        public bool Update(ref Lib.Data.Smlouva item)
        {


            if (item.Platce != null)
                item.Platce.ico = GetNormalizedIco(item.Platce.ico, "platce.ico", ref item);

            if (item.Prijemce != null)
            {
                foreach (var p in item.Prijemce)
                {
                    p.ico = GetNormalizedIco(p.ico, "prijemce.ico", ref item);
                }
            }

            return changed;
        }

        private string GetNormalizedIco(string ico,string parametrName, ref Lib.Data.Smlouva item)
        {
            if (!string.IsNullOrEmpty(ico))
            {
                var newIco = System.Text.RegularExpressions.Regex.Replace(ico, @"[^0-9]", string.Empty);
                if (newIco != ico && Util.DataValidators.CheckCZICO(newIco))
                {
                    item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Normalizováno IČO", "", parametrName, ico, newIco, this));
                    changed = true;
                    return newIco;
                }
            }
                return ico;
        }

    }
}
