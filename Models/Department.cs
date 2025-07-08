using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmployeeDepartmentCRUDApp.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Department name is required")]
        [Display(Name ="Department Name")]
        public string DepartmentName { get; set; }
        public ICollection<Employee>? Employees { get; set; }
    }
}
