using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentPlus.Infraestructure.Persistence;

namespace TalentPlus.Web.Controllers;

    [Authorize] 
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            //  I fetch general counters
            var totalEmployees = await _context.Employees.CountAsync();
            var activeEmployees = await _context.Employees.CountAsync(e => e.Status == "Activo" || e.Status == "Active");
            
            //  I calculate the total monthly payroll (Sum of salaries)
            var totalPayroll = await _context.Employees.SumAsync(e => e.Salary);

            //  I group employees by Department to see distribution
            var employeesByDept = await _context.Employees
                .Include(e => e.Department)
                .GroupBy(e => e.Department!.Name)
                .Select(g => new 
                { 
                    Department = g.Key, 
                    Count = g.Count(),
                    TotalSalary = g.Sum(e => e.Salary)
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            //  I group by Contract Type or Status
            var employeesByStatus = await _context.Employees
                .GroupBy(e => e.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            // I return the complete intelligence report
            return Ok(new
            {
                General = new 
                { 
                    TotalEmployees = totalEmployees, 
                    ActiveCount = activeEmployees, 
                    InactiveCount = totalEmployees - activeEmployees,
                    MonthlyPayrollCost = totalPayroll
                },
                ByDepartment = employeesByDept,
                ByStatus = employeesByStatus
            });
        }
    }