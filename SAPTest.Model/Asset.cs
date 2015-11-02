namespace SAPTest.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Asset
    {
        public Asset()
        {
            Reports = new HashSet<Report>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int? Rid { get; set; }

        public virtual Release Release { get; set; }

        public virtual ICollection<Report> Reports { get; set; }
    }
}
