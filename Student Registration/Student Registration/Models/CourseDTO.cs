namespace Student_Registration.Models
{
    public class CourseDTO
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int Duration { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public bool isdeleted { get; set; } = false;
        public string? whendeleted { get; set; }
    }
}
