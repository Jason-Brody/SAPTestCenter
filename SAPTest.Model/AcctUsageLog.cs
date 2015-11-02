namespace SAPTest.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AcctUsageLog
    {
        public int Id { get; set; }

        public int? Uid { get; set; }

        public DateTime? LogDt { get; set; }

        public int? AcctId { get; set; }

        [StringLength(100)]
        public string Machine { get; set; }

        public bool? IsManual { get; set; }

        public bool? IsExecute { get; set; }
    }
}
