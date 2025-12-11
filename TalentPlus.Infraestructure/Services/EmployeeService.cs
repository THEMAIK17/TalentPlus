using Microsoft.EntityFrameworkCore;
using TalentPlus.Application.Interfaces;
using TalentPlus.Domain.Entities;
using TalentPlus.Infraestructure.Persistence;

namespace TalentPlus.Infraestructure.Services;

public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync(string? searchTerm = null)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .AsQueryable();

            // If a search term is provided, I filter by First Name, Last Name, or Document Number
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(e => 
                    e.FirstName.ToLower().Contains(searchTerm) || 
                    e.LastName.ToLower().Contains(searchTerm) || 
                    e.DocumentNumber.Contains(searchTerm));
            }
            
            return await query.ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            // I search for the employee by ID, ensuring I also load their Department
            return await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        public async Task<Employee?> GetByEmailAsync(string email)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Email.ToLower().Trim() == email.ToLower().Trim());
        }

        public async Task<Employee> CreateAsync(Employee employee)
        {
            // I add the new employee to the database context and save changes
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task UpdateAsync(Employee employee)
        {
            // I update the existing employee's state and persist the changes
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // I first find the employee to ensure they exist before trying to remove them
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }
    }