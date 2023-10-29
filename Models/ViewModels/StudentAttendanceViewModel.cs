namespace MVCTest.Models.ViewModels
{
    public class StudentAttendanceViewModel
    {
        public string enrollmentNo { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public int PresentCount { get; set; }   
        public int AbsentCount { get; set; }
        public int TotalCount { get; set; }
        public float Percentage { get; set; }   

    }
}
