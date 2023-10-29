using Microsoft.AspNetCore.Mvc;
using MVCTest.Models;

namespace MVCTest.Controllers
{
    public class SubjectController : Controller
    {
        private readonly ApplicationDbContext _db;
        public SubjectController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Subject> objStudentList = _db.Subjects.ToList();
            return View(objStudentList);
        }
    }
}
