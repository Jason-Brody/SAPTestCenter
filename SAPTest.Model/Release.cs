namespace SAPTest.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Release
    {
        public Release()
        {
            Assets = new HashSet<Asset>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public int? Pid { get; set; }

        public virtual ICollection<Asset> Assets { get; set; }

        public virtual Project Project { get; set; }
    }
}
