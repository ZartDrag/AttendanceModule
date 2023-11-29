using System.ComponentModel.DataAnnotations;

namespace MVCTest.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string enrollmentNo { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Section { get; set; }
        [Required]
        public int Semester { get; set; }
        public DateTime createdAt { get; set; } = DateTime.Now;
        public ICollection<Attendance> Student_Attendance { get; set; }
    }
}
