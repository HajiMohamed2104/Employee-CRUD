using AutoMapper;
using EmployeeApi.DTOs;
using EmployeeApi.Interfaces;
using EmployeeApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IEmployeeService employeeService, IMapper mapper, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("View-All-Employees")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var employees = await _employeeService.GetAllAsync();
                _logger.LogInformation("200 OK - GetEmployees - Employees fetched successfully");
                return Ok(_mapper.Map<IEnumerable<EmployeeDto>>(employees));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "500 Internal Server Error - GetEmployees");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("View-Employee-Details/{id:int}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            try
            {
                var employee = await _employeeService.GetByIdAsync(id);
                if (employee == null)
                {
                    _logger.LogWarning("404 NotFound - GetEmployee - EmployeeId: {Id}", id);
                    return NotFound("Employee not found");
                }
                _logger.LogInformation("200 OK - GetEmployee - EmployeeId: {Id}", id);
                return Ok(_mapper.Map<EmployeeDto>(employee));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "500 Internal Server Error - GetEmployee - EmployeeId: {Id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("Add-New-Employees")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeDto dto)
        {
            try
            {
                var employee = _mapper.Map<Employee>(dto);
                await _employeeService.CreateAsync(employee);
                _logger.LogInformation("200 OK - CreateEmployee - Employee created");
                return Ok("Employee created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "500 Internal Server Error - CreateEmployee");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("Update-Employee-Details/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEmployee(int id, CreateEmployeeDto dto)
        {
            try
            {
                var updated = await _employeeService.UpdateAsync(id, _mapper.Map<Employee>(dto));
                if (!updated)
                {
                    _logger.LogWarning("404 NotFound - UpdateEmployee - EmployeeId: {Id}", id);
                    return NotFound("Employee record not found");
                }
                _logger.LogInformation("200 OK - UpdateEmployee - EmployeeId: {Id}", id);
                return Ok("Employee updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "500 Internal Server Error - UpdateEmployee - EmployeeId: {Id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("Delete-Employee-Details/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var deleted = await _employeeService.DeleteAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning("404 NotFound - DeleteEmployee - EmployeeId: {Id}", id);
                    return NotFound("Employee not found");
                }
                _logger.LogInformation("200 OK - DeleteEmployee - EmployeeId: {Id}", id);
                return Ok("Employee deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "500 Internal Server Error - DeleteEmployee - EmployeeId: {Id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("High-Salary")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetHighEarners(decimal minSalary, decimal maxSalary)
        {
            try
            {
                if (minSalary > maxSalary)
                {
                    _logger.LogWarning("400 BadRequest - GetHighEarners - Invalid salary range");
                    return BadRequest("Minimum salary cannot be greater than maximum salary");
                }

                var employees = await _employeeService.GetHighEarnersAsync(minSalary, maxSalary);
                _logger.LogInformation("200 OK - GetHighEarners");
                return Ok(_mapper.Map<IEnumerable<EmployeeDto>>(employees));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "500 Internal Server Error - GetHighEarners");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("Stats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDepartmentStats()
        {
            try
            {
                var stats = await _employeeService.GetDepartmentStatsAsync();
                _logger.LogInformation("200 OK - GetDepartmentStats");
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "500 Internal Server Error - GetDepartmentStats");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}