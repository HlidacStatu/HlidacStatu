using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Plugin.TransparetniUcty
{
    public class Tester
    {
        public static void Start()
        {
            IBankovniUcet[] ucty = new SimpleBankovniUcet[]
            {
                    new SimpleBankovniUcet()
                    {
                        CisloUctu="9711283001/5500",
                        Url = "https://www.rb.cz/cs/o-nas/povinne-uverejnovane-informace/transparentni-ucty?mvcPath=transactions&accountNumber=9711283001"
                    }
            };

            int num = 0;
            foreach (var ucet in ucty)
            {
                num++;
                Console.WriteLine(num + " ucet " + ucet.CisloUctu);
                var parser = new Plugin.TransparetniUcty.AutoParser(ucet);
                var spolozky = parser.GetPolozky().ToArray();

            }
        }
    }

}