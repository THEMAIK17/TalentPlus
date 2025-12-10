using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentPlus.Application.Interfaces;
using TalentPlus.Domain.Entities;

namespace TalentPlus.Web.Controllers;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
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
}