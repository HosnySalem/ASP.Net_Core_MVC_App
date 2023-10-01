using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SkyLine.Data;
using SkyLine.Models;

namespace SkyLine.Controllers
{
    public class EmployeesController : Controller
    {
        /* public IActionResult Index()
         {
             return View();
         }*/
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EmployeesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
       /* List<Employee> employees = new List<Employee>
    {
        new Employee { Id = 1, FullName = "Wael Mahmoud", Position = "Developer", Salary = 12500m, BirthDate = new DateTime(1998, 10, 25), JoinDateTime = new DateTime(2021, 7, 3), IsActive = true
        },
        new Employee{Id = 2, FullName = "Bahaa Ali", Position = "Tester", Salary = 6500, BirthDate = new DateTime(2000, 1, 17), JoinDateTime = new DateTime(2022, 3, 29), IsActive = true
        },
        new Employee
        {
            Id = 3, FullName = "Osama Mostafa", Position = "Senior Developer", Salary = 15000, BirthDate = new DateTime(1997, 2, 27), JoinDateTime = new DateTime(2020, 1, 22), IsActive = true
        },
        new Employee
        {
            Id = 4, FullName = "Ahmed Hassan", Position = "Developer", Salary = 10500, BirthDate = new DateTime(1999, 7, 21), JoinDateTime = new DateTime(2021, 3, 25),
            IsActive = true
        }
    };*/

        [HttpGet]
        public IActionResult GetIndexView(int deptId,string? search, string sortType, string sortOrder, int PageSize = 20 ,int PageNumber = 1)
        {
            ViewBag.AllDepartments = _context.Departments.ToList();
            ViewBag.SelectedDeptId = deptId;
            ViewBag.CurrentSearch = search;
            IQueryable<Employee> employees = _context.Employees.AsQueryable();
            if(deptId != 0)
            {
                employees = employees.Where(e=>e.DepartmentId == deptId);
            }
            if (string.IsNullOrEmpty(search) == false)
            {
                employees = employees.Where(e => e.FullName.Contains(search));
            }
            if(sortType == "FullName" && sortOrder == "asc")
            {
                employees = employees.OrderBy(e => e.FullName);

            }
            else if (sortType == "FullName" && sortOrder == "desc")
            {
                employees = employees.OrderByDescending(e => e.FullName);

            }
            else if (sortType == "Position" && sortOrder == "asc")
            {
                employees = employees.OrderBy(e => e.Position);

            }
            else if (sortType == "Position" && sortOrder == "desc")
            {
                employees = employees.OrderByDescending(e => e.Position);

            }
            if (PageSize > 50) PageSize = 50;
            if (PageSize < 1 ) PageSize = 1;
            if(PageNumber < 1 ) PageNumber = 1;
            employees = employees.Skip(PageSize * (PageNumber - 1)).Take(PageSize);
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;
            return View("Index", employees);
            
        }
        [HttpGet]
        public IActionResult GetDetailsView(int id)
        {
           
            Employee emp = _context.Employees.Include(e => e.Department).FirstOrDefault(e => e.Id == id);
              ViewBag.CurrentEmployee = emp;

            if (emp == null)
            {
                return NotFound();
            }
            else
            {
                return View("Details", emp);
            }
        }

        public string GreetVisitor()
        {
            return "Welcome to SkyLine!";
        }

        public string GreetUser(string name)
        {
            return "Hi " + name;
        }

        public string GetAge(string name, int birthYear)
        {
            int ageYears = DateTime.Now.Year - birthYear;
            return $"Hi {name}. You are {ageYears} years old.";
        }
        [HttpGet]
        public IActionResult GetCreateView()
        {
            ViewBag.AllDepartments = _context.Departments.ToList();
            return View("Create");
        }
        [HttpPost]
        public IActionResult AddNew(Employee emp,IFormFile? imageFormFile)
        {
            if(((emp.JoinDateTime -emp.BirthDate).Days / 365) < 18)
            {
                ModelState.AddModelError(string.Empty, "Illigal Hiring/Joining Age (Under 18 year)");
            }
           if(ModelState.IsValid)
            {
                if(imageFormFile == null)
                {
                    emp.ImagePath = "\\images\\No_Image_Available.png";
                }
                else
                {
                    Guid imgGuid = Guid.NewGuid();
                    string imgExtension =Path.GetExtension(imageFormFile.FileName);
                    string imgName = imgGuid + imgExtension;
                    emp.ImagePath = "\\images\\employees\\" + imgName;
                    string imgFullPath = _webHostEnvironment.WebRootPath + emp.ImagePath;
                    FileStream fileStream = new FileStream(imgFullPath, FileMode.Create);
                    imageFormFile.CopyTo(fileStream);
                    fileStream.Dispose();

                }
                _context.Employees.Add(emp);
                _context.SaveChanges();
                return RedirectToAction("GetIndexView");
            }
            else
            {
                ViewBag.AllDepartments = _context.Departments.ToList();
                return View("Create",emp);
            }
        }
        public IActionResult GetEditView(int id)
        {
            Employee emp = _context.Employees.Find(id);

            if (emp == null)
            {
                return NotFound();
            }
            else
            {
                ViewBag.AllDepartments = _context.Departments.ToList();
                return View("Edit", emp);
            }
        }

        public IActionResult EditCurrent(Employee emp,IFormFile? imageFormFile)
        {
            if (((emp.JoinDateTime - emp.BirthDate).Days / 365) < 18)
            {
                ModelState.AddModelError(string.Empty, "Illigal Hiring/Joining Age (Under 18 year)");
            }
            if (ModelState.IsValid)
            {
                if(imageFormFile != null)
                {
                    if (emp.ImagePath != "\\images\\No_Image_Available.png")
                    {
                        string imgPath = _webHostEnvironment
                            .WebRootPath + emp.ImagePath;
                        System.IO.File.Delete(imgPath);
                    }
                    Guid imgGuid = Guid.NewGuid();
                    string imgExtension = Path.GetExtension(imageFormFile.FileName);
                    string imgName = imgGuid + imgExtension;
                    emp.ImagePath = "\\images\\employees\\" + imgName;
                    string imgFullPath = _webHostEnvironment.WebRootPath + emp.ImagePath;
                    FileStream fileStream = new FileStream(imgFullPath, FileMode.Create);
                    imageFormFile.CopyTo(fileStream);
                    fileStream.Dispose();
                }


                _context.Employees.Update(emp);
                _context.SaveChanges();
                return RedirectToAction("GetIndexView");
            }
            else
            {
                ViewBag.AllDepartments = _context.Departments.ToList();
                return View("Edit", emp);
            }
        }
        public IActionResult GetDeleteView(int id)
        {
            Employee emp = _context.Employees.Include(e => e.Department).FirstOrDefault(e => e.Id == id);

            ViewBag.CurrentEmployee = emp;
            if (emp == null)
            {
                return NotFound();
            }
            else
            {
                return View("Delete", emp);
            }
        }
        public IActionResult DeleteCurrent(int id)
        {
            Employee emp = _context.Employees.Find(id);
            if (emp.ImagePath != "\\images\\No_Image_Available.png")
            {
                string imgPath = _webHostEnvironment
                    .WebRootPath + emp.ImagePath;
                if (System.IO.File.Exists(imgPath))
                {
                    System.IO.File.Delete(imgPath);
                }
            }
                _context.Employees.Remove(emp);
                _context.SaveChanges();
                return RedirectToAction("GetIndexView");
        }

    }
}
