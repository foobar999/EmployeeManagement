using EmployeeManagement.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

// beachte: Git greift auch hier auf ~/.gitconfig zurück!

// TODO SQL (EntityFramework?)
// TODO Tests
// TODO Swagger
// aaa
// TODO Fehler melden, falls ID bereits drin

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        public EmployeeController(EmployeeContext context)
        {
            this.context = context;

            // TODO Erzeugung initialer Datensätze kicken
            if (this.context.Employees.Count() == 0)
            {
                /*
                 zum Testen (Case-Sensitiv!)
                 - Case-Sensitiv
                 - klappt iwie nich immer?! -> In Addon Custom <-> JSON hilft ggf.
                {
                 "FirstName": "Hallo",
                 "SecondName": "Welt",
                 "DateOfBirth": "10.10.1990"
                }
                 */
                // Create a new Employee if collection is empty,
                // which means you can't delete all Employee.
                this.context.Employees.Add(
                    new Employee
                    {
                        FirstName = "Hans",
                        SecondName = "Wurst",
                        DateOfBirth = new DateTime(1945, 3, 12)
                    });
                this.context.Employees.Add(
                    new Employee
                    {
                        FirstName = "Jim",
                        SecondName = "Beam",
                        DateOfBirth = new DateTime(1933, 2, 11)
                    });
                this.context.SaveChanges();
            }
        }

        [HttpGet]
        public ActionResult<List<Employee>> GetAll()
        {
            return this.context.Employees.ToList();
        }

        [HttpGet("{id}", Name = "GetEmployee")]
        public ActionResult<Employee> GetById(Guid id)
        {
            var item = this.context.Employees.Find(id);
            if (item == null)
            {
                return base.NotFound();
            }
            return item;
        }

        [HttpPost]
        public ActionResult<Employee> Create(Employee employee)
        {
            this.context.Employees.Add(employee);
            this.context.SaveChanges();

            return employee;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var employee = this.context.Employees.Find(id);
            if (employee == null)
            {
                return base.NotFound();
            }
            this.context.Employees.Remove(employee);
            this.context.SaveChanges();

            return base.Ok();
        }

        // PATCH ist sehr bizarr und erwartet im JSON-Body eine Liste von kodierten Befehlen
        // siehe auch https://dotnetcoretutorials.com/2017/11/29/json-patch-asp-net-core/
        [HttpPatch("{id}")]
        public ActionResult<Employee> Patch(Guid id, JsonPatchDocument<Employee> patch)
        {
            var employee = this.context.Employees.Find(id);
            if (employee == null)
            {
                return base.NotFound();
            }
            patch.ApplyTo(employee);
            this.context.SaveChanges();

            return base.Ok();
        }

        private readonly EmployeeContext context;
    }
}