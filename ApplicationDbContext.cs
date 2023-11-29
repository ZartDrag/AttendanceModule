using Microsoft.EntityFrameworkCore;
using MVCTest.Models;

namespace MVCTest
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(ab => ab.Student_Attendance)
                .HasForeignKey(ai => ai.StudentId);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Subject)
                .WithMany(ab => ab.Subject_Attendance)
                .HasForeignKey(ai => ai.SubjectId);
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
    }
}
