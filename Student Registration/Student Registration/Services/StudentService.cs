using Student_Registration.Models;
using Student_Registration.Data;
using Student_Registration.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Student_Registration.Services
{
    public class StudentService
    {
        private readonly StudentDbContext _context;

        public StudentService(StudentDbContext context)
        {
            _context = context;
        }

        // Register Student
        public async Task<Student?> RegisterStudentAsync(Student student, string courseName, string courseStatus)
        {
            student.StudentCode = await GenerateUniqueStudentCodeAsync();

            // Convert Date to Correct Format for Database
            student.Birthdate = student.Birthdate.Date; // Keep original DateTime format

            // Assign course if CourseCode is provided
            if (!string.IsNullOrEmpty(student.CourseCode))
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseCode == student.CourseCode);

                if (course == null)
                {
                    course = new Course
                    {
                        CourseCode = student.CourseCode,
                        CourseName = courseName,
                        Status = courseStatus
                    };

                    _context.Courses.Add(course);
                    await _context.SaveChangesAsync();
                }

                student.Course = course;
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return student;
        }

        // Generate unique StudentCode
        public async Task<string> GenerateUniqueStudentCodeAsync()
        {
            string year = DateTime.Now.Year.ToString().Substring(2, 2);
            int attempt = 0;
            string studentCode;

            do
            {
                int studentCount = await _context.Students
                    .Where(s => s.StudentCode.StartsWith($"SC{year}"))
                    .CountAsync();

                string studentNumber = (studentCount + 1 + attempt).ToString("D4");
                studentCode = $"SC{year}-{studentNumber}";

                attempt++;
            }
            while (await _context.Students.AnyAsync(s => s.StudentCode == studentCode));

            return studentCode;
        }

        // Get Student by StudentCode
        public async Task<StudentDTO?> GetStudentByCodeAsync(string studentCode)
        {
            var student = await _context.Students
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.StudentCode == studentCode);

            if (student == null)
                return null;

            return new StudentDTO
            {
                FirstName = student.FirstName,
                LastName = student.LastName,
                MiddleName = student.MiddleName,
                Birthdate = student.Birthdate, 
                Age = student.Age,
                Gender = student.Gender,
                Address = student.Address,
                Contact = student.Contact,
                GuardianName = student.GuardianName,
                GuardianAddress = student.GuardianAddress,
                GuardianContact = student.GuardianContact,
                Documents = student.Documents,
                Hobby = student.Hobby,
                CourseCode = student.Course?.CourseCode,
                CourseName = student.Course?.CourseName,
                CourseStatus = student.Course?.Status
            };
        }
        public async Task<List<StudentDTO>> SearchStudentsAsync(string? name, string? courseCode, int? yearLevel)
        {
            var query = _context.Students
                .Include(s => s.Course)
                .AsQueryable();

            // Filter by name (First or Last Name)
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s =>
                    s.FirstName.Contains(name) ||
                    s.LastName.Contains(name));
            }

            // Filter by course code
            if (!string.IsNullOrEmpty(courseCode))
            {
                query = query.Where(s => s.Course.CourseCode == courseCode);
            }

            // Filter by year level (based on third character in CourseCode)
            if (yearLevel.HasValue)
            {
                query = query.Where(s =>
                    s.Course != null &&
                    s.Course.CourseCode.Length >= 3 &&
                    s.Course.CourseCode.Substring(2, 1) == yearLevel.Value.ToString()
                );
            }

            var students = await query.ToListAsync();

            return students.Select(student => new StudentDTO
            {
                FirstName = student.FirstName,
                LastName = student.LastName,
                MiddleName = student.MiddleName,
                Birthdate = student.Birthdate,
                Age = student.Age,
                Gender = student.Gender,
                Address = student.Address,
                Contact = student.Contact,
                GuardianName = student.GuardianName,
                GuardianAddress = student.GuardianAddress,
                GuardianContact = student.GuardianContact,
                Hobby = student.Hobby,
                CourseCode = student.Course?.CourseCode,
                CourseName = student.Course?.CourseName,
                CourseStatus = student.Course?.Status
            }).ToList();
        }
        // Get all students
        public async Task<List<StudentDTO>> GetAllStudentsAsync()
        {
            var students = await _context.Students
                .Include(s => s.Course) // Include related course data
                .ToListAsync();

            return students.Select(student => new StudentDTO
            {
                FirstName = student.FirstName,
                LastName = student.LastName,
                MiddleName = student.MiddleName,
                Birthdate = student.Birthdate,
                Age = student.Age,
                Gender = student.Gender,
                Address = student.Address,
                Contact = student.Contact,
                GuardianName = student.GuardianName,
                GuardianAddress = student.GuardianAddress,
                GuardianContact = student.GuardianContact,
                Hobby = student.Hobby,
                CourseCode = student.Course?.CourseCode,
                CourseName = student.Course?.CourseName,
                CourseStatus = student.Course?.Status
            }).ToList();
        }
    }
}
