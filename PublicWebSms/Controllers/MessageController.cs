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
    public class MessageController : MyController
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

            var dataSMS = (
                from smsUser in db.SMSUser 
                where smsUser.UserName == loggedUserName && smsUser.SMS.Draft == false 
                select smsUser.SMS
            ).ToList();

            // AJAX Request: Tampilkan data dalam bentuk JSON
            // Web Request: Tampilkan seluruh halaman dalam bentuk tabel

            if (Request.IsAjaxRequest())
            {
                return Json(dataSMS);
            }

            ViewBag.Success = success;
            ViewBag.Error = 0;

            int error = Convert.ToInt32(Request.QueryString["error"]);
            if (error == 1)
            {
                ViewBag.Error = 1;
            }

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

        [AllowAnonymous]
        public ActionResult FreeCompose(int freeSuccess = -1, int isValid = 1)
        {
            if (!FreeSmsSession.IsLimit())
            {
                if (freeSuccess == 1)
                {
                    ViewBag.FreeSmsSent = true;
                }

                ViewBag.SmsCount = FreeSmsSession.GetCurrentSmsCount();
                ViewBag.SmsLimit = FreeSmsSession.GetSmsLimit();
                ViewBag.IsValid = isValid;
                ViewBag.FreeMode = true;
                return View("Compose");
            }

            return View("FreeSmsLimit");
        }
        /*
         * Compose: menampilkan borang pembuatan pesan atau pengeditan pesan Draft
         */

        public ActionResult Compose(int draftId = -1, string destinationNumber = "")
        {
            ViewBag.DraftId = draftId;
            string loggedUserName = UserSession.GetLoggedUserName();
            ViewBag.DestinationNumber = "";
            ViewBag.Scheduled = false;
            ViewBag.ScheduleTime = DateTime.Now;

            ViewBag.IsValid = 1;
            string isValid = Request.QueryString["isValid"];
            if (isValid == "0")
            {
                ViewBag.IsValid = 0;
            }

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

        [AllowAnonymous]
        public ActionResult ProcessFree(SMS smsInput, int smsAction)
        {

            bool success;
            if (smsAction == 2)
            {
                // Khusus untuk free SMS
                if (ModelState.IsValid)
                {
                    success = messageProcess.SendFree(this, smsInput);
                    if (success)
                    {
                        return Redirect("~/Message/FreeCompose?freeSuccess=1");
                    }
                    else
                    {
                        return Redirect("~/Message/FreeCompose?isValid=0");
                    }
                 }
                else
                {
                    return Redirect("~/Message/FreeCompose?isValid=0");
                }
                    
            }
            return View("FreeSmsLimit");

        }
        public ActionResult Process(SMS smsInput, int smsAction)
        {
            bool success = false;
            string loggedUserName = UserSession.GetLoggedUserName();

            if (smsAction == 1)
            {
                smsInput.TimeStamp = DateTime.Now;
                smsInput.Sent = false;
                smsInput.Draft = false;

                // Dikomentari sementara, nanti masalah nilai bawaan harus diatur di model, bukan di kode ini (seperti tiga baris diatas itu)
                if (ModelState.IsValid)
                {
                    success = messageProcess.Send(this, smsInput);

                    if (success)
                    {
                        // delete jika dari draft
                        int draftId = Convert.ToInt32(Request.Form["draftId"]);
                        
                        if (draftId > 0)
                        {
                            messageProcess.DeleteDraft(this, draftId);
                        }

                        if (!Request.IsAjaxRequest())
                        {
                            return Redirect("~/Message/Outbox?success=1");
                        }
                        else
                        {
                            return Json(true);
                        }
                    }
                    else
                    {
                        if (!Request.IsAjaxRequest())
                        {
                            return Redirect("~/Message/Outbox?error=1");
                        }
                        else
                        {
                            return Json(false);
                        }
                    }

                }
               
                else
                {
                    // simpan ke draft kalau belum ada di draft
                    int draftId = Convert.ToInt32(Request.Form["draftId"]);

                    if (draftId > 0)
                    {
                        Draft draft = messageProcess.ConvertToDraft(smsInput);
                        messageProcess.UpdateDraft(this, draftId, draft);
                    }
                    else
                    {
                        messageProcess.SaveDraft(this, smsInput);
                        int lastDraftId = (
                            from draftUser in db.DraftUser
                            where draftUser.UserName == loggedUserName
                            select draftUser.DraftId
                        ).ToList().Last();

                        draftId = lastDraftId;
                    }

                    if (!Request.IsAjaxRequest())
                    {
                        return Redirect("~/Message/Compose?draftId=" + draftId + "&isValid=0");
                    }
                    else
                    {
                        return Json(false);
                    }
                }
            }
            else
            {
                success = messageProcess.SaveDraft(this, smsInput);

                if (success)
                {
                    return Redirect("~/Message/Draft?success=1");
                }
                else
                {
                    return Redirect("~/Message/Draft?success=0");
                }
            }
        }

        public ActionResult SendDraft(int draftId = -1)
        {
            bool success = false;

            if (draftId > 0)
            {
                success = messageProcess.SendDraft(this, draftId);

                if (success)
                {
                    return Redirect("~/Message/Outbox?success=1");
                }
                else
                {
                    return Redirect("~/Message/Compose?draftId=" + draftId + "&isValid=0");
                }
            }

            return Redirect("~/Message/Draft");
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
