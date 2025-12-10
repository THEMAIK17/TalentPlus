using System.ComponentModel.DataAnnotations;

namespace TalentPlus.Application.DTOs;

public class EmployeeRegisterDto {
    [Required] public string DocumentNumber { get; set; } = "";
    [Required] public string FirstName { get; set; } = "";
    [Required] public string LastName { get; set; } = "";
    [Required] [EmailAddress] public string Email { get; set; } = "";
    [Required] public int DepartmentId { get; set; }
}