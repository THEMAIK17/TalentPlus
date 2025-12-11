using Microsoft.AspNetCore.Mvc;
using TalentPlus.Application.Interfaces;

namespace TalentPlus.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImportController : ControllerBase
{
    private readonly IExcelService _excelService;

    public ImportController(IExcelService excelService)
    {
        _excelService = excelService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Please upload a valid Excel file.");

        try
        {
            var result = await _excelService.ImportEmployeesAsync(file);
            return Ok(new { message = result });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
