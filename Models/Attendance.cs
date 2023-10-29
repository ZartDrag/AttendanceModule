using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCTest.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        [ForeignKey("Students")]
        public int StudentId { get; set;}
        [ForeignKey("Subjects")]
        public int SubjectId { get; set;}
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int timeSlot { get; set;}
        public bool isPresent { get; set;}

        public virtual Student Students { get; set;}
        public virtual Subject Subjects { get; set;}
    }
}
