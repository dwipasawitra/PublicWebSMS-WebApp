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

        public ActionResult AddContactToGroup(Contact contact, Group group) 
        {
            GroupContact groupContact = new GroupContact
            {
                GroupId = group.GroupId,
                ContactId = contact.ContactId
            };

            db.GroupsContact.Add(groupContact);
            db.SaveChanges();

            return Redirect("~/Contact");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Contact newContact, string groupName = "")
        {
            string loggedUserName = UserSession.GetLoggedUserName();
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

                //Group group = from dataGroup in db.Groups where dataGroup.GroupName == groupName select dataGroup
                //GroupUser groupUser = (GroupUser)from dataGroup in db.GroupUser where dataGroup.Group.GroupName == groupName && dataGroup.UserName == UserSession.GetLoggedUserName() select dataGroup;
                GroupUser groupUser = db.GroupUser.SingleOrDefault(dataGroup => dataGroup.Group.GroupName == groupName && dataGroup.UserName == loggedUserName);

                if (groupUser == null)
                {
                    Group newGroup = new Group { GroupName = groupName };
                    this.CreateGroup(newGroup);
                    this.AddContactToGroup(newContact, newGroup);
                }
                else 
                {
                    this.AddContactToGroup(newContact, groupUser.Group);
                }

                sukses = true;

                if (!Request.IsAjaxRequest())
                {
                    return Redirect("~/Contact?sukses=1");
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
                    return Redirect("~/Contact/ShowGroup?sukses=1");
                }
                else
                {
                    return Json(sukses);
                }
            }
            else 
            {
                if (!Request.IsAjaxRequest())
                {
                    return Redirect("~/Contact/ShowGroup?sukses=0");
                }
                else
                {
                    return Json(sukses);
                }
            }


            return View();
        }

        public ActionResult ShowContact(int contactId = -1)
        {
            return Redirect("~/Message/Compose");
            //return View();
        }

        public ActionResult ShowGroup(int groupId = -1)
        {
            string loggedUserName = UserSession.GetLoggedUserName();
            
            List<Group> dataGroup = null;

            if (groupId > 0)
            {
                dataGroup = (from groupUser in db.GroupUser where groupUser.Group.GroupId == groupId && groupUser.UserName == loggedUserName select groupUser.Group).ToList();
            }
            else
            {
                dataGroup = (from groupUser in db.GroupUser where groupUser.UserName == loggedUserName select groupUser.Group).ToList();
            }
            
            ViewBag.GroupId = groupId;
            //return Redirect("~/Message/Compose");
            return View(dataGroup);
        }

       
    }
}
