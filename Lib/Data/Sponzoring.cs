//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HlidacStatu.Lib.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Sponzoring
    {
        public int Id { get; set; }
        public Nullable<int> OsobaIdDarce { get; set; }
        public string IcoDarce { get; set; }
        public Nullable<int> OsobaIdPrijemce { get; set; }
        public string IcoPrijemce { get; set; }
        public int Typ { get; set; }
        public Nullable<decimal> Hodnota { get; set; }
        public string Popis { get; set; }
        public Nullable<System.DateTime> DarovanoDne { get; set; }
        public string Zdroj { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public Nullable<System.DateTime> Edited { get; set; }
        public string UpdatedBy { get; set; }
    }
}
