using PublicWebSms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicWebSms.Controllers
{
    [PwsAdminAuthorize]
    public class AdminController : MyController
    {

        private PwsDbContext db = new PwsDbContext();

        // UserAprove: Lakukan persetujuan terhadap pengguna apakah dia aktif atau tidak
        public ActionResult UserAprove(int showWhat = 0)
        {
            // Showwhat ===> 0: All user, 1 ==> Aproved user, 2 ==> Unaproved user

            List<User> dataUser = null;

            if (showWhat == 0)
            {
                dataUser = db.Users.ToList();
            }
            else if (showWhat == 1)
            {
                dataUser = db.Users.Where(x => x.Activate == true).ToList();
            }
            else if (showWhat == 2)
            {
                dataUser = db.Users.Where(x => x.Activate == false).ToList();
            }


            return View(dataUser);
        }

        [HttpPost]
        public ActionResult UserAprove(UserAproveInput userAproveInput)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(x => x.LoginName == userAproveInput.UserName);
                user.Activate = userAproveInput.Status;
                db.SaveChanges();

                return Json(new Dictionary<string, object> { { "success", true } });
            }

            return Json(new Dictionary<string, object> { { "success", false } });
        }

    }
}
