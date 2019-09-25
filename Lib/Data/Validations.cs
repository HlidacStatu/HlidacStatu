using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HlidacStatu.Lib.Data
{
    // rozšíření Osoby o validace
    public class OsobaMetadata
    {
        [Range(0,3)]
        [Required]
        [Display(Name = "Stav")]
        public int Status;

        [Required]
        [Display(Name = "Jméno")]
        public string Jmeno;

        [Required]
        [Display(Name = "Příjmení")]
        public string Prijmeni;

        [Display(Name = "Titul před")]
        public string TitulPred;

        [Display(Name = "Titul po")]
        public string TitulPo;

        [Display(Name = "Narození")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime? Narozeni;
        
    }

    public class OsobaEventMetadata
    {

        [Required(ErrorMessage ="Prosím, vyplňte titulek.")]
        [Display(Name = "Titulek")]
        public string Title { get; set; }

        [Display(Name = "Popis")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Prosím, vyberte typ události.")]
        [Display(Name = "Typ")]
        public int Type { get; set; }

        [Display(Name = "Podtyp")]
        public int SubType { get; set; }

        [Display(Name = "Datum od")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime? DatumOd { get; set; }

        [Display(Name = "Datum do")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime? DatumDo { get; set; }

        [Display(Name = "Další informace")]
        public string AddInfo { get; set; }

        [Display(Name = "Politická strana")]
        public string PolitickaStrana { get; set; }

        [Display(Name = "Částka")]
        public decimal? AddInfoNum { get; set; }

        [Display(Name = "Zdroj informace")]
        public string Zdroj { get; set; }


        

    }
}
