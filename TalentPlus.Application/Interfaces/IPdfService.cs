using TalentPlus.Domain.Entities;

namespace TalentPlus.Application.Interfaces;

public interface IPdfService
{
    // I define the method to generate the PDF bytes for a given employee
    byte[] GenerateEmployeeCv(Employee employee);
}