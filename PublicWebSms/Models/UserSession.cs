using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicWebSms.Models
{
    // Definisi model data dari sesi
    public class SessionData
    {
        public string UserName { get; set; }
        public string CheckingString { get; set; }
        public bool IsAdmin { get; set; }

        public SessionData()
        {
            IsAdmin = false;
        }
    }

	// Definisi penggunaan sesi pada sistem login
	public static class UserSession
	{
        public static bool IsAdmin()
        {
            if (IsLogin())
            {
                SessionData sess = (SessionData) HttpContext.Current.Session["sessdata"];
                return sess.IsAdmin;
            }
            return false;
        }
        public static bool IsLogin()
        {
            // Cek apakah data pada "sessdata" ada
            if (HttpContext.Current.Session["sessdata"] != null)
            {   
                SessionData sessData = HttpContext.Current.Session["sessdata"] as SessionData;
                if(sessData.CheckingString == "hanahbanana")
                {
                    return true;
                }
            }

            return false;
        }

        public static bool DoLogin(string userName, string password)
        {
            PwsDbContext db = new PwsDbContext();

            // Cek apakah pengguna berhak untuk login atau tidak
            if (db.Users.Where(x => x.LoginName == userName && x.LoginPassword == password && x.Activate == true).Count() == 1)
            {
                // User berhak login, lakukan penulisan data pada sesi
                HttpContext.Current.Session["sessdata"] = new SessionData { CheckingString = "hanahbanana", UserName = userName, IsAdmin = false };

                // Catatkan Lastlogin pada tabel user
                db.Users.SingleOrDefault(x => x.LoginName == userName).LastLogin = DateTime.Now;
                db.SaveChanges();

                return true;
            }
            else if(db.Admins.Where(x => x.AdminName == userName && x.Password == password).Count() == 1)
            {
                HttpContext.Current.Session["sessdata"] = new SessionData { CheckingString = "hanahbanana", UserName = userName, IsAdmin = true };
                return true;
            }

            return false;
        }

        public static string GetLoggedUserName()
        {
            if (IsLogin())
            {
                SessionData sessData = HttpContext.Current.Session["sessdata"] as SessionData;
                return sessData.UserName;
            }
            
            return null;
        }

        public static void DoLogout()
        {
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.Session.RemoveAll();
        }
	}

    public static class FreeSmsSession
    {
        private static int limit = 3;

        public static int GetCurrentSmsCount()
        {
            if (HttpContext.Current.Session["smsCount"] != null)
            {
                int smsCount = (int)HttpContext.Current.Session["smsCount"];
                return smsCount;
            }

            return 0;
        }
        public static void IncrementSmsTotal()
        {
            if (HttpContext.Current.Session["smsCount"] != null)
            {
                int smsCount = (int)HttpContext.Current.Session["smsCount"];
                if (smsCount < limit)
                {
                    HttpContext.Current.Session["smsCount"] = smsCount + 1;
                }
            }
            else
            {
                HttpContext.Current.Session["smsCount"] = 1;
            }
        }

        public static bool IsLimit()
        {
            if (HttpContext.Current.Session["smsCount"] != null)
            {
                int smsCount = (int)HttpContext.Current.Session["smsCount"];
                if (smsCount>=limit)
                {
                    return true;
                }
            }
            
            return false;
            
        }

        public static int GetSmsLimit()
        {
            return limit;
        }
    }
}