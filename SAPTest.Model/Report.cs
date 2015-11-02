namespace SAPTest.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Report
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string TestName { get; set; }

        [StringLength(25)]
        public string CaseNum { get; set; }

        [StringLength(50)]
        public string CompanyCode { get; set; }

        public bool? TestResult { get; set; }

        [StringLength(100)]
        public string Executor { get; set; }

        public int? AssetId { get; set; }

        [StringLength(255)]
        public string Url { get; set; }

        public DateTime SubmitDt { get; set; }

        public int? Uid { get; set; }

        public virtual Asset Asset { get; set; }

        public virtual User User { get; set; }
    }
}
