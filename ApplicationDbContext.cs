using Microsoft.EntityFrameworkCore;
using MVCTest.Models;

namespace MVCTest
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public void onModelCreating()
        {

        }
        public DbSet<Student> Students { get; set; }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
    }
}
