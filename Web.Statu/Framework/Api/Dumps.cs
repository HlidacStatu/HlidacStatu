using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static HlidacStatu.Web.Models.ApiV1Models;

namespace HlidacStatu.Web.Framework.Api
{
    public class Dumps
    {


        public static Models.ApiV1Models.DumpInfoModel[] GetDumps()
        {

            string baseUrl = "https://www.hlidacstatu.cz/api/v1/";
            List<DumpInfoModel> data = new List<DumpInfoModel>();
            
            foreach (var fi in new System.IO.DirectoryInfo(Lib.StaticData.Dumps_Path).GetFiles("*.zip"))
            {
                var fn = fi.Name;
                var regexStr = @"((?<type>(\w*))? \.)? (?<name>(\w|-)*)\.dump -? (?<date>\d{4} - \d{2} - \d{2})?.zip";
                DateTime? date = Devmasters.DT.Util.ToDateTimeFromCode(Util.ParseTools.GetRegexGroupValue(fn, regexStr, "date"));
                string name = Util.ParseTools.GetRegexGroupValue(fn, regexStr, "name");
                string dtype = Util.ParseTools.GetRegexGroupValue(fn, regexStr, "type");
                if (!string.IsNullOrEmpty(dtype))
                    name = dtype + "." + name;
                data.Add(
                    new DumpInfoModel()
                    {
                        url = baseUrl + $"dump?datatype={name}&date={date?.ToString("yyyy-MM-dd") ?? ""}",
                        created = fi.LastWriteTimeUtc,
                        date = date,
                        fulldump = date.HasValue == false,
                        size = fi.Length,
                        dataType = name
                    }
                    ); ;
            }



            return data.ToArray();
        }


    }
}