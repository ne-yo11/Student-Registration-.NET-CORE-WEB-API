using Student_Registration.Models;
using Student_Registration.Data;
using Student_Registration.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using System.Collections.Generic;

namespace Student_Registration.Services
{
    public class StudentService
    {
        private readonly StudentDbContext _context;

        public StudentService(StudentDbContext context)
        {
            _context = context;
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

        // Register Student
        public async Task<Student?> RegisterStudentAsync(Student student, string courseName, string courseStatus, List<IFormFile>? files)
        {
            student.StudentCode = await GenerateUniqueStudentCodeAsync();
            student.Birthdate = student.Birthdate.Date;

            if (!string.IsNullOrEmpty(student.CourseCode))
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseCode == student.CourseCode);
                if (course == null)
                    throw new Exception($"Course with CourseCode '{student.CourseCode}' does not exist.");

                student.Course = course;
            }
            else
            {
                throw new Exception("CourseCode is required.");
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            // FIX: Ensure StudentDocuments is properly initialized
            if (student.StudentDocuments == null)
                student.StudentDocuments = new List<StudentDocuments>();

            // Handle File Uploads
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    using var memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);
                    var documentData = memoryStream.ToArray();

                    var document = new StudentDocuments
                    {
                        StudentCode = student.StudentCode,
                        FileName = file.FileName,
                        FileType = file.ContentType,
                        Data = documentData
                    };

                    student.StudentDocuments.Add(document);
                }

                await _context.SaveChangesAsync();
            }
            // Convert documents to DTO format (Base64)
            return new Student
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
                CourseCode = student.CourseCode,
                Course = student.Course,
                StudentDocuments = student.StudentDocuments?.Select(doc => new StudentDocuments
                {
                    StudentCode = doc.StudentCode,
                    FileName = doc.FileName,
                    FileType = doc.FileType,
                    Data = doc.Data
                }).ToList()
            };
        }

        // Get Student by StudentCode
        public async Task<StudentDTO?> GetStudentByCodeAsync(string studentCode)
        {
            var student = await _context.Students
                .Include(s => s.Course)
                .Include(s => s.StudentDocuments) // Include Student Documents
                .FirstOrDefaultAsync(s => s.StudentCode == studentCode);

            if (student == null)
                return null;

            return new StudentDTO
            {
                StudentCode = student.StudentCode,
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
                CourseStatus = student.Course?.Status,

                // Convert StudentDocuments to a list of document metadata
                Documents = student.StudentDocuments?
                    .Select(d => new StudentDocumentDTO
                    {
                        FileName = d.FileName,
                        FileType = d.FileType,
                        Data = Convert.ToBase64String(d.Data) // Convert binary data to Base64
                    }).ToList()
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
                .Include(s => s.StudentDocuments)
                .ToListAsync();

            return students.Select(student => new StudentDTO
            {
                StudentCode = student.StudentCode,
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
                CourseStatus = student.Course?.Status,

                Documents = student.StudentDocuments != null
            ? student.StudentDocuments.Select(d => new StudentDocumentDTO
            {
                FileName = d.FileName,
                FileType = d.FileType,
                Data = Convert.ToBase64String(d.Data) // Convert binary data to Base64
            }).ToList()
            : new List<StudentDocumentDTO>() // Return an empty list if no documents exist

            }).ToList();
        }



        //Update student details
        public async Task<bool> UpdateStudentInfoAsync(string studentCode, StudentDTO updatedStudentDto)
        {
            var existingStudent = await _context.Students
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.StudentCode == studentCode);

            if (existingStudent == null)
                return false; // Student not found

            // Update student details
            existingStudent.FirstName = updatedStudentDto.FirstName;
            existingStudent.LastName = updatedStudentDto.LastName;
            existingStudent.MiddleName = updatedStudentDto.MiddleName;
            existingStudent.Birthdate = updatedStudentDto.Birthdate;
            existingStudent.Age = updatedStudentDto.Age;
            existingStudent.Gender = updatedStudentDto.Gender;
            existingStudent.Address = updatedStudentDto.Address;
            existingStudent.Contact = updatedStudentDto.Contact;
            existingStudent.GuardianName = updatedStudentDto.GuardianName;
            existingStudent.GuardianAddress = updatedStudentDto.GuardianAddress;
            existingStudent.GuardianContact = updatedStudentDto.GuardianContact;
            existingStudent.Hobby = updatedStudentDto.Hobby;

            // Update Course Details if changed
            if (!string.IsNullOrEmpty(updatedStudentDto.CourseCode))
            {
                var course = await _context.Courses
                    .FirstOrDefaultAsync(c => c.CourseCode == updatedStudentDto.CourseCode);

                if (course == null)
                {
                    course = new Course
                    {
                        CourseCode = updatedStudentDto.CourseCode,
                        CourseName = updatedStudentDto.CourseName,
                        Status = updatedStudentDto.CourseStatus
                    };

                    _context.Courses.Add(course);
                    await _context.SaveChangesAsync();
                }

                existingStudent.Course = course;
            }

            _context.Students.Update(existingStudent);
            await _context.SaveChangesAsync();
            return true;
        }

        //Delete student details
        public async Task<bool> DeleteStudentAsync(string studentCode)
        {
            var existingStudent = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentCode == studentCode);

            if (existingStudent == null)
                return false; // Student not found

            _context.Students.Remove(existingStudent);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
