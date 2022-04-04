using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Leave_Management_System.Models
{
    [MetadataType(typeof(EmployeeMetaData))]
    public partial class Employee
    {

    }

    public class EmployeeMetaData
    {
        [Display(Name ="اسم الموظف")]
        [Required(ErrorMessage ="الرجاء ادخال اسم الموظف")]
        public string Emp_Name { get; set; }
        [Display(Name = "اسم المستخدم")]
        [Required(ErrorMessage = "الرجاء ادخال اسم المستخدم")]
        public string User_Name { get; set; }
        [Display(Name = "كلمة المرور")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "الرجاء ادخال كلمة المرور")]
        public string Password { get; set; }
        [Display(Name = "عدد الاجازات السنوية")]
        public Nullable<int> Available_Y_Leave { get; set; }
        [Display(Name = "عدد الاجازات المرضية")]
        public Nullable<int> Avilable_P_Leave { get; set; }
        [Display(Name = "القسم")]
        //[Required(ErrorMessage = "الرجاء اختيار القسم")]
        public Nullable<int> Department { get; set; }
        [Display(Name = "الصلاحية")]
        [Required(ErrorMessage = "الرجاء اختيار القسم")]
        public int Role { get; set; }
        [Display(Name = "فرع المكتب")]
        [Required(ErrorMessage = "الرجاء اختيار فرع المكتب")]
        public int Office { get; set; }
        [Display(Name = "الهاتف")]
        [Required(ErrorMessage = "الرجاء ادخال رقم الهاتف")]
        public string Phone { get; set; }
    }
}