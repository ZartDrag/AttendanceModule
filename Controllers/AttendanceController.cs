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
            var query = from student in _db.Students
                         join attendance in _db.Attendances on student.Id equals attendance.StudentId into studentAttendances
                         from att in studentAttendances
                         where att.SubjectId == model.SubjectId &&
                               student.Section == model.Section &&
                               att.Date >= model.SelectedDate &&
                               att.Date <= model.SelectedEndDate
                         group att by new { student.enrollmentNo, student.Name } into g
                         select new StudentAttendanceViewModel
                         {
                             enrollmentNo = g.Key.enrollmentNo,
                             Name = g.Key.Name,
                             PresentCount = g.Count(a => a.isPresent),
                             AbsentCount = g.Count(a => !a.isPresent),
                             TotalCount = g.Count()
                         };

            var query1 = from subject in _db.Subjects
                         where subject.Id == model.SubjectId
                         select subject.SubjectName;

            Tuple<string, int, IEnumerable<StudentAttendanceViewModel>> trip = new Tuple<string, int, IEnumerable<StudentAttendanceViewModel>>(query1.FirstOrDefault(), model.Section, query.ToList());

            return View(trip);
        }

        public IActionResult ViewStudentNext(int StudentId, int Semester)
        {
            var query = from attendance in _db.Attendances
                        join student in _db.Students
                        on attendance.StudentId equals student.Id
                        join subject in _db.Subjects
                        on attendance.SubjectId equals subject.Id
                        where subject.Semester == Semester && student.Id == StudentId
                        group attendance by subject.SubjectName into g
                        select new StudentAttendanceViewModel
                        {
                            Subject = g.Key, 
                            PresentCount = g.Count(a => a.isPresent),
                            AbsentCount = g.Count(a => !a.isPresent),
                            TotalCount = g.Count()
                        };


            IEnumerable<StudentAttendanceViewModel> result = query.ToList();

            float totalPresent = 0, totalAbsent = 0;
            foreach (var attendance in result)
            {
                attendance.Percentage = attendance.PresentCount * 100 / (attendance.PresentCount + attendance.AbsentCount);
                totalPresent += attendance.PresentCount;
                totalAbsent += attendance.AbsentCount;
            }

            float totalPercentage = totalPresent * 100 / (totalPresent + totalAbsent);

            var query1 = from student in _db.Students
                         where student.Id == StudentId
                         select new Student
                         {
                             Name = student.Name,
                             enrollmentNo = student.enrollmentNo,
                             Section = student.Section
                         };

            Tuple<Student, float, IEnumerable<StudentAttendanceViewModel>> trip = new Tuple<Student, float, IEnumerable<StudentAttendanceViewModel>>(query1.FirstOrDefault(), totalPercentage, result);

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
