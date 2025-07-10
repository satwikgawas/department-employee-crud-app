using System.ComponentModel.DataAnnotations;

namespace EmployeeDepartmentCRUDApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="Username")]
        [Required(ErrorMessage ="Username is required")]
        public string Username { get; set; }
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public ICollection<UserRole>? UserRoles { get; set; }
    }
}
