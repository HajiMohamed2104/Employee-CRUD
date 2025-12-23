using EmployeeApi.Data;
using EmployeeApi.DTOs;
using EmployeeApi.Interfaces;
using EmployeeApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace EmployeeApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(AppDbContext context, ILogger<EmployeeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all employees");
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching employee with ID: {Id}", id);
            return await _context.Employees.FindAsync(id);
        }

        public async Task<Employee> CreateAsync(Employee employee)
        {
            _logger.LogInformation("Creating new employee: {Name}", employee.FirstName);
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> UpdateAsync(int id, Employee employee)
        {
            _logger.LogInformation("Updating employee with ID: {Id}", id);
            var existing = await _context.Employees.FindAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Employee with ID: {Id} not found for update", id);
                return false;
            }

            existing.FirstName = employee.FirstName;
            existing.LastName = employee.LastName;
            existing.Email = employee.Email;
            existing.Department = employee.Department;
            existing.Salary = employee.Salary;
            existing.DateJoined = employee.DateJoined;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting employee with ID: {Id}", id);
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID: {Id} not found for deletion", id);
                return false;
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Employee>> GetHighEarnersAsync(decimal minSalary, decimal maxSalary)
        {
            _logger.LogInformation("Fetching high earners with salary between {MinSalary} and {MaxSalary}", minSalary, maxSalary);
            return await _context.Employees
                .Where(e => e.Salary >= minSalary && e.Salary <= maxSalary)
                .OrderByDescending(e => e.Salary)
                .ToListAsync();
        }

        public async Task<IEnumerable<DepartmentStatsDto>> GetDepartmentStatsAsync()
        {
            _logger.LogInformation("Fetching department statistics");
            return await _context.Employees
                .GroupBy(e => e.Department)
                .Select(g => new DepartmentStatsDto
                {
                    Department = g.Key,
                    Count = g.Count(),
                    TotalSalary = g.Sum(e => e.Salary),
                    AverageSalary = g.Average(e => e.Salary)
                })
                .ToListAsync();
        }
    }
}