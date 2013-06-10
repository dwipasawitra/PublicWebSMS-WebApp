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
        public int SaveContact(Controller controller, string userName, Contact contact)
        {
            ContactUser contactUser = IsContactExist(controller, userName, contact.ContactId);

            if (contactUser == null)
            {
                db.Contacts.Add(contact);
                db.SaveChanges();

                contactUser = new ContactUser
                {
                    ContactId = contact.ContactId,
                    UserName = UserSession.GetLoggedUserName()
                };

                db.ContactUser.Add(contactUser);
                db.SaveChanges();
            }
            else
            {
                db.SaveChanges();
            }

            return contact.ContactId;
        }

        public GroupUser IsGroupExist(Controller controller, string userName, string groupName)
        {
            List<GroupUser> listGroupUser = (
                from groupUsers in db.GroupUser
                where
                    groupUsers.UserName == userName && groupUsers.Group.GroupName == groupName
                select groupUsers
            ).ToList();
            
            GroupUser groupUser = null;

            if (listGroupUser.Count() > 0)
            {
                groupUser = listGroupUser.First();
            }
            

            return groupUser;
        }

        public ContactUser IsContactExist(Controller controller, string userName, int contactId)
        {
            List<ContactUser> listGroupUser = (
                from contactUsers in db.ContactUser
                where
                    contactUsers.UserName == userName && contactUsers.ContactId == contactId
                select contactUsers
            ).ToList();

            ContactUser contactUser = null;

            if (listGroupUser.Count() > 0)
            {
                contactUser = listGroupUser.First();
            }


            return contactUser;
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

            groupUser = (
                from groupUsers in db.GroupUser
                where
                    groupUsers.GroupId == groupId && groupUsers.UserName == userName
                select groupUsers
            ).ToList().First();

            group = groupUser.Group;

            return group;
        }

        public Contact GetContact(Controller controller, string userName, int contactId)
        {
            Contact contact = null;
            ContactUser contactUser = null;

            contactUser = (
                from contactUsers in db.ContactUser
                where
                    contactUsers.ContactId == contactId && contactUsers.UserName == userName
                select contactUsers
            ).ToList().First();

            contact = contactUser.Contact;

            return contact;
        }

        public List<Group> GetListGroup(Controller controller, string userName, int contactId)
        {
            List<Group> listGroup = (
                from
                    groupContacts in db.GroupsContact
                join
                    groups in db.Groups on groupContacts.GroupId equals groups.GroupId
                join
                    groupUsers in db.GroupUser on groups.GroupId equals groupUsers.GroupId
                join
                    contacts in db.Contacts on groupContacts.ContactId equals contacts.ContactId
                join
                    contactUsers in db.ContactUser on contacts.ContactId equals contactUsers.ContactId
                where
                    contactUsers.UserName == userName && contacts.ContactId == contactId
                select groups
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
                from
                    contacts in db.Contacts
                join
                    contacUsers in db.ContactUser on contacts.ContactId equals contacUsers.ContactId
                join
                    groupContacts in db.GroupsContact on contacts.ContactId equals groupContacts.ContactId
                where
                    contacUsers.UserName == userName && groupContacts.GroupId == groupId
                select contacts
            ).ToList();

            return listContact;
        }
    }
}