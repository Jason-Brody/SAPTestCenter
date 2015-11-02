namespace SAPTest.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class SAPTestContext : DbContext
    {
        public SAPTestContext()
            : base("name=Test")
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        public virtual DbSet<Access> Accesses { get; set; }
        public virtual DbSet<AccessLog> AccessLogs { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AccountUser> AccountUsers { get; set; }
        public virtual DbSet<AcctUsageLog> AcctUsageLogs { get; set; }
        public virtual DbSet<Asset> Assets { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Release> Releases { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
       
        //public virtual DbSet<UserLog> UserLogs { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<LogonRecord> LogonRecords { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasMany(e => e.Accesses)
                .WithRequired(e => e.Account)
                .HasForeignKey(e => e.AcctId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Account>()
                .HasMany(e => e.AccountUsers)
                .WithRequired(e => e.Account)
                .HasForeignKey(e => e.AcctId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Project>()
                .HasMany(e => e.Releases)
                .WithOptional(e => e.Project)
                .HasForeignKey(e => e.Pid);

            modelBuilder.Entity<Release>()
                .HasMany(e => e.Assets)
                .WithOptional(e => e.Release)
                .HasForeignKey(e => e.Rid);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Accesses)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.Uid)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.AccountUsers)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.Uid)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Reports)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.Uid);
        }
    }
}
