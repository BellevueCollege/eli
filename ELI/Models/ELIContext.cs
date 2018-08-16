using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ELI.Models
{
    public partial class ELIContext : DbContext
    {
        public ELIContext()
        {
        }

        public ELIContext(DbContextOptions<ELIContext> options)
            : base(options)
        {
        }

        public DbQuery<StudentSearch> StudentSearchResults { get; set; }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("ELIDatabase"));
            }
        }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<StudentSearch>().HasKey(ss => ss.Sid);
            //modelBuilder.Query<StudentSearch>().ToQuery( ss => "exec usp_getStudentViewData");
            //modelBuilder.Query<StudentSearch>().
        }
    }
}
