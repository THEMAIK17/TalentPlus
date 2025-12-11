using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TalentPlus.Infraestructure.Persistence;

namespace TalentPlus.Web.Pages;

// I protect this page so only logged-in users can see the stats
[Authorize]
public class DashboardModel : PageModel
{
    private readonly AppDbContext _context;

    public DashboardModel(AppDbContext context)
    {
        _context = context;
    }

    // Properties to hold the data for the View 
    public int TotalEmployees { get; set; }
    public int ActiveCount { get; set; }
    public decimal TotalPayroll { get; set; }
    public List<DepartmentStat> DeptStats { get; set; } = new();

    // Helper class for the chart data
    public class DepartmentStat
    {
        public string Name { get; set; } = "";
        public int Count { get; set; }
    }

    public async Task OnGetAsync()
    {
        // I fetch the general counters from the database
        TotalEmployees = await _context.Employees.CountAsync();
            
        // I check for both Spanish and English status just in case
        ActiveCount = await _context.Employees
            .CountAsync(e => e.Status == "Activo" || e.Status == "Active");

        //  I calculate the total monthly payroll sum
        TotalPayroll = await _context.Employees.SumAsync(e => e.Salary);

        //  I group employees by department to show the distribution
        DeptStats = await _context.Employees
            .Include(e => e.Department)
            .GroupBy(e => e.Department!.Name)
            .Select(g => new DepartmentStat 
            { 
                Name = g.Key, 
                Count = g.Count() 
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync();
    }
}