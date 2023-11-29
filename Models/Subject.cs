using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MVCTest.Models
{
    public class Subject
    {
        public int Id { get; set; }
        [Required]
        public string SubId { get; set; }
        [Required]
        public int Semester { get; set; }
        [AllowNull]
        public string? CourseId { get; set; }
        public string SubjectName { get; set; }
        public ICollection<Attendance> Subject_Attendance { get; set; }
    }
}
