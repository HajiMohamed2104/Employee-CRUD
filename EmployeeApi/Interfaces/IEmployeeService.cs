using EmployeeApi.DTOs;
using EmployeeApi.Models;
namespace EmployeeApi.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee> CreateAsync(Employee employee);
        Task<bool> UpdateAsync(int id, Employee employee);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Employee>> GetHighEarnersAsync(decimal minSalary, decimal maxSalary);
        Task<IEnumerable<DepartmentStatsDto>> GetDepartmentStatsAsync();
    }
}
