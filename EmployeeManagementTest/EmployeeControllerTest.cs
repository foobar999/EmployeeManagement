using EmployeeManagement.Controllers;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.JsonPatch;
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
        public void GetById_WithEmployeeNotInDb_ShouldReturnNotFoundResultWithPassedId()
        {
            var controller = this.CreateControllerWithMultipleEmployees();
            var actionResult = controller.GetById(this.otherId);
            this.AssertIsNotFoundResultWithExpectedId(actionResult, this.otherId);
        }

        [Fact]
        public void GetById_WithEmployeeInDb_ShouldReturnOkResultWithCorrectEmployee()
        {
            var controller = this.CreateControllerWithMultipleEmployees();
            var employeeInDb = this.sampleEmployees[0];
            var actionResult = controller.GetById(employeeInDb.Id);
            this.AssertIsOkResultWithExpectedEmployee(actionResult, employeeInDb);
        }

        [Fact]
        public void Create_WithValidEmployee_ShouldReturnCreatedAtActionWithPassedEmployee()
        {
            var controller = this.CreateControllerWithoutEmployees();
            var newEmployee = this.sampleEmployees[0];
            var actionResult = controller.Create(newEmployee);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetById", createdAtActionResult.ActionName); // guter Stil?
            Assert.Equal(newEmployee.Id, createdAtActionResult.RouteValues["id"]); // guter Stil?
            var employee = Assert.IsType<Employee>(createdAtActionResult.Value);
            Assert.Equal(employee, newEmployee);
        }

        [Fact]
        public void Delete_WithEmployeeNotInDb_ShouldReturnNotFoundResultWithPassedId()
        {
            var controller = this.CreateControllerWithMultipleEmployees();
            var actionResult = controller.Delete(this.otherId);
            this.AssertIsNotFoundResultWithExpectedId(actionResult, this.otherId);
        }

        [Fact]
        public void Delete_WithEmployeeInDb_ShouldReturnOkResultWithCorrectEmployee()
        {
            var controller = this.CreateControllerWithMultipleEmployees();
            var employeeInDb = this.sampleEmployees[0];
            var actionResult = controller.Delete(employeeInDb.Id);
            this.AssertIsOkResultWithExpectedEmployee(actionResult, employeeInDb);
        }

        [Fact]
        public void Patch_WithEmployeeNotInDb_ShouldReturnNotFoundResultWithPassedId()
        {
            var controller = this.CreateControllerWithMultipleEmployees();
            var patch = new JsonPatchDocument<Employee>();
            var actionResult = controller.Patch(this.otherId, patch);
            this.AssertIsNotFoundResultWithExpectedId(actionResult, this.otherId);
        }

        [Fact]
        public void Patch_WithEmployeeInDb_ShouldReturnOkResultWithUpdatedEmployee()
        {
            var controller = this.CreateControllerWithMultipleEmployees();
            var employeeInDb = this.sampleEmployees[0];
            var nameAndBirthdayPatch = new JsonPatchDocument<Employee>();
            nameAndBirthdayPatch.Replace(emp => emp.FirstName, "New");
            nameAndBirthdayPatch.Replace(emp => emp.SecondName, "Name");
            nameAndBirthdayPatch.Remove(emp => emp.DateOfBirth);
            var actionResult = controller.Patch(employeeInDb.Id, nameAndBirthdayPatch);
            var expectedEmployee = new Employee
            {
                Id = employeeInDb.Id,
                FirstName = "New",
                SecondName = "Name",
            };
            this.AssertIsOkResultWithExpectedEmployee(actionResult, expectedEmployee);
        }

        private readonly Guid otherId = new Guid("33333333-3333-3333-3333-333333333333");

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

        private void AssertIsNotFoundResultWithExpectedId(ActionResult<Employee> actionResult, Guid expectedId)
        {
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var id = Assert.IsType<Guid>(notFoundResult.Value);
            Assert.Equal(id, expectedId);
        }

        private void AssertIsOkResultWithExpectedEmployee(ActionResult<Employee> actionResult, Employee expectedEmployee)
        {
            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var employee = Assert.IsType<Employee>(objectResult.Value);
            Assert.Equal(employee, expectedEmployee);
        }
    }
}
