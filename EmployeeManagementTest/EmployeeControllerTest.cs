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
        public void GetAll_WithMultipleEmployees_ShouldReturnOkResultWithCorrectEmployees()
        {
            var controller = this.CreateControllerWithMultipleEmployees();
            var actionResult = controller.GetAll();
            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var employees = Assert.IsAssignableFrom<IEnumerable<Employee>>(objectResult.Value);
            var expectedEmployees = this.sampleEmployees;
            Assert.Equal(employees, expectedEmployees);
        }

        [Fact]
        public void GetById_WithEmployeeNotInDb_ShouldReturnNotFoundResultWithCorrectId()
        {
            var controller = this.CreateControllerWithMultipleEmployees();
            var actionResult = controller.GetById(this.idOutsideDb);
            var objectResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var id = Assert.IsType<Guid>(objectResult.Value);
            Assert.Equal(id, this.idOutsideDb);
        }

        [Fact]
        public void GetById_WithEmployeeInDb_ShouldReturnOkResultWithCorrectEmployee()
        {
            var controller = this.CreateControllerWithMultipleEmployees();
            var employeeInDb = this.sampleEmployees[0];
            var actionResult = controller.GetById(employeeInDb.Id);
            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var employee = Assert.IsType<Employee>(objectResult.Value);
            Assert.Equal(employee, employeeInDb);
        }

        private readonly Guid idOutsideDb = new Guid("33333333-3333-3333-3333-333333333333");

        private readonly List<Employee> sampleEmployees = new List<Employee> {
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

        private EmployeeController CreateControllerWithMultipleEmployees()
        {
            return new EmployeeController(this.CreateMockDbContext(this.sampleEmployees));
        }

        private EmployeeController CreateControllerWithoutEmployees()
        {
            return new EmployeeController(this.CreateMockDbContext(new List<Employee>()));
        }

        private EmployeeContext CreateMockDbContext(List<Employee> employees)
        {
            var newDbContextOptions = new DbContextOptionsBuilder<EmployeeContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var newDbContext = new EmployeeContext(newDbContextOptions);
            newDbContext.Employees.AddRange(employees);
            newDbContext.SaveChanges(); //!!!!!
            return newDbContext;
        }
    }
}
