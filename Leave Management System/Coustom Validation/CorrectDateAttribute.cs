using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Leave_Management_System.Coustom_Validation
{
    public class CorrectDateAttribute:ValidationAttribute
    {
        public CorrectDateAttribute() : base("الرجاء اختيار التاريخ بشكل صحيح")
        {

        }

        public override bool IsValid(object value)
        {
            DateTime dateProp = Convert.ToDateTime(value);
            if (dateProp < DateTime.Now)
                return false;
            else
                return true;
        }
    }
}