using Microsoft.AspNetCore.Mvc;
using MVCTest.Models;

namespace MVCTest.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _db;

        public StudentController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Student> objStudentList = _db.Students.ToList(); 
            return View(objStudentList);
        }
    }
}
