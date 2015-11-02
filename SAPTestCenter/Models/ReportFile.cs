using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAPTest.Model;

namespace SAPTestCenter.Models
{
    public class ReportFile
    {
        public Asset Asset { get; set; }

        public HttpPostedFile File { get; set; }
        
    }
}