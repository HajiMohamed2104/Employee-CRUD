using AutoMapper;
using EmployeeApi.DTOs;
using EmployeeApi.Interfaces;
using EmployeeApi.Models;
using EmployeeApi.Services;
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
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            try
            {
                var employees = await _employeeService.GetAllAsync();
                _logger.LogInformation("200 OK - GetEmployees - Employees fetched successfully");
                return Ok(_mapper.Map<IEnumerable<EmployeeDto>>(employees));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all employees.");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpGet("View-Employee-Details")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
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
                _logger.LogError(ex, "Error occurred while fetching employee with ID {Id}.", id);
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpPost("Add-New-Employees")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee(CreateEmployeeDto createDto)
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
                _logger.LogError(ex, "Error occurred while creating a new employee.");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpPut("Update-Employee-Details")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEmployee(int id, CreateEmployeeDto updateDto)
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
                _logger.LogError(ex, "Error occurred while updating employee with ID {Id}.", id);
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpDelete("Delete-Employee-Details")]
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
                _logger.LogError(ex, "Error occurred while deleting employee with ID {Id}.", id);
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpGet("High-Salary")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetHighEarners([FromQuery] decimal minSalary, [FromQuery] decimal maxSalary)
        {
            try
            {
                if (minSalary > maxSalary)
                {
                    return BadRequest("Minimum salary cannot be greater than maximum salary.");
                }

                var employees = await _employeeService.GetHighEarnersAsync(minSalary, maxSalary);
                _logger.LogInformation("200 OK - GetHighEarners");
                return Ok(_mapper.Map<IEnumerable<EmployeeDto>>(employees));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching high earners.");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpGet("Stats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<DepartmentStatsDto>>> GetDepartmentStats()
        {
            try
            {
                var stats = await _employeeService.GetDepartmentStatsAsync();
                _logger.LogInformation("200 OK - GetDepartmentStats");
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching department statistics.");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }
    }
}