using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicWebSms.Models
{
    class PwsAdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            if (UserSession.IsLogin() && UserSession.IsAdmin())
            {
                return true;
            }

            return false;
        }
    }
}