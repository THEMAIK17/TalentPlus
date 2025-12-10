using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TalentPlus.Application.Interfaces;
using TalentPlus.Domain.Entities;
using TalentPlus.Infraestructure.Persistence;

namespace TalentPlus.Web.Pages;

[Authorize]
public class EditModel : PageModel
{
    private readonly IEmployeeService _employeeService;
    private readonly AppDbContext _context; 

    public EditModel(IEmployeeService employeeService, AppDbContext context)
    {
        _employeeService = employeeService;
        _context = context;
    }

    [BindProperty]
    public Employee Employee { get; set; } = new();

    // I hold the list of departments for the select dropdown
    public SelectList DepartmentOptions { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var emp = await _employeeService.GetByIdAsync(id);
        if (emp == null) return NotFound();

        Employee = emp;

      
        var depts = await _context.Departments.OrderBy(d => d.Name).ToListAsync();
        DepartmentOptions = new SelectList(depts, "Id", "Name");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            // Reload departments if validation fails so the dropdown doesn't break
            var depts = await _context.Departments.OrderBy(d => d.Name).ToListAsync();
            DepartmentOptions = new SelectList(depts, "Id", "Name");
            return Page();
        }
        Employee.HiringDate = DateTime.SpecifyKind(Employee.HiringDate, DateTimeKind.Utc);
        await _employeeService.UpdateAsync(Employee);

        return RedirectToPage("/Employees");
    }
}