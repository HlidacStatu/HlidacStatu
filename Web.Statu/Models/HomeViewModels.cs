using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using static HlidacStatu.Lib.RenderTools;

namespace HlidacStatu.Web.Models
{


    public class OsobySponzoriModel
    {
        public string strana { get; set; }
        public int pocet { get; set; }
        public decimal sum { get; set; }
    }
    public class SubjektReportModel
    {
        public HlidacStatu.Lib.Data.Firma firma { get; set; }
        public HlidacStatu.Lib.Render.ReportModel reports { get; set; }
        public string ICO { get; set; }

    }



    public class NewPersonModel
    {

        public int InternalId { get; set; }
        public string TitulPred { get; set; }

        [Required, MaxLength(150)]
        public string Jmeno { get; set; }
        [Required, MaxLength(150)]
        public string Prijmeni { get; set; }
        public string TitulPo { get; set; }
        [Required, MaxLength(1)]
        public string Pohlavi { get; set; }

        [Required]
        public System.DateTime Narozeni { get; set; }

        [Required]
        [EnumDataType(typeof(HlidacStatu.Lib.Data.Osoba.StatusOsobyEnum))]
        public string Status { get; set; }


        public Event[] events { get; set; }
        public Vazba[] vazby { get; set; }

        public class Event
        {
            public int OsobaId { get; set; }
            [Required()]
            public string Title { get; set; }
            public string Description { get; set; }

            public string Type { get; set; }
            public DateTime? DatumOd { get; set; }
            public DateTime? DatumDo { get; set; }

            public string PolitickaStrana { get; set; }

        }

        public class Vazba
        {
            public int pk { get; set; }

            public string VazbakICO { get; set; }

            [Required, EnumDataType(typeof(HlidacStatu.Lib.Data.Relation.RelationEnum))]
            public string TypVazby { get; set; }
            public string PojmenovaniVazby { get; set; }
            public Nullable<decimal> podil { get; set; }
            public string Zdroj { get; set; }
            public System.DateTime LastUpdate { get; set; }
            public Nullable<System.DateTime> DatumOd { get; set; }
            public Nullable<System.DateTime> DatumDo { get; set; }
            public Nullable<int> VazbakOsobaId { get; set; }
            public Nullable<int> RucniZapis { get; set; }
            public string Poznamka { get; set; }

        }
    }

    public class PridatSeModel
    {
        [Phone(ErrorMessage = "Uveďte platný telefon"), Display(Name = "Telefon", Prompt = "Telefonni čislo")]
        public string Phone { get; set; }
        [EmailAddress(ErrorMessage ="Uveďte platný eamil"), Required(ErrorMessage = "Uveďte platný email"), Display(Name ="Email", Prompt = "Platný email")]
        public string Email { get; set; }
        [DisplayName("Jméno")]
        public string Jmeno { get; set; }

        [Display(Name ="Příjmení")]
        public string Prijmeni { get; set; }

        [DisplayName("V jakých oblastech se chcete zapojit?")]
        public string[] TypPrace { get; set; } = new string[] { };


        [Display(Name = "Vzkaz")]
        public string Vzkaz { get; set; }
    }



    public class UctySearchResult
    {

        public HlidacStatu.Lib.Data.TransparentniUcty.BankovniUcet BU {get;set;} = null;
        public HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka[] Polozky { get; set; } = new HlidacStatu.Lib.Data.TransparentniUcty.BankovniPolozka[] { };

        public string Query { get; set; }
        public string InternalQuery { get; set; } = null;

    }


}