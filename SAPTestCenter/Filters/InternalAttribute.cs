using SAPTest.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using System.Web.Http;
using System.Web.Http.Controllers;

namespace SAPTestCenter.Filters
{
    public class InternalAttribute:AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var u = GetUser();
            if (u != null)
                return true;
            return false;
        }

        public static User GetUser()
        {
            User user = null;
            using (var db = new SAPTestContext())
            {
                var me = System.Web.HttpContext.Current.User;
                //user = db.Users.Where(u => u.NTAccount == @"ASIAPACIFIC\yanzhou").FirstOrDefault();
                user = db.Users.Where(u => u.NTAccount == me.Identity.Name).FirstOrDefault();
            }
            return user;
        }
    }
}