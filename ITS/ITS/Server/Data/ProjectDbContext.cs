
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection.Emit;

namespace ITS.Server.Data
{
    public class ProjectDbContext : DbContext
    {

        public DbSet<Project> Projects { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<SubJob> SubJobs { get; set; }
        public DbSet<BOM> BillOfMaterials { get; set; }
        public DbSet<Material> Materials { get; set; }

        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>()
            .HasOne<Project>(s => s.Project)
            .WithMany(g => g.Jobs)
            .HasForeignKey(s => s.ProjectId);


            modelBuilder.Entity<SubJob>()
            .HasOne<Job>(s => s.Job)
            .WithMany(g => g.SubJobs)
            .HasForeignKey(s => s.JobId);


            modelBuilder.Entity<BOM>()
                .HasOne<SubJob>(x => x.SubJob)
                .WithMany(x => x.BillOfMaterials)
                .HasForeignKey(s => s.SubJobId);


            base.OnModelCreating(modelBuilder);
        }


    }
}
