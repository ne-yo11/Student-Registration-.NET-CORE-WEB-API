using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Student_Registration.Models
{
    public class StudentDocuments
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("StudentCode")]
        public string StudentCode { get; set; }  // Foreign key to Student

        
        public virtual Student Student { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FileType { get; set; } // e.g., "image/png", "application/pdf"

        [Required]
        public byte[] Data { get; set; }  // Store file as byte array
    }
}
