using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Student_Registration.Models
{
    public class Course
    {
        [Key]  // Marks this as the primary key
        [Required]
        [StringLength(10)]
        public string CourseCode { get; set; }  // CourseCode as the primary key

        [Required]
        [StringLength(100)]
        public string CourseName { get; set; }

        public int Duration { get; set; }  // Duration in semesters

        [Required]
        [StringLength(100)]
        public string Department { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [StringLength(10)]
        public string Status { get; set; } // Active or Inactive

        // Navigation Property for Students (One Course can have many Students)
        [JsonIgnore]
        public virtual ICollection<Student> Students { get; set; }

        public bool isdeleted { get; set; } = false;
        public string? whendeleted { get; set; }
        public string? whenrestored { get; set; }
    }
}
