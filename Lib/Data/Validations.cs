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
}
