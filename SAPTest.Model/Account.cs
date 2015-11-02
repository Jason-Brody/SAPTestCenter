namespace SAPTest.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Account
    {
        public Account()
        {
            Accesses = new HashSet<Access>();
            AccountUsers = new HashSet<AccountUser>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string BoxName { get; set; }

        [Display(Name="User Name")]
        [StringLength(50)]
        public string UserName { get; set; }

        [StringLength(50)]
        public string Password { get; set; }

        [StringLength(50)]
        public string Client { get; set; }

        [StringLength(255)]
        public string ServerAddress { get; set; }

        public bool IsAvailable { get; set; }

        public DateTime? UpdateDt { get; set; }

        public bool IsPrivate { get; set; }

        
        public virtual ICollection<Access> Accesses { get; set; }

        
        public virtual ICollection<AccountUser> AccountUsers { get; set; }
    }
}
