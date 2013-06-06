using PublicWebSms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicWebSms.Controllers
{
    [PwsAuthorize]
    public class ContactController : Controller
    {
        private PwsDbContext db = new PwsDbContext();
        //
        // GET: /Contact/

        public ActionResult Index(int groupId = -1, bool groupEdit = false)
        {
            string loggedUserName = UserSession.GetLoggedUserName();
            List <Contact> dataContact = null;

            //var dataContact = (from contactUser in db.ContactUser where contactUser.UserName == loggedUserName select smsUser.SMS).ToList();
            
            if (groupId == -1)
            {
                dataContact = (from contact in db.ContactUser where contact.UserName == loggedUserName select contact.Contact).ToList();
            }
            else
            {
                dataContact = (from groupContact in db.GroupsContact join groupUser in  db.GroupUser on groupContact.GroupId equals groupUser.GroupId where groupUser.UserName == loggedUserName && groupUser.GroupId == groupId select groupContact.Contact).ToList();
                ViewBag.Group = db.Groups.SingleOrDefault(x => x.GroupId == groupId);
            }

            if (groupEdit)
            {
                ViewBag.PartialView = true;
                return PartialView(dataContact);
            }
            ViewBag.PartialView = false;
            return View(dataContact);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Contact newContact)
        {
            bool sukses = false;

            if (ModelState.IsValid) 
            {
                db.Contacts.Add(newContact);
                db.SaveChanges();

                ContactUser contactUser = new ContactUser { 
                    ContactId = newContact.ContactId,
                    UserName = UserSession.GetLoggedUserName()
                };

                db.ContactUser.Add(contactUser);
                db.SaveChanges();

                sukses = true;

                if (!Request.IsAjaxRequest())
                {
                    return Redirect("~/Contact/Create?sukses=1");
                }

            }

            if (Request.IsAjaxRequest())
            {
                return Json(sukses);
            }

            return View();
        }

        public ActionResult CreateGroup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateGroup(Group newGroup)
        {
            bool sukses = false;

            if (ModelState.IsValid)
            {
                db.Groups.Add(newGroup);
                db.SaveChanges();

                GroupUser groupGroup = new GroupUser
                {
                    GroupId = newGroup.GroupId,
                    UserName = UserSession.GetLoggedUserName()
                };

                db.GroupUser.Add(groupGroup);
                db.SaveChanges();

                sukses = true;

                if (!Request.IsAjaxRequest())
                {
                    return Redirect("~/Contact/CreateGroup?sukses=1");
                }
            }
            return View();
        }

        public ActionResult ShowContact(int idKontak = -1)
        {
            return Redirect("~/Message/Compose");
            //return View();
        }

        public ActionResult ShowGroup(int idGroup = -1)
        {
            return Redirect("~/Message/Compose");
            //return View();
        }

       
    }
}
