using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_Registration.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [StringLength(12)]
        public string? StudentCode { get; set; }

        [Required]
        [StringLength(200)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(200)]
        public string LastName { get; set; }

        [StringLength(200)]
        public string? MiddleName { get; set; }

        public DateTime Birthdate { get; set; }
        public int Age { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(15)]
        public string? Contact { get; set; }

        [StringLength(100)]
        public string? GuardianName { get; set; }

        [StringLength(255)]
        public string? GuardianAddress { get; set; }

        [StringLength(15)]
        public string? GuardianContact { get; set; }

        [Required]
        [StringLength(10)]
        public string CourseCode { get; set; }

        [ForeignKey("CourseCode")]
        public virtual Course Course { get; set; }

        public virtual ICollection<StudentDocuments> StudentDocuments { get; set; } = new List<StudentDocuments>();

        [StringLength(255)]
        public string? Hobby { get; set; }
    }
}

