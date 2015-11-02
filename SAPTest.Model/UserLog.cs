namespace SAPTest.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserLog
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string NTAccount { get; set; }

        public DateTime? CreateDT { get; set; }
    }
}
