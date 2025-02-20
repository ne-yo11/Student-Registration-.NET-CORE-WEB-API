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
            return await _context.Courses
                .Where(c => !c.isdeleted) // Only fetch active courses
                .ToListAsync();
        }

        // Get a course by CourseCode
        public async Task<Course?> GetCourseByCodeAsync(string courseCode)
        {
            return await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseCode == courseCode && !c.isdeleted);
        }

        //Update course details
        public async Task<Course?> UpdateCourseAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }

        //delete course (update account status)
        public async Task<Course?> SoftDeleteCourseAsync(string courseCode)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseCode == courseCode);
            if (course == null)
            {
                return null; // Course not found
            }

            course.isdeleted = true;
            course.whendeleted = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"); // Store timestamp

            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }
        //Delete Course Details
        public async Task DeleteCourseAsync(Course course)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }



    }
}
