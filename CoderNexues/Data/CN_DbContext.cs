using CoderNexues.Models;
using Microsoft.EntityFrameworkCore;

namespace CoderNexues.Data
{
    public class CN_DbContext : DbContext
    {
        public CN_DbContext(DbContextOptions<CN_DbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Camp> Camps { get; set; }
        public DbSet<CampUser> CampUsers { get; set; }
        public DbSet<C_Task> Tasks { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<FeedbackCategory> FeedbackCategories { get; set; }
        public DbSet<StudentPerformance> StudentPerformances { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<CampSchedule> CampSchedules { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CampUser>()
                .HasOne(cu => cu.User)
                .WithMany(u => u.CampUsers)
                .HasForeignKey(cu => cu.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CampUser>()
                .HasOne(cu => cu.Camp)
                .WithMany(c => c.CampUsers)
                .HasForeignKey(cu => cu.CampID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.Evaluator)
                .WithMany()
                .HasForeignKey(e => e.EvaluatorID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Student)
                .WithMany(u => u.Submissions)
                .HasForeignKey(s => s.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}