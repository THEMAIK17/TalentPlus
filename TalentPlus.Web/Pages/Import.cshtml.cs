using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentPlus.Application.Interfaces;

namespace TalentPlus.Web.Pages;

[Authorize] 
public class ImportModel : PageModel
{
    private readonly IExcelService _excelService;

    public ImportModel(IExcelService excelService)
    {
        _excelService = excelService;
    }

    [BindProperty]
    [Required(ErrorMessage = "Debes seleccionar un archivo.")]
    public IFormFile Upload { get; set; }

    
    [TempData]
    public string Message { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || Upload == null)
        {
            return Page();
        }
        
        if (!Upload.FileName.EndsWith(".xlsx"))
        {
            ModelState.AddModelError("Upload", "Solo se permiten archivos Excel (.xlsx)");
            return Page();
        }

        try
        {
           
            var result = await _excelService.ImportEmployeesAsync(Upload);
                
            
            Message = $" {result}"; 
            return RedirectToPage("/Dashboard"); 
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Error al procesar: {ex.Message}");
            return Page();
        }
    }
}
