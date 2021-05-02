using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebDataAnnotation.Models
{
    public class Employee
    {
        [DisplayName("ID")]
        [Required(ErrorMessage = "Id Required")]
        public int EmployeeID { get; set; }
        [DisplayName("NAME")]
        [Required(ErrorMessage = "Name Required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Name must be Between 20 and 5")]
        public string EmployeeName { get; set; }
        [DisplayName("AGE")]
        [Required(ErrorMessage = "Age Required")]
        [Range(10, 30, ErrorMessage = "Age Between 10 and 30")]
        public int? EmployeeAge { get; set; }
        [DisplayName("GENDER")]
        [Required(ErrorMessage = "Gender Required")]
        public string EmployeeGender { get; set; }
        [DisplayName("EMAIL")]
        [Required(ErrorMessage = "Email Required")]
        [RegularExpression("^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$",ErrorMessage ="Invalid Email!!!!")]
        public string EmployeeEmail { get; set; }
        [DisplayName("PASSWORD")]
        [Required(ErrorMessage ="Password must be required")]
        [RegularExpression(@"(?=^.{8,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage ="Inavlid Password")]
        [DataType(DataType.Password)]
        public string EmployeePassword { get; set; }
        [DisplayName("CONFIRM PASSWORD")]
        [Required(ErrorMessage = "ComparePassword must be required")]
        [Compare("EmployeePassword",ErrorMessage ="Password donot Match")]
        [DataType(DataType.Password)]
        public string CompareEmployeePassword { get; set; }
        [DisplayName("ORGANIZATION NAME")]
        [ReadOnly(true)]
        public string EmployeeOrganization { get; set; }
        [DisplayName("ADRESS")]
        [Required(ErrorMessage = "Address must be required")]
        [DataType(DataType.MultilineText)]
        public string EmployeeAddress { get; set; }
        [DisplayName("JOINING DATE")]
        [Required(ErrorMessage = "date must be required")]
        [DataType(DataType.Date)]
        public string EmployeeJoiningDate { get; set; }
        [DisplayName("JOINING DATE")]
        [Required(ErrorMessage = "Time must be required")]
        [DataType(DataType.Time)]
        public string EmployeeJoiningTime { get; set; }

    }
}