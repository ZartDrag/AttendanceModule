namespace MVCTest.Models.ViewModels
{
    public class StudViewModel
    {
        public int Id { get; set; }
        public string enrollmentNo { get; set; }
        public string Name { get; set; }
        public bool isPresent { get; set; }
        public DateTime Date { get; set; }
        public int TimeSlot { get; set; }
    }
}
