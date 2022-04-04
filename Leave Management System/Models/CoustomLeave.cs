using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Leave_Management_System.Coustom_Validation;
namespace Leave_Management_System.Models
{
    [MetadataType(typeof(LeaveMetaData))]
    public partial class Leave
    {

    }
    public class LeaveMetaData
    {
        [Display(Name = "من تاريخ")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage ="الرجاء اختيار تاريخ بدء الاجازة")]
        //[CorrectDate]
        public System.DateTime From_Date { get; set; }
        [Display(Name = "إلى تاريخ")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "الرجاء اختيار تاريخ إنتهاء الاجازة")]
        //[CorrectDate]
        public System.DateTime To_Date { get; set; }
        [Display(Name = "السبب")]
        public string Reson { get; set; }
        [Display(Name = "حالة الاجازة")]
        [Required(ErrorMessage = "الرجاء حالة الاجازة")]
        public int Leave_State { get; set; }
        [Display(Name = "نوع الاجازة")]
        [Required(ErrorMessage = "الرجاء اختيار نوع الاجازة")]
        public int Leave_Type { get; set; }
        [Display(Name = "صورة المعاينة الطبية")]
        //[Required(ErrorMessage = "الرجاء ارفاق صورة المعاينة الطبية")]
        public string medicalInspectionImgPath { get; set; }
    }
}