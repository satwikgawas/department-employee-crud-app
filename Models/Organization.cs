using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeDepartmentCRUDApp.Models
{
    public class Organization
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="Organization Name")]
        [Required(ErrorMessage ="Organization name is required")]
        public string OrganizationName { get; set; }
        [Display(Name ="Organization Email")]
        [Required(ErrorMessage ="Organization email is required")]
        [EmailAddress]
        public string OrganizationEmail { get; set; }
        [Display(Name ="Organization GST No")]
        [Required(ErrorMessage ="Organization GST No is required")]
        [RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$", ErrorMessage= "Invalid GST No format.It should follow the standard 15-character GSTIN structure.")]
        public string OrganizationGSTNo { get; set; }
        [Display(Name ="Organization Address")]
        [Required(ErrorMessage ="Organization address is required")]
        public string OrganizationAddress { get; set; }
        [Display(Name ="Organization Published Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage ="Please select organization published date")]
        public DateTime OrganizationPublishedAt { get; set; }
        public ICollection<Department>? Departments { get; set; }
    }
}
