using PublicWebSms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;


namespace PublicWebSms.Controllers
{
    [PwsAuthorize]
    public class MessageController : Controller
    {
        private PwsDbContext db = new PwsDbContext();
        private MessageProcess messageProcess = new MessageProcess();
        //
        // GET: /Message/

        public ActionResult Index()
        {
            return Redirect("~/Message/Outbox");
        }

        /*
         * Outbox: menampilkan daftar SMS yang sedang mengantri dalam proses pengiriman
         */
        public ActionResult Outbox(int success = 0)
        {
            string loggedUserName = UserSession.GetLoggedUserName();

            var dataSMS = (from smsUser in db.SMSUser where smsUser.UserName == loggedUserName && smsUser.SMS.Draft == false select smsUser.SMS).ToList();

            // AJAX Request: Tampilkan data dalam bentuk JSON
            // Web Request: Tampilkan seluruh halaman dalam bentuk tabel

            if (Request.IsAjaxRequest())
            {
                return Json(dataSMS);
            }

            ViewBag.Success = success;
            return View(dataSMS);
        }

        /*
         * Draft: menampilkan daftar SMS yang disimpan
         */
        public ActionResult Draft(int success = 0)
        {
            string loggedUserName = UserSession.GetLoggedUserName();

            var dataDraft = (from draftUser in db.DraftUser where draftUser.UserName == loggedUserName select draftUser.Draft).ToList();

            if (Request.IsAjaxRequest())
            {
                return Json(dataDraft);
            }

            ViewBag.Success = success;
            return View(dataDraft);
        }

        /*
         * Compose: menampilkan borang pembuatan pesan atau pengeditan pesan Draft
         */
        public ActionResult Compose(int draftId = -1, string destinationNumber = "")
        {
            ViewBag.DraftId = draftId;
            string loggedUserName = UserSession.GetLoggedUserName();
            ViewBag.Scheduled = false;
            ViewBag.ScheduleTime = DateTime.Now;

            ViewBag.IsValid = Convert.ToBoolean(Request.QueryString["isValid"]);

            if (draftId > 0)
            {
                var draftUserData = db.DraftUser.SingleOrDefault(x => x.Draft.DraftId == draftId && x.UserName == loggedUserName);
                var draftData = draftUserData.Draft;
                ViewBag.DestinationNumber = draftData.DestinationNumber;
                ViewBag.MessageContent = draftData.Content;
                if (draftData.Scheduled)
                {
                    ViewBag.ScheduleCheck = "checked=checked";
                }
                else
                {
                    ViewBag.ScheduleCheck = "";
                }
                ViewBag.Scheduled = draftData.Scheduled;
                ViewBag.ScheduleTime = draftData.ScheduleTime;
                return View(draftData);
            }
            return View();
        }

        public ActionResult Process(SMS smsInput, int smsAction)
        {
            bool sukses = false;
            string loggedUserName = UserSession.GetLoggedUserName();

            if (smsAction == 1)
            {
                smsInput.TimeStamp = DateTime.Now;
                smsInput.Sent = false;
                smsInput.Draft = false;

                // Dikomentari sementara, nanti masalah nilai bawaan harus diatur di model, bukan di kode ini (seperti tiga baris diatas itu)
                if (ModelState.IsValid)
                {
                    sukses = messageProcess.Send(this, smsInput);

                    if (!Request.IsAjaxRequest())
                    {
                        if (sukses)
                        {
                            return Redirect("~/Message/Outbox?sukses=1");
                        }
                        else
                        {
                            return Redirect("~/Message/Outbox?sukses=0");
                        }
                    }
                    else
                    {
                        return Json(sukses);
                    }

                }
                else
                {
                    // simpan ke draft kalau belum ada di draft
                    int draftId = Convert.ToInt32(Request.Form["draftId"]);
                    int theDraftId;

                    if (draftId > 0)
                    {
                        theDraftId = draftId;
                    }
                    else
                    {
                        sukses = messageProcess.SaveDraft(this, smsInput);
                        int lastDraftId = (
                            from draftUser in db.DraftUser 
                            where draftUser.UserName == loggedUserName 
                            select draftUser.DraftId
                        ).ToList().Last();

                        theDraftId = lastDraftId;
                    }

                    if (!Request.IsAjaxRequest())
                    {
                        return Redirect("~/Message/Compose?draftId=" + theDraftId + "&isValid=false");
                    }
                    else
                    {
                        return Json(false);
                    }
                }
            }
            else
            {
                sukses = messageProcess.SaveDraft(this, smsInput);

                if (sukses)
                {
                    return Redirect("~/Message/Draft?success=1");
                }
                else
                {
                    return Redirect("~/Message/Draft?success=0");
                }
            }
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
                DateTime currentTime = DateTime.Now;

                var dataSMS = (from sms in db.SMSes where (sms.Sent == false && sms.Scheduled == false) || (sms.Sent == false && (sms.Scheduled == true && sms.ScheduleTime <= currentTime)) select sms );

                var jsonSMS = from sms in dataSMS.ToList() select new Dictionary<string, string> { { "Dest", sms.DestinationNumber.ToString() }, { "Msg", sms.Content.ToString() } };

                foreach (SMS sms in dataSMS)
                {
                    sms.Sent = true;
                }

                db.SaveChanges();

                // Untuk tiap SMS, tandai sms.Sent menjadi true dan kirimkan dalam bentuk JSON
                
                return Json(jsonSMS, JsonRequestBehavior.AllowGet);
            }

            return View();
        }
    }
}
