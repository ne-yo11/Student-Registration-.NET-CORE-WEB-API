using Microsoft.AspNetCore.Mvc;
using Student_Registration.Models;
using Student_Registration.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Student_Registration.Controllers
{
    [Route("api/course")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly CourseService _courseService;

        public CourseController(CourseService courseService)
        {
            _courseService = courseService;
        }

        // Add a new course
        [HttpPost("add")]
        public async Task<IActionResult> AddCourse([FromBody] CourseDTO courseDto)
        {
            if (courseDto == null)
                return BadRequest("Course data is required.");

            var course = new Course
            {
                CourseCode = courseDto.CourseCode,
                CourseName = courseDto.CourseName,
                Duration = courseDto.Duration,
                Department = courseDto.Department,
                Description = courseDto.Description,
                Status = courseDto.Status
            };

            var createdCourse = await _courseService.AddCourseAsync(course);

            if (createdCourse == null)
                return Conflict("A course with this CourseCode already exists.");

            return CreatedAtAction(nameof(GetCourseByCode), new { courseCode = createdCourse.CourseCode }, createdCourse);
        }

        // Retrieve all courses
        [HttpGet("list")]
        public async Task<ActionResult<List<Course>>> GetAllCourses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        // Retrieve a course by CourseCode
        [HttpGet("{courseCode}")]
        public async Task<ActionResult<Course>> GetCourseByCode(string courseCode)
        {
            var course = await _courseService.GetCourseByCodeAsync(courseCode);
            if (course == null)
                return NotFound(new { message = "Course not found." });

            return Ok(course);
        }
        [HttpPut("update/{courseCode}")]
        public async Task<IActionResult> UpdateCourse(string courseCode, [FromBody] CourseDTO courseDto)
        {
            if (courseDto == null)
                return BadRequest("Course data is required.");

            var existingCourse = await _courseService.GetCourseByCodeAsync(courseCode);
            if (existingCourse == null)
                return NotFound(new { message = "Course not found." });

            // Update course properties
            existingCourse.CourseName = courseDto.CourseName;
            existingCourse.Duration = courseDto.Duration;
            existingCourse.Department = courseDto.Department;
            existingCourse.Description = courseDto.Description;
            existingCourse.Status = courseDto.Status;

            var updatedCourse = await _courseService.UpdateCourseAsync(existingCourse);

            return Ok(updatedCourse);
        }
        [HttpDelete("delete/{courseCode}")]
        public async Task<IActionResult> DeleteCourse(string courseCode)
        {
            var course = await _courseService.GetCourseByCodeAsync(courseCode);
            if (course == null)
                return NotFound(new { message = "Course not found." });

            await _courseService.DeleteCourseAsync(course);
            return NoContent(); // 204 No Content response
        }




    }
}
