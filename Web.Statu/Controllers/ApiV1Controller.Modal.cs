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
        

        public ActionResult ModalClassification(string _id, string modalId)
        {
            string id = _id;

            return View(new Tuple<string,string>(id,modalId));
        }


        public ActionResult ModalZneplatnenaSmlouva(string _id, string modalId)
        {
            string id = _id;

            return View(new Tuple<string, string>(id, modalId));
        }

    }
}


