using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Leave_Management_System.Models;
using System.Net;
using System.Data.Entity;
using System.IO;
using System.Web.Security;

namespace Leave_Management_System.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly LMS_DBEntities db = new LMS_DBEntities();
        // GET: Employees
        public ActionResult Index()
        {

            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            Session["loginRole"] = null;
            Session["logedUser"] = null;
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login([Bind(Include = "User_Name,Password")] Employee emp)
        {
            var isEmployee = db.Employees.Where(e => e.User_Name == emp.User_Name && e.Password == emp.Password).FirstOrDefault();
            if (isEmployee != null)
            {

                Session["loginRole"] = isEmployee.Role1.Role_Type;
                Session["currentUser"] = isEmployee.Emp_Id;
                Session["logedUser"] = isEmployee.Emp_Name;
                FormsAuthentication.SetAuthCookie(emp.User_Name, false);
                return RedirectToAction("Index", "Employees");
            }
            else
            {
                ModelState.AddModelError("", "تجقق من معلومات الدخول");
                return View(emp);
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Employees");
        }

        [Authorize(Roles = "باحث , رئيس قسم, مدير")]
        public ActionResult LeaveRequest()
        {
            List<Leave_Status> Ls = db.Leave_Status.ToList();
            SelectList listOfLs = new SelectList(Ls, "Ls_Id", "Ls_Name");
            ViewBag.Ls = listOfLs;

            List<Leave_Type> Leave_Type = db.Leave_Type.ToList();
            SelectList listOfLt = new SelectList(Leave_Type, "Lt_Id", "Lt_Name");
            ViewBag.Leave_Type = listOfLt;

            return View();
        }
        [Authorize(Roles = "باحث , رئيس قسم, مدير")]
        [HttpPost]
        public ActionResult LeaveRequest(Leave leave)
        {
            int id = (int)Session["currentUser"];
            var currentUser = db.Employees.Find(Session["currentUser"]);
            var isValid = db.Leaves.Where(l => l.Leave_State == 6 && l.Employee_Id == id && l.Leave_Type == 2 && l.medicalInspectionImgPath == null).ToList().FirstOrDefault();
            if (isValid != null)
            {
                ViewBag.ImageMessage = "لديك اجازة مرضية بدون وثيقة معاينة الطبية";
                List<Leave_Type> Leave_Type = db.Leave_Type.ToList();
                SelectList listOfLt = new SelectList(Leave_Type, "Lt_Id", "Lt_Name");
                ViewBag.Leave_Type = listOfLt;
                return View(leave);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (currentUser.Role == 4)//=>باحث
                    {
                        leave.Leave_State = 1;//=>قيد الاجراء
                    }
                    if (currentUser.Role == 3)//=>رئيس قسم
                    {
                        leave.Leave_State = 2;//=>موافقة رئيس قسم
                    }
                    if (currentUser.Role == 2)//=>مدير
                    {
                        leave.Leave_State = 4;//=>موفقة مدير
                    }
                    leave.Employee_Id = (int)Session["currentUser"];
                    db.Leaves.Add(leave);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    List<Leave_Type> Leave_Type = db.Leave_Type.ToList();
                    SelectList listOfLt = new SelectList(Leave_Type, "Lt_Id", "Lt_Name");
                    ViewBag.Leave_Type = listOfLt;
                    return View(leave);
                }
            }
        }

        public JsonResult HavePLeave()
        {
            var id = (int)Session["currentUser"];
            var numberOfPLeave = db.Employees.Where(e => e.Emp_Id == id).ToList().FirstOrDefault();
            return Json(data: new { success = true, data = numberOfPLeave.Avilable_P_Leave },JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "باحث , رئيس قسم, مدير")]
        public ActionResult MyLeaves()
        {
            var id = (int)Session["currentUser"];
            var allLeaves = db.Leaves.Where(l => l.Employee_Id == id && l.Leave_State != 1).ToList();
            var currentEmp = db.Employees.Where(e => e.Emp_Id == id).ToList().FirstOrDefault();
            ViewData["numberOfYLeave"] = currentEmp.Available_Y_Leave;
            ViewData["numberOfPLeave"] = currentEmp.Avilable_P_Leave;
            ViewData["numberOfLeaves"] = allLeaves.Count();
            if (allLeaves != null)
            {
                return View(allLeaves);
            }
            else
            {
                return RedirectToAction("Index", "Employees");
            }
        }
        [Authorize(Roles = "رئيس قسم")]
        public ActionResult ResearcherLeaveRequest()
        {
            var currentUser = db.Employees.Find(Session["currentUser"]);
            int deptId = (int)currentUser.Department;
            int officeId = (int)currentUser.Office;
            //اعرضهم يكونو في نفس القسم
            var allResearcherLeaveRequest = db.Leaves.Where(l => l.Leave_State == 1
                                                                && l.Employee.Role == 4
                                                                && l.Employee.Department == deptId
                                                                && l.Employee.Office == officeId).ToList();

            return View(allResearcherLeaveRequest);
        }

        [Authorize(Roles = "رئيس قسم")]
        //show details of leave for head department
        [HttpGet]
        public ActionResult ResLeaveReq(int? id)
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
        [Authorize(Roles = "رئيس قسم")]
        //accept or reject the leave for head department
        [HttpPost]
        public ActionResult ResLeaveReq([Bind(Include = "Leave_Id,Leave_State")] Leave leavee)
        {
            var beforReq = db.Leaves.AsNoTracking().Where(l => l.Leave_Id == leavee.Leave_Id).ToList().FirstOrDefault();
            leavee.From_Date = beforReq.From_Date;
            leavee.To_Date = beforReq.To_Date;
            leavee.Reson = beforReq.Reson;
            leavee.Employee_Id = beforReq.Employee_Id;
            leavee.Leave_Type = beforReq.Leave_Type;
            if (ModelState.IsValid)
            {
                db.Entry(leavee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(leavee);
        }

        //manager
        [Authorize(Roles = "مدير")]

        public ActionResult EmpLeaveRequest()
        {
            var currentUser = db.Employees.Find(Session["currentUser"]);
            int officeId = (int)currentUser.Office;
            var allEmpLeaveRequest = db.Leaves.Where(l => l.Leave_State == 2 &&
                                                    (l.Employee.Role == 4 || l.Employee.Role == 3)
                                                    && l.Employee.Office == officeId).ToList();
            return View(allEmpLeaveRequest);
        }

        [Authorize(Roles = "مدير")]
        //show details of leave for manager
        [HttpGet]
        public ActionResult EmpLeaveReq(int? id)
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

        [Authorize(Roles = "مدير")]
        //accept or reject the leave for Manager
        [HttpPost]
        public ActionResult EmpLeaveReq([Bind(Include = "Leave_Id,Leave_State")] Leave leavee)
        {

            var beforReq = db.Leaves.AsNoTracking().Where(l => l.Leave_Id == leavee.Leave_Id).ToList().FirstOrDefault();
            var befoorRec = db.Employees.Find(beforReq.Employee_Id);// db.Employees.AsNoTracking().Where(e => e.Emp_Id == beforReq.Employee_Id).ToList().FirstOrDefault();//حتى تنقص اجازاته
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

        [Authorize(Roles = "باحث , رئيس قسم, مدير")]
        public ActionResult medicalInspection(int? id)
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
            //id = test();
            //if (needImg==null)
            //{
            //    ViewBag.message = "لا يوجد  لديك اجازة مرضية بحاجة الى ارفاق معاينة طبية";
            //}
            //ViewData["model"] = /*needImg*/;
            //return View();
        }
        //public int test()
        //{
        //    var id = (int)Session["currentUser"];
        //    var needImg = db.Leaves.Where(l => l.Leave_Status.Ls_Id == 6 && l.Leave_Type1.Lt_Id == 2 && l.Employee_Id == id).ToList().FirstOrDefault();
        //    int leaveId =needImg.Leave_Id;
        //    return RedirectToAction("medicalInspection",leaveId);
        //}

        [Authorize(Roles = "باحث , رئيس قسم, مدير")]
        [HttpPost]
        public ActionResult medicalInspection([Bind(Include = "Leave_Id,medicalInspectionImgPath")] Leave leavee, HttpPostedFileBase ImageUpload)
        {
            var beforReq = db.Leaves.AsNoTracking().Where(l => l.Leave_Id == leavee.Leave_Id).ToList().FirstOrDefault();
            leavee.From_Date = beforReq.From_Date;
            leavee.To_Date = beforReq.To_Date;
            leavee.Reson = beforReq.Reson;
            leavee.Employee_Id = beforReq.Employee_Id;
            leavee.Leave_Type = beforReq.Leave_Type;
            leavee.Leave_State = beforReq.Leave_State;
            if (ModelState.IsValid)
            {
                string path = "";
                if (ImageUpload.FileName.Length > 0)
                {
                    path = "~/Images/" + DateTime.Now.Year.ToString() + Path.GetFileName(ImageUpload.FileName);
                    ImageUpload.SaveAs(Server.MapPath(path));
                }
                leavee.medicalInspectionImgPath = path;
                db.Entry(leavee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(leavee);
        }


        public JsonResult ResearcherWithoutmedicalInspectionImg()
        {
            var id = (int)Session["currentUser"];
            var currentUser = db.Employees.Find(id);
            var emp = db.Leaves.Where(l => l.medicalInspectionImgPath == null && l.Employee.Department1.Dep_Id == currentUser.Department && l.Employee.Office1.Office_Id == currentUser.Office&&l.Leave_State==6&&l.Leave_Type==2).ToList().FirstOrDefault();
            if (emp!=null)
            {
                return Json(data: new { success = true, data = emp.Employee.Emp_Name }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(data: new { success = true, data = "لا يوجد باحثين" }, JsonRequestBehavior.AllowGet);

            }
        }
    }
}