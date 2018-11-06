using EmployeeManagement.Controllers;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Xunit;

namespace EmployeeManagementTest
{
    public class EmployeeControllerTest
    {
        // https://docs.microsoft.com/de-de/aspnet/core/mvc/controllers/testing?view=aspnetcore-2.1#test-actionresultlttgt
        [Fact]
        public void GetAll_WithMultipleEmployees_ShouldReturnCorrectEmployees()
        {
            var controller = this.GetControllerWithMultipleEmployees();
            var actionResult = controller.GetAll();
            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var employees = Assert.IsAssignableFrom<IEnumerable<Employee>>(objectResult.Value);
            var expectedEmployees = this.GetSampleEmployees();
            Assert.Equal(employees, expectedEmployees);
        }

        private EmployeeController GetControllerWithMultipleEmployees()
        {
            return new EmployeeController(this.GetMockDbContext(this.GetSampleEmployees()));
        }

        private EmployeeContext GetMockDbContext(List<Employee> employees)
        {
            var newDbContextOptions = new DbContextOptionsBuilder<EmployeeContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var newDbContext = new EmployeeContext(newDbContextOptions);
            newDbContext.Employees.AddRange(employees);
            newDbContext.SaveChanges(); //!!!!!
            return newDbContext;
        }

        private List<Employee> GetSampleEmployees()
        {
            return new List<Employee> {
                new Employee
                {
                    Id = new Guid("11111111-1111-1111-1111-111111111111"),
                    FirstName = "Hans",
                    SecondName = "Wurst",
                    DateOfBirth = new DateTime(1945, 3, 12)
                },
                new Employee
                {
                    Id = new Guid("22222222-2222-2222-2222-222222222222"),
                    FirstName = "Jim",
                    SecondName = "Beam",
                    DateOfBirth = new DateTime(1933, 2, 11)
                }
            };
        }
    }
}
