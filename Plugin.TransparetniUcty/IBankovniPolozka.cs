using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Plugin.TransparetniUcty
{

    public interface IBankovniPolozka
    {
        string Id { get; set; }
        string CisloUctu { get; set; }
        DateTime Datum { get; set; }
        string PopisTransakce { get; set; }
        string NazevProtiuctu { get; set; }
        string CisloProtiuctu { get; set; }
        string ZpravaProPrijemce { get; set; }
        string VS { get; set; }
        string KS { get; set; }
        string SS { get; set; }
        decimal Castka { get; set; }
        string AddId { get; set; }
        string ZdrojUrl { get; set; }
    }
}
