using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkyLine.Data;
using SkyLine.Models;

namespace SkyLine.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DepartmentsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetIndexView(string? search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return View("Index", _context.Departments.ToList());
            }
            else
            {
                ViewBag.CurrentSearch = search;
                return View("Index", _context.Departments.Where(e => e.Name.Contains(search)).ToList());
            }

        }
        public IActionResult AddView()
        {
            ViewBag.AllEmployees = _context.Employees.ToList();
            return View("Create");
        }

        public IActionResult AddDepartment(Department department, IFormFile? imageFormFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFormFile == null)
                {
                    department.ImagePath = "\\images\\No_Image_Available.png";
                }
                else
                {
                    Guid imgGuid = Guid.NewGuid();
                    string imgExtension = Path.GetExtension(imageFormFile.FileName);
                    string imgName = imgGuid + imgExtension;
                    department.ImagePath = "\\images\\Department\\" + imgName;
                    string imgFullPath = _webHostEnvironment.WebRootPath + department.ImagePath;
                    FileStream fileStream = new FileStream(imgFullPath, FileMode.Create);
                    imageFormFile.CopyTo(fileStream);
                    fileStream.Dispose();
                }
                this. _context.Add(department);
               this. _context.SaveChanges();
                return RedirectToAction("GetIndexView");
            }
            else
            {
                ViewBag.AllEmployees = _context.Employees.ToList();
                return View("Create", department);
            }
        }
        public IActionResult GetEditView(int id)
        {
            Department? dept = this._context.Departments.Find(id);

            if (dept == null)
            {
                return NotFound();
            }
            else
            {
                ViewBag.AllEmployees = _context.Employees.ToList();
                return View("Edit", dept);
            }
        }

        public IActionResult EditCurrent(Department department)
        {
            if (ModelState.IsValid)
            {
                this._context.Update(department);
                this._context.SaveChanges();
                return RedirectToAction("GetIndexView");
            }
            else
            {
                ViewBag.AllEmployees = _context.Employees.ToList();
                return View("Create", department);
            }
        }
        public IActionResult GetDeleteView(int id)
        {
            Department? dept = this._context.Departments.Include(d => d.Employees).FirstOrDefault(d => d.Id == id);

            ViewBag.CurrentEmployee = dept;
            if (dept == null)
            {
                return NotFound();
            }
            else
            {
                return View("Delete", dept);
            }
        }
        public IActionResult DeleteCurrent(Department department)
        {
            _context.Departments.Remove(department);
            _context.SaveChanges();
            return RedirectToAction("GetIndexView");
        }

        public IActionResult GetDetailsView(int id)
        {
            Department? dept =this._context.Departments.Include(d => d.Employees).FirstOrDefault(d => d.Id == id);
            ViewBag.CurrentEmployee = dept;
            if (dept == null) return NotFound();
            return View("Details",dept);
        }

        public String GreetDep()
        {
            return "Welcome to Skyline!";
        }
        public String GreetEmp(string name)
        {
            return $"Hi{name}";
        }
    }
}
