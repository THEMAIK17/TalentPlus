namespace TalentPlus.Domain.Entities;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Relationship: One department has many employees
    public ICollection<Employee>? Employees { get; set; }
}