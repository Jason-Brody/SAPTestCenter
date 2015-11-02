using SAPTest.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAPTestCenter.Controllers
{
    public class SAPBaseController : Controller
    {
        // GET: SAPBase
        protected User getUser()
        {
            User user = null;
            using (var db = new SAPTestContext())
            {
                //user = db.Users.Where(u => u.NTAccount == @"ASIAPACIFIC\yanzhou").FirstOrDefault();
                user = db.Users.Where(u => u.NTAccount == User.Identity.Name).FirstOrDefault();
            }
            return user;
        }
    }
}