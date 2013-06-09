using PublicWebSms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PublicWebSms.Controllers
{
    public class MessageProcess
    {
        private PwsDbContext db = new PwsDbContext();

        public bool SaveDraft(Controller controller, SMS smsInput)
        {
            Draft draft = new Draft
            {
                Content = smsInput.Content,
                DestinationNumber = smsInput.DestinationNumber,
                Scheduled = smsInput.Scheduled
            };

            if (draft.Scheduled) draft.ScheduleTime = smsInput.ScheduleTime;
            else draft.ScheduleTime = DateTime.Now;

            db.Drafts.Add(draft);
            db.SaveChanges();

            DraftUser draftUser = new DraftUser
            {
                UserName = UserSession.GetLoggedUserName(),
                DraftId = draft.DraftId
            };

            db.DraftUser.Add(draftUser);
            db.SaveChanges();

            return true;
        }

        public bool SendDraft(Controller controller, int draftId = -1)
        {
            string loggedUserName = UserSession.GetLoggedUserName();
            SMS smsData = null;

            DraftUser draftUser = db.DraftUser.SingleOrDefault(x => x.DraftId == draftId &&
                x.UserName == loggedUserName);
            
            smsData = new SMS
            {
                Content = draftUser.Draft.Content,
                DestinationNumber = draftUser.Draft.DestinationNumber,
                ScheduleTime = draftUser.Draft.ScheduleTime,
                Scheduled = draftUser.Draft.Scheduled,
            };

            db.Drafts.Remove(draftUser.Draft);
            db.DraftUser.Remove(draftUser);

            return Send(controller, smsData);
        }

        public bool Send(Controller controller, SMS smsInput)
        {
            if (controller.ModelState.IsValid)
            {
                db.SMSes.Add(smsInput);
                db.SaveChanges();

                SMSUser smsUser = new SMSUser
                {
                    UserName = UserSession.GetLoggedUserName(),
                    SMSId = smsInput.SmsId
                };

                db.SMSUser.Add(smsUser);
                db.SaveChanges();

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}