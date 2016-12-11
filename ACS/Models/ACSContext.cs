using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace ACS.Models
{
    public class ACSContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public ACSContext() : base("name=ACSContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public System.Data.Entity.DbSet<ACS.Models.DocumentType> DocumentTypes { get; set; }

        public System.Data.Entity.DbSet<ACS.Models.Partner> Partners { get; set; }

        public System.Data.Entity.DbSet<ACS.Models.Employee> Employees { get; set; }

        public System.Data.Entity.DbSet<ACS.Models.Sport> Sports { get; set; }

        public System.Data.Entity.DbSet<ACS.Models.MonthlyFee> MonthlyFees { get; set; }

        public System.Data.Entity.DbSet<ACS.Models.MonthlyFeeDetail> MonthlyFeeDetails { get; set; }

        public DbSet<PartnerSport> PartnerSport { get; set; }
    }
}
