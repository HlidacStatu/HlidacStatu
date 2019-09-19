using HlidacStatu.Lib.Data;
using HlidacStatu.Util;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HlidacStatu.Web.Controllers
{
    [Authorize]
    public partial class ManageController : GenericAuthController
    {

        // find people
        public ActionResult FindPerson(string jmeno, string prijmeni, string narozeni )
        {
            if(string.IsNullOrWhiteSpace(jmeno) 
                && string.IsNullOrWhiteSpace(prijmeni)
                && string.IsNullOrWhiteSpace(narozeni))
            {
                return View();
            }
            DateTime? nar = ParseTools.ToDateTime(narozeni, "yyyy-MM-dd");
            var osoby = Osoba.GetAllByName(jmeno, prijmeni, nar);
            if (osoby.Count() == 0)
                osoby = Osoba.GetAllByNameAscii(jmeno, prijmeni, nar);

            return View(osoby);
        }

        public ActionResult PersonDetail(int? id)
        {
            if (id == null) return new HomeController().NotFound("/", "Pokračovat na titulní straně");

            Osoba osoba = HlidacStatu.Lib.Data.Osoba.GetByInternalId(id ?? 0);

            return View(osoba);
        }


        [Authorize(Roles = "canEditData")]
        public ActionResult CreatePerson()
        {
            return View();
        }

        // Create a new person
        [Authorize(Roles = "canEditData")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePerson([Bind(Include = "TitulPred,Jmeno,Prijmeni,TitulPo,Narozeni,Status")] Osoba osoba)
        {
            if (ModelState.IsValid)
            {
                //ještě mi chybí user
                Osoba.GetOrCreateNew(osoba.TitulPred, osoba.Jmeno, osoba.Prijmeni, osoba.TitulPo, osoba.Narozeni, (Osoba.StatusOsobyEnum)osoba.Status, this.User.Identity.Name);
                return RedirectToAction(nameof(FindPerson), // asi by šlo rovnou id...
                    new {
                        jmeno = osoba.Jmeno,
                        prijmeni = osoba.Prijmeni,
                        narozeni = osoba.Narozeni
                    });
            }

            return View(osoba);
        }

        [Authorize(Roles = "canEditData")]
        public ActionResult EditPersonNP(int? id)
        {
            if (id == null) return new HomeController().NotFound("/", "Pokračovat na titulní straně");

            Osoba osoba = HlidacStatu.Lib.Data.Osoba.GetByInternalId(id ?? 0);

            return View(osoba);
        }

        [Authorize(Roles = "canEditData")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPersonNP(int id, [Bind(Include = "InternalId,TitulPred,Jmeno,Prijmeni,TitulPo,Narozeni,Status")] Osoba osoba)
        {
            if (id != osoba.InternalId) return new HomeController().NotFound("/", "Pokračovat na titulní straně");
            if (ModelState.IsValid)
            {
                Osoba result = Osoba.Update(osoba, this.User.Identity.Name);

                return RedirectToAction(nameof(FindPerson), // asi by šlo rovnou id...
                    new
                    {
                        jmeno = osoba.Jmeno,
                        prijmeni = osoba.Prijmeni,
                        narozeni = osoba.Narozeni
                    });
            }

            return View(osoba);
        }


        // add create event



        // tohle dodělat až potom co rozšířím strukturu
        [Authorize(Roles = "canEditData")]
        public ActionResult EditPersonEvent(int? id)
        {
            if (id == null) return new HomeController().NotFound("/", "Pokračovat na titulní straně");
            OsobaEvent osobaEvent = OsobaEvent.GetById(id ?? 0);
            
            return View(osobaEvent);
        }

        [Authorize(Roles = "canEditData")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditPersonEvent(string id, FormCollection form)
        {
            return Redirect("Index");
        }

        // add create event

        [Authorize(Roles = "canEditData")]
        [HttpPost]
        public JsonResult UpdateEvent(string id, OsobaEvent osobaEvent)
        {
            if(ModelState.IsValid)
            {
                var result = OsobaEvent.Update(osobaEvent, this.User.Identity.Name);
                return Json(new { Success = true });
            }
            // https://stackoverflow.com/questions/14005773/use-asp-net-mvc-validation-with-jquery-ajax

            var errorModel =
                    from x in ModelState.Keys
                    where ModelState[x].Errors.Count > 0
                    select new
                    {
                        key = x,
                        errors = ModelState[x].Errors.
                                                      Select(y => y.ErrorMessage).
                                                      ToArray()
                    };

            Response.StatusCode = 400;
            return new JsonResult() { Data = errorModel };
        }
    }
}