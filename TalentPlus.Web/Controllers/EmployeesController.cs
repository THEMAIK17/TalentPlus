using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentPlus.Application.Interfaces;
using TalentPlus.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TalentPlus.Web.Controllers;
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;
    private readonly IPdfService _pdfService;

    public EmployeesController(IEmployeeService service, IPdfService pdfService)
    {
        _service = service;
        _pdfService = pdfService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(string? search)
    {
        // I request the list of employees from my service, passing the search term if available
        var employees = await _service.GetAllAsync(search);
        return Ok(employees);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        // I try to find the employee; if not found, I return a 404 Not Found response
        var employee = await _service.GetByIdAsync(id);
        if (employee == null) return NotFound();
        return Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Employee employee)
    {
        // I send the data to create a new employee and return the location of the new resource
        var created = await _service.CreateAsync(employee);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Employee employee)
    {
        // I validate that the ID in the URL matches the ID in the body
        if (id != employee.Id) return BadRequest("ID mismatch");
            
        await _service.UpdateAsync(employee);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        // I request the deletion of the employee by ID
        await _service.DeleteAsync(id);
        return NoContent();
    }
    //  GET MY INFO 
    [HttpGet("me")]
    public async Task<IActionResult> GetMyInfo()
    {
        
        var employeeIdClaim = User.FindFirst("EmployeeId")?.Value;

        if (!string.IsNullOrEmpty(employeeIdClaim) && int.TryParse(employeeIdClaim, out int employeeId))
        {
            var employee = await _service.GetByIdAsync(employeeId);
            if (employee != null) return Ok(employee);
        }
        
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email)) return Unauthorized();
            
        var empByEmail = await _service.GetByEmailAsync(email);
        if (empByEmail == null) return NotFound("Profile not found.");

        return Ok(empByEmail);
    }

        // DOWNLOAD MY CV 
        [HttpGet("me/cv")]
        public async Task<IActionResult> DownloadMyCv()
        {
            var employeeIdClaim = User.FindFirst("EmployeeId")?.Value;
            Employee? employee = null;

            if (!string.IsNullOrEmpty(employeeIdClaim) && int.TryParse(employeeIdClaim, out int empId))
            {
                employee = await _service.GetByIdAsync(empId);
            }
            
            if (employee == null)
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (!string.IsNullOrEmpty(email)) 
                {
                    employee = await _service.GetByEmailAsync(email);
                }
            }
            
            if (employee == null) return NotFound("Employee profile not found for PDF generation.");
            
            var pdfFile = _pdfService.GenerateEmployeeCv(employee);
            return File(pdfFile, "application/pdf", $"HV_{employee.FirstName}_{employee.LastName}.pdf");
        }
}