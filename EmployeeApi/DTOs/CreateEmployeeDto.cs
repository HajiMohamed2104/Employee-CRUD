using System.ComponentModel.DataAnnotations;
namespace EmployeeApi.DTOs
{
    public class CreateEmployeeDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Department { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }
    }
}