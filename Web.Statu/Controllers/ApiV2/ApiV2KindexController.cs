using HlidacStatu.Web.Attributes;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using HlidacStatu.Lib.Analysis.KorupcniRiziko;
using HlidacStatu.Web.Models.Apiv2;

namespace HlidacStatu.Web.Controllers
{
    [RoutePrefix("api/v2/kindex")]
    public class ApiV2KindexController : ApiV2AuthController
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [AuthorizeAndAudit(Roles = "Admin")]
        [HttpGet, Route("{ico}")]
        public KIndexData Detail(string ico)
        {
            if (string.IsNullOrEmpty(ico))
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.BadRequest, $"Hodnota ico chybí."));
            }

            var kindex = KIndex.Get(ico);
            
            if (kindex == null)
            {
                throw new HttpResponseException(new ErrorMessage(System.Net.HttpStatusCode.NotFound, $"Kindex pro ico [{ico}] nenalezen."));
            }

            //if(this.ApiAuth.ApiCall.UserRoles.Contains("Admin"))
            //{
            //    return kindex;
            //}

            List<KIndexData.Annual> limitedYears = new List<KIndexData.Annual>();
            foreach(var year in kindex.roky)
            {
                var limitedYear = KIndexData.Annual.Empty(year.Rok);
                limitedYear.KIndex = year.KIndex;
                limitedYear.KIndexVypocet = year.KIndexVypocet;
                limitedYears.Add(limitedYear);
            }
            var limitedKindex = new KIndexData
            {
                Ico = kindex.Ico,
                Jmeno = kindex.Jmeno,
                LastSaved = kindex.LastSaved,
                roky = limitedYears
            };

            return limitedKindex;
        }
    }
}