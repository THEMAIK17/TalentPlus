using System.ComponentModel.DataAnnotations;

namespace TalentPlus.Application.DTOs;

public class EmployeeLoginDto {
    [Required] public string DocumentNumber { get; set; } = "";
    [Required] [EmailAddress] public string Email { get; set; } = "";
}