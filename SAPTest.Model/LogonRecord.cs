namespace SAPTest.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LogonRecord")]
    public partial class LogonRecord
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        [StringLength(255)]
        public string User { get; set; }

        [StringLength(255)]
        public string Tim { get; set; }

        [StringLength(255)]
        public string Box { get; set; }

        [StringLength(255)]
        public string Username { get; set; }

        [StringLength(255)]
        public string Style { get; set; }

        [StringLength(255)]
        public string Status { get; set; }

        [StringLength(255)]
        public string Machine { get; set; }
    }
}
