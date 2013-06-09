using PublicWebSms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicWebSms.Controllers
{
    [PwsAuthorize]
    public class DashboardController : MyController
    {
        //
        // GET: /Dashboard/
        private PwsDbContext db = new PwsDbContext();

        public ActionResult Index()
        {
            // Jika Admin, maka lempar halaman aprovasi pengguna
            if (UserSession.IsAdmin())
            {
                return RedirectToAction("UserAprove", "Admin");
            }
            
            string loggedUser = UserSession.GetLoggedUserName();

            // Get data required for Dashboard
            ViewBag.LastLogin = db.Users.SingleOrDefault(x => x.LoginName == loggedUser).LastLogin;
            ViewBag.OutboxCount = db.SMSUser.Count(x => x.UserName == loggedUser && x.SMS.Draft == false);
            ViewBag.DraftCount = db.SMSUser.Count(x => x.UserName == loggedUser && x.SMS.Draft == true);
            ViewBag.SmsTotal = ViewBag.OutboxCount + ViewBag.DraftCount;
            ViewBag.ContactCount = db.ContactUser.Count(x => x.UserName == loggedUser);
            ViewBag.GroupCount = db.GroupUser.Count(x => x.UserName == loggedUser);


            return View();
        }

    }
}
