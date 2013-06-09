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
    }

	// Definisi penggunaan sesi pada sistem login
	public static class UserSession
	{
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
            if (db.Users.Where(x => x.LoginName == userName && x.LoginPassword == password).Count() == 1)
            {
                // User berhak login, lakukan penulisan data pada sesi
                HttpContext.Current.Session["sessdata"] = new SessionData { CheckingString = "hanahbanana", UserName = userName };
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
        }
	}
}