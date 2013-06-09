using PublicWebSms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicWebSms.Controllers
{
    public class ContactProcess
    {
        private PwsDbContext db = new PwsDbContext();
        public int SaveContact(Controller controller, Contact contact)
        {
            db.Contacts.Add(contact);
            db.SaveChanges();

            ContactUser contactUser = new ContactUser
            {
                ContactId = contact.ContactId,
                UserName = UserSession.GetLoggedUserName()
            };

            db.ContactUser.Add(contactUser);
            db.SaveChanges();

            return contact.ContactId;
        }

        public GroupUser IsGroupExist(Controller controller, string userName, string groupName)
        {
            GroupUser groupUser = db.GroupUser.SingleOrDefault(
                dataGroup => dataGroup.Group.GroupName == groupName
                            && dataGroup.UserName == userName
            );

            if (groupUser != null) return groupUser;
            return null;
        }

        public GroupContact AddContactToGroup(Controller controller, Contact contact, Group group)
        {
            GroupContact groupContact = new GroupContact
            {
                GroupId = group.GroupId,
                ContactId = contact.ContactId
            };

            db.GroupsContact.Add(groupContact);
            db.SaveChanges();

            return groupContact;
        }

        public GroupContact AddContactToGroup(Controller controller, int contactId, int groupId)
        {
            GroupContact groupContact = new GroupContact();
            return groupContact;
        }

        public int SaveGroup(Controller controller, string userName, Group group)
        {
            if (controller.ModelState.IsValid)
            {
                db.Groups.Add(group);
                db.SaveChanges();

                GroupUser groupUser = new GroupUser { 
                    GroupId = group.GroupId,
                    UserName = userName
                };

                db.GroupUser.Add(groupUser);
                db.SaveChanges();

                return group.GroupId;
            }
            
            
            return 0;
        }

        public Group GetGroup(Controller controller, string userName, int groupId)
        {
            Group group = null;
            GroupUser groupUser = null;

            groupUser = db.GroupUser.SingleOrDefault(x => x.GroupId == groupId && x.UserName == userName);
            group = groupUser.Group;

            return group;
        }

        public List<Group> GetListGroup(Controller controller, string userName, int contactId)
        {
            List<Group> listGroup = null;

            listGroup = (
                from groupUser in db.GroupUser
                where groupUser.UserName == userName
                join groupContact in db.GroupsContact on groupUser.GroupId
                equals groupContact.GroupId
                select groupUser.Group
            ).ToList();

            return listGroup;
        }

        public List<Contact> GetListContact(Controller controller, string userName)
        {
            List <Contact> listContact = null;
            
            listContact = (
                from contact in db.ContactUser 
                where contact.UserName == userName 
                select contact.Contact
            ).ToList();

            return listContact;
        }

        public List<Contact> GetListContact(Controller controller, string userName, int groupId)
        {
            List<Contact> listContact = null;

            listContact = (
                from groupContact in db.GroupsContact 
                join groupUser in db.GroupUser on groupContact.GroupId 
                equals groupUser.GroupId
                where groupUser.UserName == userName 
                    && groupUser.GroupId == groupId 
                select groupContact.Contact
            ).ToList();

            return listContact;
        }
    }
}