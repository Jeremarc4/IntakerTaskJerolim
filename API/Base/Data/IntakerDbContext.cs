using Base.Models;
using Microsoft.EntityFrameworkCore;

namespace Base.Data
{
    public class IntakerDbContext : DbContext
    {
        public IntakerDbContext(DbContextOptions<IntakerDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Status>().HasData(
                new Status
                {
                    Id = 1,
                    Name = "NotStarted"
                },
                new Status
                {
                    Id = 2,
                    Name = "InProgress"
                },
                new Status
                {
                    Id = 3,
                    Name = "Completed"
                });
        }

        public DbSet<IntakerTask> IntakerTasks { get; set; }
        public DbSet<Status> Statuses { get; set; }
    }
}
