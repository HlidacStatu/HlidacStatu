using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using static HlidacStatu.Lib.RenderTools;
using HlidacStatu.Lib.Data.External.DataSets;

namespace HlidacStatu.Web.Models
{


    public class DataDetailModel
    {
        public DataSet Dataset { get; set; }
        public string Data { get; set; }
    }


}