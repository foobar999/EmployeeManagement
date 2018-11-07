using EmployeeManagement.Controllers;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace EmployeeManagementTest
{
    public class EmployeeControllerTest
    {
        // https://docs.microsoft.com/de-de/aspnet/core/mvc/controllers/testing?view=aspnetcore-2.1#test-actionresultlttgt
        [Fact]
        [Trait("Category", "Unit")]
        [ExcludeFromCodeCoverage]
        public void GetAll_WithMultipleEmployees_ShouldReturnOkWithCorrectEmployees()
        {
            var controller = this.CreateControllerWithMultipleEmployees();
            var expectedEmployees = this.sampleEmployees;

            var actionResult = controller.GetAll();

            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var actualEmployees = Assert.IsAssignableFrom<IEnumerable<Employee>>(objectResult.Value);
            Assert.Equal(expectedEmployees, actualEmployees);
        }

        [Fact]
        [Trait("Category", "Unit")]
        [ExcludeFromCodeCoverage]
        public void GetById_WithEmployeeNotInDb_ShouldReturnNotFoundWithPassedId()
        {
            var controller = this.CreateControllerWithMultipleEmployees();
            var expectedId = this.otherId;

            var actionResult = controller.GetById(this.otherId);

            this.AssertIsNotFoundWithExpectedId(actionResult, expectedId);
        }

        [Fact]
        [Trait("Category", "Unit")]
        [ExcludeFromCodeCoverage]
        public void GetById_WithEmployeeInDb_ShouldReturnOkWithCorrectEmployee()
        {
            var controller = this.CreateControllerWithMultipleEmployees();
            var expectedEmployee = this.sampleEmployees[0];

            var actionResult = controller.GetById(this.sampleEmployees[0].Id);

            this.AssertIsOkWithExpectedEmployee(actionResult, expectedEmployee);
        }

        [Fact]
        [Trait("Category", "Unit")]
        [ExcludeFromCodeCoverage]
        public void GetById_WithInvalidState_ShouldReturnBadRequestWithSerializableError()
        {
            var controller = this.CreateControllerWithMultipleEmployees();
            controller.ModelState.AddModelError("error", "some error");

            var actionResult = controller.GetById(this.sampleEmployees[0].Id);

            this.AssertIsBadRequestWithSerializableError(actionResult);
        }

        [Fact]
        [Trait("Category", "Unit")]
        [ExcludeFromCodeCoverage]
        public void Create_WithValidEmployee_ShouldReturnCreatedAtActionWithPassedEmployee()
        {
            var controller = this.CreateControllerWithoutEmployees();
            var expectedEmployee = this.sampleEmployees[0];

            var actionResult = controller.Create(this.sampleEmployees[0]);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetById", createdAtActionResult.ActionName); // guter Stil?
            Assert.Equal(expectedEmployee.Id, createdAtActionResult.RouteValues["id"]); // guter Stil?
            var actualEmployee = Assert.IsType<Employee>(createdAtActionResult.Value);
            Assert.Equal(expectedEmployee, actualEmployee);
        }

        [Fact]
        [Trait("Category", "Unit")]
        [ExcludeFromCodeCoverage]
        public void Create_WithInvalidState_ShouldReturnBadRequestWithSerializableError()
        {
            var controller = this.CreateControllerWithoutEmployees();
            controller.ModelState.AddModelError("error", "some error");

            var actionResult = controller.Create(this.sampleEmployees[0]);

            this.AssertIsBadRequestWithSerializableError(actionResult);
        }

        [Fact]
        [Trait("Category", "Unit")]
        [ExcludeFromCodeCoverage]
        public void Delete_WithEmployeeNotInDb_ShouldReturnNotFoundWithPassedId()
        {
            var controller = this.CreateControllerWithMultipleEmployees();
            var expectedId = this.otherId;

            var actionResult = controller.Delete(this.otherId);

            this.AssertIsNotFoundWithExpectedId(actionResult, expectedId);
        }

        [Fact]
        [Trait("Category", "Unit")]
        [ExcludeFromCodeCoverage]
        public void Delete_WithEmployeeInDb_ShouldReturnOkWithCorrectEmployee()
        {
            var controller = this.CreateControllerWithMultipleEmployees();
            var expectedEmployee = this.sampleEmployees[0];

            var actionResult = controller.Delete(this.sampleEmployees[0].Id);

            this.AssertIsOkWithExpectedEmployee(actionResult, expectedEmployee);
        }

        [Fact]
        [Trait("Category", "Unit")]
        [ExcludeFromCodeCoverage]
        public void Patch_WithEmployeeNotInDb_ShouldReturnNotFoundWithPassedId()
        {
            var controller = this.CreateControllerWithMultipleEmployees();
            var patch = new JsonPatchDocument<Employee>();
            var expectedId = this.otherId;

            var actionResult = controller.Patch(this.otherId, patch);

            this.AssertIsNotFoundWithExpectedId(actionResult, expectedId);
        }

        [Fact]
        [Trait("Category", "Unit")]
        [ExcludeFromCodeCoverage]
        public void Patch_WithEmployeeInDb_ShouldReturnOkWithUpdatedEmployee()
        {
            var controller = this.CreateControllerWithMultipleEmployees();
            var idOfEmployeeInDb = this.sampleEmployees[0].Id;
            var nameAndBirthdayPatch = new JsonPatchDocument<Employee>();
            nameAndBirthdayPatch.Replace(emp => emp.FirstName, "New");
            nameAndBirthdayPatch.Replace(emp => emp.SecondName, "Name");
            nameAndBirthdayPatch.Remove(emp => emp.DateOfBirth);
            var expectedEmployee = new Employee
            {
                Id = idOfEmployeeInDb,
                FirstName = "New",
                SecondName = "Name",
            };

            var actionResult = controller.Patch(idOfEmployeeInDb, nameAndBirthdayPatch);

            this.AssertIsOkWithExpectedEmployee(actionResult, expectedEmployee);
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
            return new EmployeeController(this.CreateDbContextWithEmployees(this.sampleEmployees));
        }

        private EmployeeController CreateControllerWithoutEmployees()
        {
            return new EmployeeController(this.CreateDbContextWithEmployees(new List<Employee>()));
        }

        private EmployeeContext CreateDbContextWithEmployees(List<Employee> employees)
        {
            var newDbContextOptions = new DbContextOptionsBuilder<EmployeeContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var newDbContext = new EmployeeContext(newDbContextOptions);
            newDbContext.Employees.AddRange(employees);
            newDbContext.SaveChanges(); //!!!!!
            return newDbContext;
        }

        private void AssertIsNotFoundWithExpectedId(ActionResult<Employee> actionResult, Guid expectedId)
        {
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var actualId = Assert.IsType<Guid>(notFoundResult.Value);
            Assert.Equal(expectedId, actualId);
        }

        private void AssertIsOkWithExpectedEmployee(ActionResult<Employee> actionResult, Employee expectedEmployee)
        {
            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var actualEmployee = Assert.IsType<Employee>(objectResult.Value);
            Assert.Equal(expectedEmployee, actualEmployee);
        }

        private void AssertIsBadRequestWithSerializableError(ActionResult<Employee> actionResult)
        {
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }
    }
}
