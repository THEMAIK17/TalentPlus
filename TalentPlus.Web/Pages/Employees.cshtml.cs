using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentPlus.Application.Interfaces;
using TalentPlus.Domain.Entities;

namespace TalentPlus.Web.Pages;

[Authorize] 
public class EmployeesModel : PageModel
{
    private readonly IEmployeeService _employeeService;

    public EmployeesModel(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // List of employees to display in the UI
    public IEnumerable<Employee> EmployeesList { get; set; } = new List<Employee>();

    // Property to hold the search text (binds to the URL query string ?search=...)
    [BindProperty(SupportsGet = true)]
    public string Search { get; set; } = "";

    public async Task OnGetAsync()
    {
       
        EmployeesList = await _employeeService.GetAllAsync(Search);
    }
    // I handle the delete request sent from the form
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _employeeService.DeleteAsync(id);
            
        
        return RedirectToPage();
    }
}