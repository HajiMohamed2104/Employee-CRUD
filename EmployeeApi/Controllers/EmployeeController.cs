using AutoMapper;
using EmployeeApi.Data;
using EmployeeApi.DTOs;
using EmployeeApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public EmployeeController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            try
            {
                var employees = await _context.Employees.ToListAsync();
                var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
                return Ok(employeeDtos);
            }
            catch (Exception ex)
            {
                // Basic Exception Handling
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("{Employee View Id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound($"Employee with Id {id} not found.");
            }

            return Ok(_mapper.Map<EmployeeDto>(employee));
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee(CreateEmployeeDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // Map DTO to Entity
                var employee = _mapper.Map<Employee>(createDto);

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                // Map back to DTO for response
                var returnDto = _mapper.Map<EmployeeDto>(employee);

                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, returnDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error creating employee record.");
            }
        }

        [HttpPut("{Update Employee Details}")]
        public async Task<IActionResult> UpdateEmployee(int id, CreateEmployeeDto updateDto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            _mapper.Map(updateDto, employee);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Concurrency error occurred.");
            }

            return NoContent();
        }

        [HttpDelete("{Delete Employee Details}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("high-salary")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetHighEarners([FromQuery] decimal minSalary)
        {
            var highEarners = await _context.Employees
                .Where(e => e.Salary >= minSalary)
                .OrderByDescending(e => e.Salary)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<EmployeeDto>>(highEarners));
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDepartmentStats()
        {
            var stats = await _context.Employees
                .GroupBy(e => e.Department)
                .Select(g => new
                {
                    Department = g.Key,
                    Count = g.Count(),
                    TotalSalary = g.Sum(e => e.Salary),
                    AverageSalary = g.Average(e => e.Salary)
                })
                .ToListAsync();

            return Ok(stats);
        }
    }
}