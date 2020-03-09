﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace HlidacStatu.Web.Controllers
{
    public class ApiV2Controller : GenericAuthController
    {
        [HttpGet]
        [Route("/api/v2.0.0/smlouvy/detail/{id}")]
        [SwaggerOperation("SmlouvyDetailIdGet")]
        [SwaggerResponse(statusCode: 200, type: typeof(Smlouva), description: "Úspěšně vrácena smlouva")]
        [SwaggerResponse(statusCode: 400, type: typeof(Chyba), description: "Některé z předaných parametrů byly zadané nesprávně")]
        [SwaggerResponse(statusCode: 401, type: typeof(Chyba), description: "Nesprávný autorizační token")]
        [SwaggerResponse(statusCode: 404, type: typeof(Chyba), description: "Požadovaný dokument nebyl nalezen")]
        [SwaggerResponse(statusCode: 500, type: typeof(Chyba), description: "Došlo k interní chybě na serveru")]
        public ActionResult SmlouvyDetailIdGet([FromRoute][Required]string id)
        {
        }

    }
}
