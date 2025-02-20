using Microsoft.EntityFrameworkCore;
using Student_Registration.Models;

namespace Student_Registration.Data
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<StudentDocuments> StudentDocument { get; set; }


    }
}
