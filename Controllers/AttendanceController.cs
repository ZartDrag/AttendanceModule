using Microsoft.AspNetCore.Mvc;
using MVCTest.Models;
using MVCTest.Models.ViewModels;

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
            .Join(
                _db.Attendances.Where(a =>
                    a.SubjectId == model.SubjectId &&
                    a.Date >= model.SelectedDate &&
                    a.Date <= model.SelectedEndDate),
                student => student.Id,
                attendance => attendance.StudentId,
                (student, attendance) => new { Student = student, Attendance = attendance }
            )
            .Where(joinedData => joinedData.Student.Section == model.Section)
            .GroupBy(joinedData => new { joinedData.Student.enrollmentNo, joinedData.Student.Name })
            .Select(g => new StudentAttendanceViewModel
            {
                enrollmentNo = g.Key.enrollmentNo,
                Name = g.Key.Name,
                PresentCount = g.Count(a => a.Attendance.isPresent),
                AbsentCount = g.Count(a => !a.Attendance.isPresent),
                TotalCount = g.Count()
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
            .Join(
                _db.Students,
                attendance => attendance.StudentId,
                student => student.Id,
                (attendance, student) => new { Attendance = attendance, Student = student }
            )
            .Join(
                _db.Subjects,
                joinedData => joinedData.Attendance.SubjectId,
                subject => subject.Id,
                (joinedData, subject) => new { joinedData.Attendance, joinedData.Student, Subject = subject }
            )
            .Where(joinedData => joinedData.Subject.Semester == Semester && joinedData.Student.Id == StudentId)
            .GroupBy(joinedData => joinedData.Subject.SubjectName)
            .Select(g => new StudentAttendanceViewModel
            {
                Subject = g.Key,
                PresentCount = g.Count(a => a.Attendance.isPresent),
                AbsentCount = g.Count(a => !a.Attendance.isPresent),
                TotalCount = g.Count()
            })
            .ToList();


            IEnumerable<StudentAttendanceViewModel> result = query.ToList();

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
