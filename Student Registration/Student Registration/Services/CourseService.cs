using Student_Registration.Models;
using Student_Registration.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Student_Registration.Services
{
    public class CourseService
    {
        private readonly StudentDbContext _context;

        public CourseService(StudentDbContext context)
        {
            _context = context;
        }

        // Add a new course
        public async Task<Course?> AddCourseAsync(Course course)
        {
            // Check if the course already exists
            var existingCourse = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseCode == course.CourseCode);

            if (existingCourse != null)
                return null; // Conflict: Course already exists

            // Add course to the database
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        // Get all courses
        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        // Get a course by CourseCode
        public async Task<Course?> GetCourseByCodeAsync(string courseCode)
        {
            return await _context.Courses.FirstOrDefaultAsync(c => c.CourseCode == courseCode);
        }
    }
}
