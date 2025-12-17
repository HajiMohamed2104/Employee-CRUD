namespace EmployeeApi.DTOs
{
    public class DepartmentStatsDto
    {
        public string Department { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalSalary { get; set; }
        public decimal AverageSalary { get; set; }
    }
}