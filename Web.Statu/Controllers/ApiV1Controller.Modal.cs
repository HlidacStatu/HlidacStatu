using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace HlidacStatu.Web.Controllers
{
    public partial class ApiV1Controller : Controllers.GenericAuthController
    {
        

        public ActionResult ModalClassification(string id, string modalId)
        {
            return View(new Tuple<string,string>(id,modalId));
        }


    }
}


