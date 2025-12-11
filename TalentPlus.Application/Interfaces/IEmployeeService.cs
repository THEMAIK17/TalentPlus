using TalentPlus.Domain.Entities;

namespace TalentPlus.Application.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetAllAsync(string? searchTerm = null);
    
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByEmailAsync(string email);
    // I define the methods to modify the state of my entities (Create, Update, Delete)
    Task<Employee> CreateAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(int id);
}