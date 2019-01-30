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
        public DbQuery<StudentClassDetail> StudentClassDetails { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbQuery<Quarter> Quarters { get; set; }
        public DbSet<Scores> Scores { get; set; }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("ELIDatabase"));
            }
        }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //define views for these models
            modelBuilder.Entity<Student>().ToTable("Students");
            modelBuilder.Entity<Scores>().ToTable("Scores");
            modelBuilder.Query<StudentClassDetail>().ToView("vw_StudentClassDetail");
            modelBuilder.Query<Quarter>().ToView("vw_YearQuarter");
        }
    }
}
