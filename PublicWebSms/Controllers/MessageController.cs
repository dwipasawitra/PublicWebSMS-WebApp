using PublicWebSms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicWebSms.Controllers
{
    [PwsAuthorize]
    public class MessageController : Controller
    {
        private PwsDbContext db = new PwsDbContext();
        //
        // GET: /Message/

        public ActionResult Index()
        {
            return Redirect("/Message/Outbox");
        }

        public ActionResult Outbox(int success = 0)
        {
            // Halaman ini akan menapilkan daftar SMS di dalam outbox yang dimiliki oleh pengguna

            string loggedUserName = UserSession.GetLoggedUserName();

            var dataSMS = (from smsUser in db.SMSUser where smsUser.UserName == loggedUserName select smsUser.SMS).ToList();

            // AJAX Request: Tampilkan data dalam bentuk JSON
            // Web Request: Tampilkan seluruh halaman dalam bentuk tabel

            if (Request.IsAjaxRequest())
            {
                return Json(dataSMS);
            }

            ViewBag.Success = success;
            return View(dataSMS);
        }

        public ActionResult Compose(int id = -1, string nomorTujuan = "")
        {
            ViewBag.NomorTujuan = nomorTujuan;
            if (id != -1)
            {
                var smsData = db.SMSes.SingleOrDefault(x => x.SmsId == id);
                ViewBag.NomorTujuan = smsData.DestinationNumber;
                ViewBag.Pesan = smsData.Content;
                if (smsData.Scheduled)
                {
                    ViewBag.IsScheduled = "checked";
                }
                else
                {
                    ViewBag.IsScheduled = "";
                }
                
                ViewBag.ScheduleTime = smsData.ScheduleTime;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Send(SMS smsInput)
        {
            bool sukses = false;
            smsInput.TimeStamp = DateTime.Now;
            smsInput.Sent = false;

            // Dikomentari sementara, nanti masalah nilai bawaan harus diatur di model, bukan di kode ini (seperti tiga baris diatas itu)
            if (ModelState.IsValid)
            {
                db.SMSes.Add(smsInput);
                db.SaveChanges();

                SMSUser smsUser = new SMSUser { UserName = UserSession.GetLoggedUserName(), SMSId = smsInput.SmsId };

                db.SMSUser.Add(smsUser);
                db.SaveChanges();

                sukses = true;

                if (!Request.IsAjaxRequest())
                {
                    return Redirect("/Message/Outbox?sukses=1");
                }
               
            }

            if (Request.IsAjaxRequest())
            {
                return Json(sukses);
            }

            return View();
        }

        [HttpPost]
        public ActionResult SaveDraft(SMS smsInput)
        {
            return View();
        }

        public ActionResult ScheduleSMS(SMS smsInput)
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult SMSAPI(SMSAPIInput input)
        {
            // API Secret Code Checking
            // HARDCODED!

            if (input.APIId == "hanahbanana" && input.APISecretCode == "segogoreng")
            {
                // Ambil data SMS yang siap kirim
                // SMS yang siap kirim sementara, atau yang terjadwal saat ini selisih 5 menit (untuk jaga-jaga gituch)
                DateTime maxTime = DateTime.Now.AddMinutes(5);
                DateTime minTime = DateTime.Now.AddMinutes(-5);

                var dataSMS = (from sms in db.SMSes where (sms.Sent == false) || (sms.Sent == false && (sms.Scheduled == true && sms.ScheduleTime <= maxTime && sms.ScheduleTime >= minTime)) select sms);

                // Untuk tiap SMS, tandai sms.Sent menjadi true dan kirimkan dalam bentuk JSON
                foreach (SMS sms in dataSMS)
                {
                    sms.Sent = true;
                }

                return Json(dataSMS.ToList());
            }

            return View();
        }



    }
}
