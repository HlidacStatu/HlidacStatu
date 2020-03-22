using HlidacStatu.Lib.Data;
using HlidacStatu.Lib.Data.External.DataSets;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HlidacStatu.Lib.Data.VZ;

namespace HlidacStatu.Web.Controllers
{
    public partial class ApiV1Controller : Controllers.GenericAuthController
    {
    


        public ActionResult Watchdog(string _id, string _dataid, string dataType = "VerejnaZakazka", string query = null, string expiration = null)
        {
            string id = _id;
            string dataid = _dataid;

            id = id.ToLower();
            var apiAuth = Framework.ApiAuth.IsApiAuth(this,
                parameters: new Framework.ApiCall.CallParameter[] {
                    new Framework.ApiCall.CallParameter("id", id),
                    new Framework.ApiCall.CallParameter("query", query),
                    new Framework.ApiCall.CallParameter("expiration", expiration)
                });

            if (!apiAuth.Authentificated)
            {
                //Response.StatusCode = 401;
                return Json(ApiResponseStatus.ApiUnauthorizedAccess, JsonRequestBehavior.AllowGet);
            }
            else
            {

                if (apiAuth.ApiCall.User != "michal@michalblaha.cz" 
                    && apiAuth.ApiCall.User != "maixner@solidis.cz"
                    )
                    return Json(ApiResponseStatus.ApiUnauthorizedAccess, JsonRequestBehavior.AllowGet);

                var wdName = WatchDogProcessor.APIID_Prefix + dataid;
                using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
                {

                    switch (id.ToLower())
                    {
                        case "add":
                            var expirDate = HlidacStatu.Util.ParseTools.ToDateTime(expiration, "yyyy-MM-ddTHH:mm:ss");
                            if (string.IsNullOrEmpty(query))
                            {
                                return Json(ApiResponseStatus.Error(-99, "No query"), JsonRequestBehavior.AllowGet);
                            }

                            var wd2 = db.WatchDogs.AsNoTracking().Where(m => m.Name == wdName).FirstOrDefault();
                            if (wd2 != null)
                            {
                                wd2.SearchTerm = query;
                                wd2.Expires = expirDate;
                                wd2.Save();
                            }
                            else
                            {

                                var dt = dataType;

                                HlidacStatu.Lib.Data.WatchDog wd = new HlidacStatu.Lib.Data.WatchDog();
                                wd.Created = DateTime.Now;
                                wd.UserId = apiAuth.ApiCall.UserId;
                                wd.StatusId = 1;
                                wd.SearchTerm = query;
                                wd.PeriodId = 2; //daily
                                wd.FocusId = 0;
                                wd.Name = wdName;
                                wd.Expires = expirDate;
                                wd.SpecificContact = "HTTPPOSTBACK";
                                if (dt.ToLower() == typeof(Smlouva).Name.ToLower())
                                    wd.dataType = typeof(Smlouva).Name;
                                else if (dt.ToLower() == typeof(VerejnaZakazka).Name.ToLower())
                                    wd.dataType = typeof(VerejnaZakazka).Name;
                                else if (dt.ToLower().StartsWith(typeof(HlidacStatu.Lib.Data.External.DataSets.DataSet).Name.ToLower()))
                                {
                                    var dataSetId = dt.Replace("DataSet.", "");
                                    if (HlidacStatu.Lib.Data.External.DataSets.DataSet.ExistsDataset(dataSetId) == false)
                                    {
                                        HlidacStatu.Util.Consts.Logger.Error("AddWd - try to hack, wrong dataType = " + dataType + "." + dataSetId);
                                        throw new ArgumentOutOfRangeException("AddWd - try to hack, wrong dataType = " + dataType + "." + dataSetId);
                                    }
                                    wd.dataType = typeof(HlidacStatu.Lib.Data.External.DataSets.DataSet).Name + "." + dataSetId;
                                }
                                else if (dt == WatchDog.AllDbDataType)
                                {
                                    wd.dataType = dt;
                                }
                                else
                                {
                                    HlidacStatu.Util.Consts.Logger.Error("AddWd - try to hack, wrong dataType = " + dataType);
                                    throw new ArgumentOutOfRangeException("AddWd - try to hack, wrong dataType = " + dataType);
                                }

                                wd.Save();
                            }
                            break;
                        case "delete":
                        case "disable":
                        case "get":
                        case "enable":
                            var wd1 = db.WatchDogs.AsNoTracking().Where(m => m.Name == wdName).FirstOrDefault();
                            if (wd1==null)
                                return Json(ApiResponseStatus.Error(-404,"Watchdog not found"), JsonRequestBehavior.AllowGet);

                            if (id == "delete")
                            {
                                wd1.Delete();
                                return Json(new { Ok = true }, JsonRequestBehavior.AllowGet);
                            }
                            if (id == "disable")
                                wd1.StatusId = 0;
                            if (id == "delete")
                                wd1.StatusId = 1;
                            if (id == "get")
                                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { id = wd1.Name.Replace("APIID:", ""), expiration = wd1.Expires, query = wd1.SearchTerm }), "text/json");

                            wd1.Save();
                            break;
                        default:
                            break;
                    }

                }


                return Json(new { Ok = true }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}


