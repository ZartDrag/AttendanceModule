using System.ComponentModel.DataAnnotations;

namespace MVCTest.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int timeSlot { get; set; }
        public bool isPresent { get; set; }

        public Student Student { get; set; }
        public Subject Subject { get; set; }

    }
}
