using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TalentPlus.Application.DTOs;
using TalentPlus.Application.Interfaces;
using TalentPlus.Domain.Entities;
using TalentPlus.Infraestructure.Persistence;

namespace TalentPlus.Web.Controllers;

[Route("api/[controller]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public PublicController(AppDbContext context, IEmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
        }

        // PUBLIC LIST OF DEPARTMENTS
        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            
            var depts = await _context.Departments
                .Select(d => new { d.Id, d.Name })
                .ToListAsync();
            return Ok(depts);
        }

        // SELF-REGISTRATION
        [HttpPost("register")]
        public async Task<IActionResult> Register(EmployeeRegisterDto dto)
        {
            
            if (await _context.Employees.AnyAsync(e => e.DocumentNumber == dto.DocumentNumber || e.Email == dto.Email))
            {
                return BadRequest("El empleado ya existe.");
            }

            // I create the new employee entity
            var employee = new Employee
            {
                DocumentNumber = dto.DocumentNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                DepartmentId = dto.DepartmentId,
                Position = "Solicitante", // Default position
                HiringDate = DateTime.UtcNow,
                Status = "Activo",
                Address = "Pendiente", 
                Phone = "Pendiente"
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            
            try 
            {
                await _emailService.SendWelcomeEmailAsync(dto.Email, dto.FirstName);
            }
            catch
            {
                // I allow the process to continue even if email fails 
            }

            return Ok(new { message = "Registro exitoso. Revisa tu correo." });
        }

        //  EMPLOYEE LOGIN 
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] EmployeeLoginDto login)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.DocumentNumber == login.DocumentNumber && e.Email == login.Email);

            if (employee == null) return Unauthorized("Credenciales inválidas.");
            
            var token = GenerateEmployeeToken(employee);
            return Ok(new { token });
        }

        private string GenerateEmployeeToken(Employee employee)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, employee.Email), 
                new("EmployeeId", employee.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }