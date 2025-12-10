using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TalentPlus.Application.Interfaces;
using TalentPlus.Domain.Entities;
using TalentPlus.Infraestructure.Persistence;

namespace TalentPlus.Infraestructure.Services;

public class ExcelService : IExcelService
    {
        private readonly AppDbContext _context;

       
        [Obsolete("Obsolete")]
        public ExcelService(AppDbContext context)
        {
            _context = context;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<string> ImportEmployeesAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return "File is empty.";

            // Set EPPlus license context (Required for non-commercial use)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            int newEmployees = 0;
            int updatedEmployees = 0;

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    // Start iterating from row 2 to skip headers
                    for (int row = 2; row <= rowCount; row++)
                    {
                        // --- COLUMN MAPPING BASED ON CSV STRUCTURE ---
                        // 1:Documento, 2:Nombres, 3:Apellidos, 4:FechaNacimiento, 5:Direccion, 6:Telefono
                        // 7:Email, 8:Cargo, 9:Salario, 10:FechaIngreso, 11:Estado, 12:NivelEducativo, 13:Perfil, 14:Departamento
                        
                        var document = worksheet.Cells[row, 1].Text.Trim();   
                        var firstName = worksheet.Cells[row, 2].Text.Trim();  
                        var lastName = worksheet.Cells[row, 3].Text.Trim();   
                        // Column 4 (BirthDate) is skipped as it's not in the Entity
                        var address = worksheet.Cells[row, 5].Text.Trim();    
                        var phone = worksheet.Cells[row, 6].Text.Trim();      
                        var email = worksheet.Cells[row, 7].Text.Trim();      
                        var position = worksheet.Cells[row, 8].Text.Trim();   
                        var salaryText = worksheet.Cells[row, 9].Text.Trim(); 
                        var dateText = worksheet.Cells[row, 10].Text.Trim();  
                        var status = worksheet.Cells[row, 11].Text.Trim();    
                        var education = worksheet.Cells[row, 12].Text.Trim(); 
                        var profile = worksheet.Cells[row, 13].Text.Trim();   
                        var deptName = worksheet.Cells[row, 14].Text.Trim();  
                        
                        if (string.IsNullOrEmpty(document)) continue;

                        // 1. DEPARTMENT 
                        var department = await _context.Departments
                            .FirstOrDefaultAsync(d => d.Name == deptName);
                        
                        if (department == null)
                        {
                            department = new Department { Name = deptName };
                            _context.Departments.Add(department);
                            await _context.SaveChangesAsync();
                        }

                        //  EMPLOYEE 
                        var employee = await _context.Employees
                            .FirstOrDefaultAsync(e => e.DocumentNumber == document);
                        
                        decimal.TryParse(salaryText, out decimal salary);
                        DateTime.TryParse(dateText, out DateTime hiringDate);
                        if (hiringDate == DateTime.MinValue) hiringDate = DateTime.UtcNow;

                        if (employee == null)
                        {
                            // Create new employee
                            employee = new Employee
                            {
                                DocumentNumber = document,
                                FirstName = firstName,
                                LastName = lastName,
                                Email = email,
                                Phone = phone,
                                Address = address,
                                Position = position,
                                DepartmentId = department.Id,
                                Salary = salary,
                                HiringDate = hiringDate.ToUniversalTime(), 
                                Status = status,       
                                EducationLevel = education, 
                                Profile = profile      
                            };
                            _context.Employees.Add(employee);
                            newEmployees++;
                        }
                        else
                        {
                            
                            employee.FirstName = firstName;
                            employee.LastName = lastName;
                            employee.Email = email;
                            employee.Phone = phone;
                            employee.Address = address;
                            employee.DepartmentId = department.Id;
                            employee.Salary = salary;
                            employee.Position = position;
                            employee.Status = status;
                            employee.EducationLevel = education;
                            employee.Profile = profile;
                         
                            
                            _context.Employees.Update(employee);
                            updatedEmployees++;
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }
            return $"Success. New Employees: {newEmployees}, Updated: {updatedEmployees}.";
        }
    }