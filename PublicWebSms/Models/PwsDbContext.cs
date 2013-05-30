using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PublicWebSms.Models
{
    public class PwsDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<SMS> SMSes { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupContact> GroupsContact { get; set; }
        public DbSet<ContactUser> ContactUser { get; set; }
        public DbSet<GroupUser> GroupUser { get; set; }
        public DbSet<SMSUser> SMSUser { get; set; }
    }
}