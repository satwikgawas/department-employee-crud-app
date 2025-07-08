using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeDepartmentCRUDApp.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Employee name is required")]
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        [EmailAddress]
        [Display(Name = "Employee Email")]
        [Required(ErrorMessage ="Employee email is required")]
        public string EmployeeEmail { get; set; }
        [Phone]
        [Required(ErrorMessage ="Employee phone no is required")]
        [Display(Name = "Employee Phone")]
        public string EmployeePhone { get; set; }
        [Required(ErrorMessage ="Employee position is required")]
        [Display(Name = "Employee Position")]
        public string EmployeePosition { get; set; }
        [ForeignKey("Department")]
        [Required(ErrorMessage ="Please select department")]
        [Display(Name = "Employee Department")]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        [ForeignKey("Organization")]
        [Required(ErrorMessage = "Please select organization")]
        [Display(Name = "Employee Organization")]
        public int OrganizationId { get; set; }
        public Organization? Organization { get; set; }
    }
}
