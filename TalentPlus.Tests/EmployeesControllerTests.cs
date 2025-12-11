using Microsoft.AspNetCore.Mvc;
using Moq;
using TalentPlus.Application.Interfaces;
using TalentPlus.Domain.Entities;
using TalentPlus.Web.Controllers;

namespace TalentPlus.Tests;

public class EmployeesControllerTests
    {
        // 1. UNIT TEST: GetById returns Ok (200) when employee exists
        [Fact]
        public async Task GetById_ReturnsOk_WhenEmployeeExists()
        {
            // Arrange (Prepare the simulation)
            var mockService = new Mock<IEmployeeService>();
            var mockPdfService = new Mock<IPdfService>(); // We need to mock dependencies
            var testEmployee = new Employee { Id = 1, FirstName = "Test", LastName = "User" };

            // I tell the mock: "When GetByIdAsync(1) is called, return this testEmployee"
            mockService.Setup(s => s.GetByIdAsync(1))
                       .ReturnsAsync(testEmployee);

            var controller = new EmployeesController(mockService.Object, mockPdfService.Object);

            // Act (Run the method)
            var result = await controller.GetById(1);

            // Assert (Verify the result)
            var okResult = Assert.IsType<OkObjectResult>(result); // Check if it's 200 OK
            var returnEmployee = Assert.IsType<Employee>(okResult.Value); // Check if data is Employee
            Assert.Equal("Test", returnEmployee.FirstName); // Check if data is correct
        }

        // 2. UNIT TEST: GetById returns NotFound (404) when employee does not exist
        [Fact]
        public async Task GetById_ReturnsNotFound_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var mockService = new Mock<IEmployeeService>();
            var mockPdfService = new Mock<IPdfService>();

            // I tell the mock: "When GetByIdAsync(99) is called, return null"
            mockService.Setup(s => s.GetByIdAsync(99))
                       .ReturnsAsync((Employee)null);

            var controller = new EmployeesController(mockService.Object, mockPdfService.Object);

            // Act
            var result = await controller.GetById(99);

            // Assert
            Assert.IsType<NotFoundResult>(result); // Check if it's 404 Not Found
        }
    }