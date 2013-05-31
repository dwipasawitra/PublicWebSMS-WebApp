using PublicWebSms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicWebSms.Controllers
{
    public class HomeController : Controller
    {
        // Database Instance test
        private PwsDbContext db = new PwsDbContext();
        public ActionResult Index()
        {
            string errorMessage = "Tidak ada";

            try
            {
                var data = db.SMSes.ToList();
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }


            ViewBag.ErrorMessage = errorMessage.ToString();
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            db.Dispose();
        }

    }
}
