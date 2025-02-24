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
            return await _context.Courses.ToListAsync();// Only fetch active courses
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

        //delete course (update account status to Inactive)
        public async Task<Course?> SoftDeleteCourseAsync(string courseCode)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseCode == courseCode);
            if (course == null)
            {
                return null; // Course not found
            }

            course.isdeleted = true;
            course.whendeleted = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"); // Store timestamp
            course.Status = "Inactive";

            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }

        //restore course(update accont status to Active)
        public async Task<Course?> SoftRestoreCourseAsync(string courseCode)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseCode == courseCode);
            if (course == null || !course.isdeleted)
            {
                return null; // Course not found or not deleted
            }

            course.isdeleted = false;
            course.whendeleted = null; // Clear deletion timestamp
            course.whenrestored = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"); // Store timestamp
            course.Status = "Active";

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

        //count number of active and inactive courses
        public async Task<Dictionary<int, int>> CountActiveInactiveCourseAsync()
        {
            var activeCount = await _context.Courses.CountAsync(c => c.Status == "Active");
            var inactiveCount = await _context.Courses.CountAsync(c => c.Status == "Inactive");

            var result = new Dictionary<int, int>
            {
                { 1, activeCount },   // Active courses
                { 0, inactiveCount }  // Inactive courses
            };

            return result;
        }


    }
}
