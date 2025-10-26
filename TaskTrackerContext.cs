using Microsoft.EntityFrameworkCore;
using SGS.TaskTracker.Entities;
using SGS.TaskTracker.Models;


namespace SGS.TaskTracker
{
    class TaskTrackerContext : DbContext
    {
        public TaskTrackerContext(DbContextOptions<TaskTrackerContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<TaskItem> TaskItems => Set<TaskItem>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("task_tracker");

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Role).IsRequired();
                entity.Property(u => u.CreatedAt).IsRequired();

                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();

                entity.HasMany(u => u.RefreshTokens)
                      .WithOne(rt => rt.User)
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.AssignedTasks)
                      .WithOne(t => t.AssignedUser)
                      .HasForeignKey(t => t.AssignedUserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
                entity.Property(t => t.Description).HasMaxLength(1000);
                entity.Property(t => t.Status).IsRequired();
                entity.Property(t => t.DueDate).IsRequired();
                entity.Property(t => t.CreatedDate).IsRequired();

                entity.HasOne(t => t.AssignedUser)
                      .WithMany(u => u.AssignedTasks)
                      .HasForeignKey(t => t.AssignedUserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);
                entity.Property(rt => rt.Token).IsRequired().HasMaxLength(500);
                entity.Property(rt => rt.Expires).IsRequired();
                entity.Property(rt => rt.Created).IsRequired();
                entity.Property(rt => rt.IsRevoked).IsRequired();

                entity.HasIndex(rt => rt.Token);
                entity.HasIndex(rt => new { rt.UserId, rt.IsRevoked });
            });
        }
    }
}
