using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Leave_Management_System.Models;
using Rotativa.MVC;

namespace Leave_Management_System.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly LMS_DBEntities db = new LMS_DBEntities();
        // GET: Admin
        public ActionResult Index()
        {
            var tables = new EmployeeViewModel
            {
                Employees = db.Employees.ToList(),
                Departments = db.Departments.ToList(),
                Roles = db.Roles.ToList(),
                Offices = db.Offices.ToList()

            };
            return View(tables);
        }
  
        [Authorize(Roles = "ادمن")]
        public ActionResult AddEmp()
        {
            //var tables = new EmployeeViewModel
            //{
            //    Employees = db.Employees.ToList(),
            //    Departments = db.Departments.ToList(),
            //    Roles = db.Roles.ToList(),
            //    Offices = db.Offices.ToList()

            //};
            List<Office> off = db.Offices.ToList();
            SelectList listOfOf = new  SelectList(off, "Office_Id", "Office_Name");
            ViewBag.off = listOfOf;

            List<Role> roles = db.Roles.ToList();
            SelectList listOfRoles = new SelectList(roles, "Role_Id", "Role_Type");
            ViewBag.permissens = listOfRoles;

            List<Department> dept = db.Departments.ToList();
            SelectList listOfDept = new SelectList(dept, "Dep_Id", "Dep_Name");
            ViewBag.departments = listOfDept;


            return View();
        }
        [Authorize(Roles = "ادمن")]
        [HttpPost]
        public ActionResult AddEmp([Bind(Exclude = "Available_Y_Leave,Avilable_P_Leave")] Employee emp)
        {
            emp.Available_Y_Leave = 20;
            emp.Avilable_P_Leave = 7;
            
            if (ModelState.IsValid)
                {
                    db.Employees.Add(emp);
                    db.SaveChanges();
            }
            else
            {
                List<Office> off = db.Offices.ToList();
                SelectList listOfOf = new SelectList(off, "Office_Id", "Office_Name");
                ViewBag.off = listOfOf;

                List<Role> roles = db.Roles.ToList();
                SelectList listOfRoles = new SelectList(roles, "Role_Id", "Role_Type");
                ViewBag.permissens = listOfRoles;

                List<Department> dept = db.Departments.ToList();
                SelectList listOfDept = new SelectList(dept, "Dep_Id", "Dep_Name");
                ViewBag.departments = listOfDept;
                return View(emp);
            }
                return RedirectToAction("Index");

        }

        [Authorize(Roles = "ادمن")]
        public ActionResult ManagersLeaveRequest()
        {
            //var currentUser = db.Employees.Find(Session["currentUser"]);
            //int officeId = (int)currentUser.Office;
            var allManagersLeaveRequest = db.Leaves.Where(l => l.Leave_State == 4 &&l.Employee.Role == 2).ToList();
            return View(allManagersLeaveRequest);
        }

        [Authorize(Roles = "ادمن")]
        //show details of leave for Admin
        [HttpGet]
        public ActionResult ManagersLeaveReq(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var existingLeve = db.Leaves.Find(id);
            if (existingLeve == null)
            {
                return HttpNotFound();
            }
            return View(existingLeve);
        }
        [Authorize(Roles = "ادمن")]
        //accept or reject the leave for Admin
        [HttpPost]
        public ActionResult ManagersLeaveReq([Bind(Include = "Leave_Id,Leave_State")] Leave leavee)
        {

            var beforReq = db.Leaves.AsNoTracking().Where(l => l.Leave_Id == leavee.Leave_Id).ToList().FirstOrDefault();
            var befoorRec = db.Employees.AsNoTracking().Where(e => e.Emp_Id == beforReq.Employee_Id).ToList().FirstOrDefault();//حتى تنقص اجازاته
            leavee.From_Date = beforReq.From_Date;
            leavee.To_Date = beforReq.To_Date;
            leavee.Reson = beforReq.Reson;
            leavee.Employee_Id = beforReq.Employee_Id;
            leavee.Leave_Type = beforReq.Leave_Type;
            if (leavee.Leave_State == 6)
            {
                TimeSpan numberOfLeaves = beforReq.To_Date - beforReq.From_Date;
                double days = Convert.ToDouble(numberOfLeaves.TotalDays);
                if (beforReq.Leave_Type1.Lt_Id == 1)
                {
                    var currentYLeave = beforReq.Employee.Available_Y_Leave;
                    befoorRec.Available_Y_Leave = currentYLeave - (int)days;
                }
                if (beforReq.Leave_Type1.Lt_Id == 2)
                {
                    int currentPLeave = (int)beforReq.Employee.Avilable_P_Leave;
                    int yourLeave = currentPLeave - (int)days;
                    befoorRec.Avilable_P_Leave = yourLeave;
                }
            }
            if (ModelState.IsValid)
            {
                db.Entry(leavee).State = EntityState.Modified;
                db.Entry(befoorRec).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(leavee);
        }

        [Authorize(Roles = "ادمن")]
        public ActionResult EmpDetails()
        {

                ViewBag.Message = "Your contact page.";

                return View(db.Employees.ToList());

        }

        [Authorize(Roles = "ادمن")]
        public ActionResult PrintRep()
        {
            return View(db.Employees.ToList());

        }

        [Authorize(Roles = "ادمن")]
        public ActionResult PrintReport()
        {
            return new ActionAsPdf("PrintRep");
        }
    }
}