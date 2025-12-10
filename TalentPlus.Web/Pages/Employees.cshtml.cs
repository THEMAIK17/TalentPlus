using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentPlus.Application.Interfaces;
using TalentPlus.Domain.Entities;

namespace TalentPlus.Web.Pages;

[Authorize] // I ensure only authenticated users can access this page
public class EmployeesModel : PageModel
{
    private readonly IEmployeeService _employeeService;
    private readonly IPdfService _pdfService; // I inject the PDF service

    public EmployeesModel(IEmployeeService employeeService, IPdfService pdfService)
    {
        _employeeService = employeeService;
        _pdfService = pdfService;
    }

    public IEnumerable<Employee> EmployeesList { get; set; } = new List<Employee>();

    [BindProperty(SupportsGet = true)]
    public string Search { get; set; } = "";

    public async Task OnGetAsync()
    {
        // I retrieve the list of employees filtered by search term
        EmployeesList = await _employeeService.GetAllAsync(Search);
    }

    // I handle the request to download the CV PDF
    public async Task<IActionResult> OnGetDownloadCv(int id)
    {
        // I fetch the employee data
        var employee = await _employeeService.GetByIdAsync(id);
            
        if (employee == null) return NotFound();

        // I generate the PDF bytes
        var pdfBytes = _pdfService.GenerateEmployeeCv(employee);

        // I return the file with the correct PDF content type
        return File(pdfBytes, "application/pdf", $"HV_{employee.FirstName}_{employee.LastName}.pdf");
    }

    // I handle the delete action
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _employeeService.DeleteAsync(id);
        return RedirectToPage();
    }
}