using AutoMapper;
using EmployeeApi.DTOs;
using EmployeeApi.Interfaces;
using EmployeeApi.Models;
using EmployeeApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public EmployeeController(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        [HttpGet("View-All-Employees")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<EmployeeDto>>(employees));
        }

        [HttpGet("View-Employee-Details")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound();
            return Ok(_mapper.Map<EmployeeDto>(employee));
        }

        [HttpPost("Add-New-Employees")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee(CreateEmployeeDto createDto)
        {
            var employee = _mapper.Map<Employee>(createDto);
            var createdEmployee = await _employeeService.CreateAsync(employee);

            var returnDto = _mapper.Map<EmployeeDto>(createdEmployee);
            return CreatedAtAction(nameof(GetEmployee), new { id = createdEmployee.Id }, returnDto);
        }

        [HttpPut("Update-Employee_Details")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEmployee(int id, CreateEmployeeDto updateDto)
        {
            var employee = _mapper.Map<Employee>(updateDto);

            var result = await _employeeService.UpdateAsync(id, employee);
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpDelete("Delete-Employee-Details")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await _employeeService.DeleteAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpGet("High-Salary")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetHighEarners([FromQuery] decimal minSalary)
        {
            var employees = await _employeeService.GetHighEarnersAsync(minSalary);
            return Ok(_mapper.Map<IEnumerable<EmployeeDto>>(employees));
        }

        [HttpGet("Stats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<DepartmentStatsDto>>> GetDepartmentStats()
        {
            var stats = await _employeeService.GetDepartmentStatsAsync();
            return Ok(stats);
        }
    }
}