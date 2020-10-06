using HlidacStatu.Web.Attributes;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using HlidacStatu.Lib.Analysis.KorupcniRiziko;
using HlidacStatu.Web.Models.Apiv2;
using System.Linq;

namespace HlidacStatu.Web.Controllers
{
    [RoutePrefix("api/v2/kindex")]
    public class ApiV2KindexController : ApiV2AuthController
    {

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet, Route()]
        public IEnumerable<SubjectNameCache> AllSubjects()
        {
            return SubjectNameCache.GetCompanies().Values;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet, Route("full/{ico}")]
        public KIndexData FullDetail(string ico)
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

            return kindex;
            
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet, Route("{ico}")]
        public KIndexDTO Detail(string ico)
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

            var limitedKindex = new KIndexDTO
            {
                Ico = kindex.Ico,
                Name = kindex.Jmeno,
                LastChange = kindex.LastSaved,
                AnnualCalculations = kindex.roky.Select(annual => new KIndexYearsDTO { 
                    KIndex = annual.KIndex,
                    Calculation = annual.KIndexVypocet
                })
            };

            return limitedKindex;
        }

    }
}