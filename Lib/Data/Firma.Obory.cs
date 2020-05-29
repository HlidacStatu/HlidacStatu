using Devmasters.Core;
using HlidacStatu.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HlidacStatu.Lib.Data
{
    public partial class Firma
        : Bookmark.IBookmarkable, ISocialInfo
    {
        public class Obory
        {
            /*
             Nemocnice:
             select distinct f.ICO, f.Jmeno from firma f inner join Firma_NACE fn on f.ICO = fn.ICO
             where nace like '86%' and f.IsInRS = 1

             */
        }
    }
}
