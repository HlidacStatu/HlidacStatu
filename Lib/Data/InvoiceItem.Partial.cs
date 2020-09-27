using Devmasters;
using Devmasters.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{
    public partial class InvoiceItems
    {
        [ShowNiceDisplayName()]
        public enum ShopItem
        {
            [NiceDisplayName("Služby pro nevládní organizace")]
            NGO = 1,
            [NiceDisplayName("Základní služby")]
            Zakladni = 2,
            [NiceDisplayName("Kompletní služby")]
            Kompletni = 3,
        }
        public InvoiceItems()
        { }

        public decimal FinalPriceWithoutVat()
        {
            return (this.Price - this.Discount) * Amount;
        }

        public decimal FinalPriceWithVat()
        {
            return ((this.Price - this.Discount) * (1 + (this.VAT / 100))) * Amount;
        }
    }
}
