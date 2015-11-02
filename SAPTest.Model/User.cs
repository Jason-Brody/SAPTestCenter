namespace SAPTest.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        public User()
        {
            Accesses = new HashSet<Access>();
            AccountUsers = new HashSet<AccountUser>();
            Reports = new HashSet<Report>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string NTAccount { get; set; }

        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        public DateTime? CreateDt { get; set; }

        public DateTime? LastUpdateDt { get; set; }

        [StringLength(255)]
        public string BusinessUnit { get; set; }

        public int? EmployeeId { get; set; }

        [StringLength(100)]
        public string Manager { get; set; }

        [StringLength(255)]
        public string PhotoUrl { get; set; }

        public bool IsValid { get; set; }

        public virtual ICollection<Access> Accesses { get; set; }

        public virtual ICollection<AccountUser> AccountUsers { get; set; }

        public virtual ICollection<Report> Reports { get; set; }
    }
}
