using Microsoft.AspNetCore.Http;

namespace TalentPlus.Application.Interfaces;

public interface IExcelService
{
    // Contract: Process the uploaded file and return a summary message
    Task<string> ImportEmployeesAsync(IFormFile file);
}