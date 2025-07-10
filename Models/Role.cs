using System.ComponentModel.DataAnnotations;

namespace EmployeeDepartmentCRUDApp.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Display(Name="Role Name")]
        [Required(ErrorMessage ="Role name is required")]
        public string Name { get; set; }

        public ICollection<UserRole>? UserRoles { get; set; }
    }
}
