using Microsoft.EntityFrameworkCore;
using ExamApi.Models;

namespace ExamApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique email
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // Unique (StudentId, TestId) in Results
            modelBuilder.Entity<Result>().HasIndex(r => new { r.StudentId, r.TestId }).IsUnique();

            // Submission -> Question (explicit foreign key)
            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Question)
                .WithMany(q => q.Submissions)
                .HasForeignKey(s => s.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Submission -> Student
            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Result -> Student
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Student)
                .WithMany()
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Result -> Test
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Test)
                .WithMany()
                .HasForeignKey(r => r.TestId)
                .OnDelete(DeleteBehavior.Restrict);

            // Question -> Test
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Test)
                .WithMany(t => t.Questions)
                .HasForeignKey(q => q.TestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}