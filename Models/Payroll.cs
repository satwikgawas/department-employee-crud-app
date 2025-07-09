using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeDepartmentCRUDApp.Models
{
    public class Payroll
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Employee")]
        [Display(Name = "Employee")]
        [Required(ErrorMessage ="Please select employee")]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        [Required(ErrorMessage ="Employee basic salary is required")]
        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }
        [Required(ErrorMessage ="Employee HRA is required")]
        [Display(Name = "HRA")]
        public decimal HRA { get; set; }
        [Required(ErrorMessage ="Employee DA is required")]
        [Display(Name = "DA")]
        public decimal DA { get; set; }
        [Required(ErrorMessage ="Employee deduction is required")]
        [Display(Name = "Deductions")]
        public decimal Deductions { get; set; }
        [Display(Name = "Net Play")]
        public decimal NetPlay { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage ="Employee PayDate is required")]
        [Display(Name = "Pay Date")]
        public DateTime PayDate { get; set; }
        [Required(ErrorMessage ="Employee PayMonth is required")]
        [Display(Name = "Pay Month")]
        public string PayMonth { get; set; }
    }
}
