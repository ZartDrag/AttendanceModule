namespace MVCTest.Models.ViewModels
{
    public class AttendanceViewModel
    {
        public List<StudViewModel> StudentList { get; set; }
        public Dictionary<int, bool> Attendance { get; set; }
        public DateTime SelectedDate { get; set; }
        public DateTime SelectedEndDate { get; set; }
        public int Semester { get; set; }
        public int SubjectId { get; set; }
        public int Time { get; set; }
        public int Section { get; set; }        
    }
}
