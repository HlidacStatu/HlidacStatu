using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HlidacStatu.Web.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [Display(Name = "Zapamatovat si mě?")]
        public bool RememberMe { get; set; } = true;
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Fungující e-mail adresa")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} musí být dlouhé alespoň {2} znaků.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        //[Display(Name = "Celé jméno")]
        //public string Fullname { get; set; }

        //[RegularExpression("\\d{8}", ErrorMessage ="Špatné IČO, napište ho bez mezer, 8 čísel dlouhé")]
        //[Display(Name = "IČO organizace, úřadu či firmy")]
        //public string OrganizationId { get; set; }

    }
    public class ObjednavkaViewModel
    {
        [Display(Name = "Jméno firmy")]
        public string JmenoFirmy { get; set; }

        [Display(Name = "Jméno objednavajici osoby")]
        public string JmenoOsoby { get; set; }

        [Display(Name = "IČ")]
        public string ICO { get; set; }

        [Display(Name = "DIČ")]
        public string DIC { get; set; }


        [Required]
        [Display(Name = "Ulice, číslo popisné a orientační")]
        public string Adresa { get; set; }


        [Required]
        [Display(Name = "Město")]
        public string Mesto { get; set; }

        [Required]
        [Display(Name = "PSČ")]
        public string PSC { get; set; }

        [Required]
        [Display(Name = "Telefon")]
        public string Tel { get; set; }


        //[Display(Name = "Celé jméno")]
        //public string Fullname { get; set; }

        //[RegularExpression("\\d{8}", ErrorMessage ="Špatné IČO, napište ho bez mezer, 8 čísel dlouhé")]
        //[Display(Name = "IČO organizace, úřadu či firmy")]
        //public string OrganizationId { get; set; }

    }
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email použitý při registraci")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} musí být dlouhé alespoň {2} znaků.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potvrzení hesla")]
        [Compare("Password", ErrorMessage = "Obě hesla musí být stejná.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }



}
