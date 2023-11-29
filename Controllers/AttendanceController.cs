using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCTest.Models;
using MVCTest.Models.ViewModels;
using static System.Collections.Specialized.BitVector32;

namespace MVCTest.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AttendanceController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult addIndex()
        {
            var tupleModel = new Tuple<IEnumerable<Student>, IEnumerable<Subject>>(_db.Students.ToList(), _db.Subjects.ToList());
            return View(tupleModel);
        }

        public IActionResult ViewSubject()
        {
            IEnumerable<Subject> Subjects = _db.Subjects.ToList();
            return View(Subjects);
        }

        public IActionResult ViewStudent()
        {
            IEnumerable<Student> Students = _db.Students.ToList();
            return View(Students);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewSubjectNext(AttendanceViewModel model)
        {
            IEnumerable<StudentAttendanceViewModel> studentAttendance = _db.Students
            .Where(student => student.Section == model.Section)
            .Include(student => student.Student_Attendance)
                .ThenInclude(attendance => attendance.Subject)
            .Select(student => new StudentAttendanceViewModel
            {
                enrollmentNo = student.enrollmentNo,
                Name = student.Name,
                PresentCount = student.Student_Attendance
                    .Count(attendance =>
                        attendance.SubjectId == model.SubjectId &&
                        attendance.isPresent &&
                        attendance.Date >= model.SelectedDate &&
                        attendance.Date <= model.SelectedEndDate),
                AbsentCount = student.Student_Attendance
                    .Count(attendance =>
                        attendance.SubjectId == model.SubjectId &&
                        !attendance.isPresent &&
                        attendance.Date >= model.SelectedDate &&
                        attendance.Date <= model.SelectedEndDate),
                TotalCount = student.Student_Attendance
                    .Count(attendance =>
                        attendance.SubjectId == model.SubjectId &&
                        attendance.Date >= model.SelectedDate &&
                        attendance.Date <= model.SelectedEndDate)
            })
            .ToList();

            string subjectName = _db.Subjects
            .Where(subject => subject.Id == model.SubjectId)
            .Select(subject => subject.SubjectName)
            .FirstOrDefault();

            Tuple<string, int, IEnumerable<StudentAttendanceViewModel>> trip = new Tuple<string, int, IEnumerable<StudentAttendanceViewModel>>(subjectName, model.Section, studentAttendance);

            return View(trip);
        }

        public IActionResult ViewStudentNext(int StudentId, int Semester)
        {

            IEnumerable<StudentAttendanceViewModel> studentAttendance = _db.Attendances
            .Include(a => a.Student)
            .Include(a => a.Subject)
            .Where(a => a.Subject.Semester == Semester && a.Student.Id == StudentId)
            .GroupBy(a => a.Subject.SubjectName)
            .Select(g => new StudentAttendanceViewModel
            {
                Subject = g.Key,
                PresentCount = g.Count(a => a.isPresent),
                AbsentCount = g.Count(a => !a.isPresent),
                TotalCount = g.Count()
            })
            .ToList();




            float totalPresent = 0, totalAbsent = 0;
            foreach (var attendance in studentAttendance)
            {
                attendance.Percentage = attendance.PresentCount * 100 / (attendance.PresentCount + attendance.AbsentCount);
                totalPresent += attendance.PresentCount;
                totalAbsent += attendance.AbsentCount;
            }

            float totalPercentage = totalPresent * 100 / (totalPresent + totalAbsent);


            Student studentInfo = _db.Students
            .Where(student => student.Id == StudentId)
            .Select(student => new Student
            {
                Name = student.Name,
                enrollmentNo = student.enrollmentNo,
                Section = student.Section
            })
            .FirstOrDefault();

            Tuple<Student, float, IEnumerable<StudentAttendanceViewModel>> trip = new Tuple<Student, float, IEnumerable<StudentAttendanceViewModel>>(studentInfo, totalPercentage, studentAttendance);

            return View(trip);
        }

        [HttpPost]
        public IActionResult formSubmit(AttendanceViewModel model)
        {

            IEnumerable<Student> StuObj = _db.Students.ToList();
            IEnumerable<Student> StudList = StuObj.Where(StuOb => StuOb.Semester == model.Semester && StuOb.Section == model.Section).ToList();
            model.StudentList = new List<StudViewModel>();

            foreach (var stud in StudList)
            {
                model.StudentList.Add(new StudViewModel
                {
                    Id = stud.Id,
                    enrollmentNo = stud.enrollmentNo,
                    Name = stud.Name,
                    isPresent = false
                });
            }

            
            return View(model);
        }

        [HttpPost]
        public IActionResult RecordAttendance(AttendanceViewModel model)
        {

            foreach (var (studentId, present) in model.Attendance)
            {
                var newAttendance = new Attendance
                {
                    StudentId = studentId,
                    SubjectId = model.SubjectId,
                    timeSlot = model.Time,
                    isPresent = present,
                    Date = model.SelectedDate
                };

                _db.Attendances.Add(newAttendance);

            }
                _db.SaveChanges();            

            //Note to Self: Place manual checks to simulate composite PK behaviour

            return RedirectToAction("Index");
        }

        
    }
}
