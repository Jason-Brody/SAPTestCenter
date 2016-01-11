using SAPTest.Model;
using SAPTestCenter.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAPTestCenter.Controllers
{
    public class SAPBaseController : Controller
    {
        public SAPBaseController()
        {
            var session = System.Web.HttpContext.Current.Session;


            if (session["IsInnerUser"] == null)
            {
                 
                var user = InternalAttribute.GetUser();
                if (user == null)
                    session["IsInnerUser"] = false;
                else
                    session["IsInnerUser"] = true;
            }

            ViewBag.IsValid = bool.Parse(session["IsInnerUser"].ToString());
        }

        
    }
}