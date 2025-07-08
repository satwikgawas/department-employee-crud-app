using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [ForeignKey("Organization")]
        [Required(ErrorMessage = "Please select organization")]
        [Display(Name = "Department Organization")]
        public int OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        public ICollection<Employee>? Employees { get; set; }
    }
}
