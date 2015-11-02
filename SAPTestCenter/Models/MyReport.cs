using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAPTestCenter.Models
{
    public class MyReportFilter
    {
        private int _sort = -1;

        public int Pid { get; set; }

        public int Rid { get; set; }

        public int Aid { get; set; }

        public string qs { get; set; }

        public int Sort { get { return _sort; } set { _sort = value; } }

        public bool isMyReport { get; set; }
    }
}