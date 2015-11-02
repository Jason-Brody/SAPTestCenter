namespace SAPTest.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AccessLog
    {
        public int Id { get; set; }

        public int? AcctId { get; set; }

        public int? Uid { get; set; }

        public DateTime? CreateDt { get; set; }

       

        public bool IsActivate { get; set; }
    }
}
