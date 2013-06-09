using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace PublicWebSms.Models
{
    class PwsAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            if(UserSession.IsLogin())
            {
                return true;
            }

            return false;
        }
    }
}
