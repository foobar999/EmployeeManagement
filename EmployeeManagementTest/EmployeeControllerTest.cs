using EmployeeManagement.Controllers;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EmployeeManagementTest
{
    public class EmployeeControllerTest
    {
        // https://docs.microsoft.com/de-de/aspnet/core/mvc/controllers/testing?view=aspnetcore-2.1#test-actionresultlttgt
        [Fact]
        public void GetAll_WithMultipleEmployees_ShouldReturnCorrectEmployees()
        {
            var controller = new EmployeeController(this.GetFilledMockDbContext());
            var result = controller.GetAll();
            var actionResult = Assert.IsType<ActionResult<List<Employee>>>(result);
            var listResult = Assert.IsType<List<Employee>>(actionResult.Value);
            Assert.Equal(2, listResult.Count());
        }

        private EmployeeContext GetMockDbContext(params Employee[] employees)
        {
            var newDbContextOptions = new DbContextOptionsBuilder<EmployeeContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var newDbContext = new EmployeeContext(newDbContextOptions);
            newDbContext.Employees.AddRange(employees);
            newDbContext.SaveChanges(); //!!!!!
            return newDbContext;
        }

        private EmployeeContext GetFilledMockDbContext()
        {
            return this.GetMockDbContext(
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
            );
        }
    }
}
