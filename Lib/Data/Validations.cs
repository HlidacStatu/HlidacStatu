using System;
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
        public DateTime? Narozeni;
        
    }

    public class OsobaEventMetadata
    {

        [Required(ErrorMessage ="Prosím, vyplňte titulek.")]
        [Display(Name = "Titulek")]
        
        public string Title { get; set; }

        [Display(Name = "Popis")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Prosím, vyberte typ události.")]
        public int Type { get; set; }

        [Display(Name = "Datum od")]
        public DateTime? DatumOd { get; set; }

        [Display(Name = "Datum do")]
        public DateTime? DatumDo { get; set; }

        [Display(Name = "Další informace")]
        public string AddInfo { get; set; }

        [Display(Name = "Částka")]
        public decimal? AddInfoNum { get; set; }

        [Display(Name = "Zdroj informace")]
        public string Zdroj { get; set; }

    }
}
