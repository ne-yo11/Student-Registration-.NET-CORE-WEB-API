using Microsoft.AspNetCore.Mvc;
using Student_Registration.Models;
using Student_Registration.Services;
using Student_Registration.DTOs;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Student_Registration.Controllers
{
    [Authorize]
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentService _studentService;

        public StudentController(StudentService studentService)
        {
            _studentService = studentService;
        }

        // Register a Student
        [HttpPost("register")]
        public async Task<IActionResult> RegisterStudent([FromBody] StudentDTO studentDto)
        {
            if (studentDto == null)
                return BadRequest(new { message = "Invalid student data." });

            // Validate and Convert Date
            if (!DateTime.TryParseExact(studentDto.Birthdate.ToString("yyyy-dd-MM"), "yyyy-dd-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
            {
                return BadRequest(new { message = "Invalid date format. Expected format: YYYY-DD-MM" });
            }

            var student = new Student
            {
                FirstName = studentDto.FirstName,
                LastName = studentDto.LastName,
                MiddleName = studentDto.MiddleName,
                Birthdate = birthDate, 
                Age = studentDto.Age,
                Gender = studentDto.Gender,
                Address = studentDto.Address,
                Contact = studentDto.Contact,
                GuardianName = studentDto.GuardianName,
                GuardianAddress = studentDto.GuardianAddress,
                GuardianContact = studentDto.GuardianContact,
                Hobby = studentDto.Hobby,
                CourseCode = studentDto.CourseCode
            };

            student.StudentCode = await _studentService.GenerateUniqueStudentCodeAsync();

            var registeredStudent = await _studentService.RegisterStudentAsync(student, studentDto.CourseName, studentDto.CourseStatus);

            if (registeredStudent == null)
                return Conflict(new { message = "Student already exists or invalid Course Code." });

            return CreatedAtAction(nameof(GetStudentByCode), new { studentCode = registeredStudent.StudentCode }, registeredStudent);
        }

        // Get Student by StudentCode
        [HttpGet("{studentCode}")]
        public async Task<IActionResult> GetStudentByCode(string studentCode)
        {
            var studentDto = await _studentService.GetStudentByCodeAsync(studentCode);

            if (studentDto == null)
                return NotFound(new { message = "Student not found." });

            return Ok(studentDto);
        }

        // Retrieve all students
        [HttpGet("list")]
        public async Task<ActionResult<List<StudentDTO>>> GetAllStudents()
        {
            var studentList = await _studentService.GetAllStudentsAsync();

            if (studentList == null || studentList.Count == 0)
                return NotFound(new { message = "No students found." });

            return Ok(studentList);
        }
        [HttpGet("search")]
        public async Task<ActionResult<List<StudentDTO>>> SearchStudents(
    [FromQuery] string? name,
    [FromQuery] string? courseCode,
    [FromQuery] int? yearLevel)
        {
            var filteredStudents = await _studentService.SearchStudentsAsync(name, courseCode, yearLevel);

            if (filteredStudents == null || filteredStudents.Count == 0)
                return NotFound(new { message = "No students found matching the criteria." });

            return Ok(filteredStudents);
        }


        //Update student details
        [HttpPut("update/{studentCode}")]
        public async Task<IActionResult> UpdateStudent(string studentCode, [FromBody] StudentDTO studentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _studentService.UpdateStudentInfoAsync(studentCode, studentDto);
            if (!result)
                return NotFound(new { message = "Student not found" });

            return Ok(new { message = "Student updated successfully" });
        }


        //Delete student details
        [HttpDelete("delete/{studentCode}")]
        public async Task<IActionResult> DeleteStudent(string studentCode)
        {
            var result = await _studentService.DeleteStudentAsync(studentCode);

            if (!result)
                return NotFound(new { message = "Student not found" });

            return Ok(new { message = "Student deleted successfully" });
        }
    }
}
