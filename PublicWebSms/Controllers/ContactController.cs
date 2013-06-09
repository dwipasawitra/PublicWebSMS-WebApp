using PublicWebSms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicWebSms.Controllers
{
    [PwsAuthorize]
    public class ContactController : MyController
    {
        private PwsDbContext db = new PwsDbContext();
        private ContactProcess contactProcess = new ContactProcess();
        
        // Index menangani halaman daftar kontak
        public ActionResult Index(int groupId = -1, bool groupEdit = false)
        {
            string loggedUserName = UserSession.GetLoggedUserName();
            List <Contact> listContact = null;
            
            if (groupId == -1)
            {
                listContact = contactProcess.GetListContact(this, loggedUserName);
            }
            else
            {
                listContact = contactProcess.GetListContact(this, loggedUserName, groupId);
                ViewBag.Group = contactProcess.GetGroup(this, loggedUserName, groupId);
            }

            List<ContactList> listContactList = new List<ContactList>();

            foreach (Contact contact in listContact)
            {
                ContactList contactList = new ContactList();
                
                contactList.ContactId = contact.ContactId;
                contactList.Nama = contact.Nama;
                contactList.Nomor = contact.Nomor;
                contactList.ListGroup = contactProcess.GetListGroup(this, loggedUserName, contact.ContactId);

                listContactList.Add(contactList);
            }

            if (groupEdit)
            {
                ViewBag.PartialView = true;
                return PartialView(listContactList);
            }

            ViewBag.PartialView = false;
            return View(listContactList);
        }

        public ActionResult AddContactToGroup(Contact contact, Group group) 
        {
            GroupContact groupContact = contactProcess.AddContactToGroup(this, contact, group);

            return Redirect("~/Contact/ShowGroup?groupId=" + group.GroupId);
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
                int contactId = contactProcess.SaveContact(this, newContact);
                /* masih error
                if (groupName != "")
                {
                    
                    Group group = contactProcess.IsGroupExist
                    if (contactProcess.IsGroupExist(this, loggedUserName, groupName))
                    {
                        
                        contactProcess.AddContactToGroup(this, groupUser.Group);
                    }
                    else
                    {
                        Group newGroup = new Group { GroupName = groupName };
                        contactProcess.SaveGroup(this, loggedUserName, newGroup);
                        contactProcess.AddContactToGroup(this, newContact, newGroup);
                    }
                }
                 */

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
