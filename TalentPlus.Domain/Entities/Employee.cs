namespace TalentPlus.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
        
    // Personal Data
    public string DocumentNumber { get; set; } = string.Empty; 
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
        
    // Job Information
    public string Position { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public DateTime HiringDate { get; set; }
    public string Status { get; set; } = "Active"; // Values: Active, Inactive, Vacation
        
    // CV / Resume Data
    public string EducationLevel { get; set; } = string.Empty;
    public string Profile { get; set; } = string.Empty;

    // Relationships
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    // Link with Identity User 
    public string? AspNetUserId { get; set; } 
}