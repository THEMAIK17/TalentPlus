using Microsoft.AspNetCore.Identity;
using TalentPlus.Domain.Entities;
using TalentPlus.Infraestructure.Identity;

namespace TalentPlus.Infraestructure.Persistence;

public static class DbInitializer
    {
        public static async Task SeedDataAsync(AppDbContext context, UserManager<AppUser> userManager)
        {
            // I check if the database is already seeded to avoid duplicates
            if (context.Employees.Any()) return;

            //  SEED DEPARTMENTS
            var itDept = new Department { Name = "Tecnología" };
            var hrDept = new Department { Name = "Recursos Humanos" };
            var salesDept = new Department { Name = "Ventas" };
            var logDept = new Department { Name = "Logística" };

            // I add departments only if none exist
            if (!context.Departments.Any())
            {
                context.Departments.AddRange(itDept, hrDept, salesDept, logDept);
                await context.SaveChangesAsync();
            }

            // SEED EMPLOYEES
            // I create a mix of Active, Vacation and Inactive employees for dashboard stats
            var employees = new List<Employee>
            {
                new Employee
                {
                    FirstName = "Carlos", LastName = "Díaz", Email = "carlos.dev@talentplus.com",
                    DocumentNumber = "1001", Position = "Desarrollador Senior",
                    Salary = 8500000, Status = "Activo", HiringDate = DateTime.UtcNow.AddYears(-2),
                    DepartmentId = itDept.Id, Address = "Calle 123 #45-67", Phone = "3001234567"
                },
                new Employee
                {
                    FirstName = "Ana", LastName = "Martínez", Email = "ana.hr@talentplus.com",
                    DocumentNumber = "1002", Position = "Gerente RRHH",
                    Salary = 7200000, Status = "Vacaciones", HiringDate = DateTime.UtcNow.AddYears(-3),
                    DepartmentId = hrDept.Id, Address = "Carrera 10 #20-30", Phone = "3109876543"
                },
                new Employee
                {
                    FirstName = "Jorge", LastName = "Pérez", Email = "jorge.sales@talentplus.com",
                    DocumentNumber = "1003", Position = "Ejecutivo de Ventas",
                    Salary = 4500000, Status = "Activo", HiringDate = DateTime.UtcNow.AddMonths(-6),
                    DepartmentId = salesDept.Id, Address = "Av Siempre Viva 742", Phone = "3151112233"
                },
                new Employee
                {
                    FirstName = "Luisa", LastName = "Ramírez", Email = "luisa.log@talentplus.com",
                    DocumentNumber = "1004", Position = "Auxiliar Logístico",
                    Salary = 2800000, Status = "Inactivo", HiringDate = DateTime.UtcNow.AddYears(-1),
                    DepartmentId = logDept.Id, Address = "Transversal 4 #5-10", Phone = "3205556677"
                },
                new Employee
                {
                    FirstName = "Maikol", LastName = "Administrador", Email = "admin@talentplus.com",
                    DocumentNumber = "9999", Position = "CTO",
                    Salary = 15000000, Status = "Activo", HiringDate = DateTime.UtcNow.AddYears(-5),
                    DepartmentId = itDept.Id, Address = "Torre Empresarial", Phone = "3000000000"
                }
            };

            context.Employees.AddRange(employees);
            await context.SaveChangesAsync();

            // SEED DEFAULT ADMIN USER
            if (!userManager.Users.Any(u => u.Email == "admin@talentplus.com"))
            {
                var adminUser = new AppUser
                {
                    UserName = "admin@talentplus.com",
                    Email = "admin@talentplus.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "Password123!");
            }
        }
    }