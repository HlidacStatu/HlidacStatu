using HlidacStatu.Lib.Data.External.DataSets;
using HlidacStatu.Web.Attributes;
using HlidacStatu.Web.Models.Apiv2;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace HlidacStatu.Web.Controllers
{
    public class ApiV2SmlouvyController : GenericAuthController
    {
        // /api/v2/smlouvy/detail/{id}
        [HttpGet]
        [AuthorizeAndAudit]
        [SwaggerOperation("Detail")]
        [SwaggerResponse(statusCode: 200, type: typeof(Smlouva), description: "Úspěšně vrácena smlouva")]
        [SwaggerResponse(statusCode: 400, type: typeof(ErrorMessage), description: "Některé z předaných parametrů byly zadané nesprávně")]
        [SwaggerResponse(statusCode: 401, type: typeof(ErrorMessage), description: "Nesprávný autorizační token")]
        [SwaggerResponse(statusCode: 404, description: "Požadovaný dokument nebyl nalezen")]
        [SwaggerResponse(statusCode: 500, description: "Došlo k interní chybě na serveru")]
        public ActionResult Detail(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Hodnota id chybí.").ToJson(), "application/json");
            }

            var model = HlidacStatu.Lib.Data.Smlouva.Load(id);
            if (model == null)
            {
                Response.StatusCode = 404;
                return Content(new ErrorMessage($"Smlouva nenalezena").ToJson(), "application/json");
            }
            var smodel = HlidacStatu.Lib.Data.Smlouva.ExportToJson(model,
                !string.IsNullOrWhiteSpace(Request.QueryString["nice"]),
                this.User.IsInRole("Admin")
                );

            return Content(smodel, "application/json");
            
        }

        // /api/v2/Smlouvy/hledat/?query=auto&page=1&order=0
        [HttpGet]
        [AuthorizeAndAudit]
        [SwaggerOperation("Hledat")]
        [SwaggerResponse(statusCode: 200, type: typeof(InlineResponse200), description: "Úspěšně vrácen seznam smluv")]
        [SwaggerResponse(statusCode: 400, type: typeof(ErrorMessage), description: "Některé z předaných parametrů byly zadané nesprávně")]
        [SwaggerResponse(statusCode: 401, type: typeof(ErrorMessage), description: "Nesprávný autorizační token")]
        [SwaggerResponse(statusCode: 404, description: "Žádná smlouva nenalezena")]
        [SwaggerResponse(statusCode: 500, description: "Došlo k interní chybě na serveru")]
        public ActionResult Hledat(string query, int? page, int? order)
        {
            page = page ?? 1;
            order = order ?? 0;
            Lib.Searching.SmlouvaSearchResult res = null;

            if (string.IsNullOrWhiteSpace(query))
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Hodnota query chybí.").ToJson(), "application/json");
            }

            bool? platnyzaznam = null; //1 - nic defaultne
            if (
                System.Text.RegularExpressions.Regex.IsMatch(query.ToLower(), "(^|\\s)id:")
                ||
                query.ToLower().Contains("idverze:")
                ||
                query.ToLower().Contains("idsmlouvy:")
                ||
                query.ToLower().Contains("platnyzaznam:")
                )
                platnyzaznam = null;

            res = HlidacStatu.Lib.Data.Smlouva.Search.SimpleSearch(query, page.Value,
                HlidacStatu.Lib.Data.Smlouva.Search.DefaultPageSize,
                (HlidacStatu.Lib.Data.Smlouva.Search.OrderResult)order.Value,
                platnyZaznam: platnyzaznam);


            if (res.IsValid == false)
            {
                Response.StatusCode = 400;
                return Content(new ErrorMessage($"Špatně nastavená hodnota query=[{query}]").ToJson(), "application/json");
            }
            else
            {
                var filtered = res.Result.Hits
                                .Select(m => new Newtonsoft.Json.Linq.JRaw(HlidacStatu.Lib.Data.Smlouva.ExportToJson(m.Source, false, this.User.IsInRole("Admin"))))
                                .ToArray();

                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { total = res.Total, items = filtered }, Newtonsoft.Json.Formatting.None), "application/json");
            }

        }



    }
}
