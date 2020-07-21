using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HlidacStatu.Web.Framework
{
    public class DoubleForm
    {
        public string PrimaryAction { get; set; } = "/hledat";
        public string PrimaryActionLabel { get; set; } = "Hledat všude";
        public string SecondaryAction { get; set; } = null;
        public string SecondaryActionLabel { get; set; } = null;
    }
}