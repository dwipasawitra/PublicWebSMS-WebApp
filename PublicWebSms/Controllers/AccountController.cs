using PublicWebSms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicWebSms.Controllers
{
    
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        private PwsDbContext db = new PwsDbContext();

        public ActionResult Index()
        {
            return Redirect("~/Account/Login");
        }

        public ActionResult Login(string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            // Jika sudah login, tidak usah menampilkan halaman login. Langsung dashboard!
            if (UserSession.IsLogin())
            {
                return Redirect("~/Dashboard");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginData loginData, string returnUrl = "")
        {
            bool berhasil = false;

            if (ModelState.IsValid)
            {
                if (UserSession.DoLogin(loginData.LoginName, loginData.Password))
                {
                    berhasil = true;
                    if (!Request.IsAjaxRequest())
                    {
                        if(returnUrl != "")
                        {
                            return Redirect("~" + returnUrl);
                        }

                        return Redirect("~/Dashboard");
                    }
                }

            }

            if (Request.IsAjaxRequest())
            {
                return Json(berhasil);
            }
            return View();
        }

        [PwsAuthorize]
        public ActionResult Logout()
        {
            // Lakukan proses logout
            UserSession.DoLogout();
            return Redirect("~/Account/Login");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterData registerData)
        {
            // AJAX Request: Berikan aba-aba jika pengisian formulir sudah benar
            // HTTP Request: Bawa ke alamat RegisterSucccess jika pengisian sudah benar
            
            bool berhasil = false;
            
            if (ModelState.IsValid && registerData.AcceptTerm == true)
            {
                User newUser = new User { LoginName = registerData.Email, LoginPassword = registerData.Password };
                db.Users.Add(newUser);
                db.SaveChanges();

                berhasil = true;

                if (!Request.IsAjaxRequest())
                {
                    return Redirect("~/Account/RegisterSuccess");
                }
            }


            if (Request.IsAjaxRequest())
            {
                return Json(berhasil);
            }

            return View();
        }

        public ActionResult RegisterSuccess()
        {
            return View();
        }

        [PwsAuthorize]
        public ActionResult Manage()
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
