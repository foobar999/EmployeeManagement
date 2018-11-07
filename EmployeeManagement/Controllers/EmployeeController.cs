using EmployeeManagement.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;

// Liste der ActionResults https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase.accepted?view=aspnetcore-2.1

// beachte: Git greift auch hier auf ~/.gitconfig zurück!
// siehe auch 
// https://stackoverflow.com/questions/15381198/remove-credentials-from-git
// https://stackoverflow.com/questions/5343068/is-there-a-way-to-skip-password-typing-when-using-https-on-github
// Systemsteuerung\Benutzerkonten und Family Safety\Anmeldeinformationsverwaltung
// ich hab die Konfig auf das allgemeine 'helper = cache --timeout=300' gesetzt
// wäre das Windows-spezifische 'helper = manager' ggf. sinnvoller?
// VS scheint stets ein Token in Anmeldeinformationsverwaltung zu hinterlegen

// zum initialen Erstellen der SQLite-DB hab ich Migrationen benutzt
// dazu die ersten beiden "PowerShell"-Befehle aus https://docs.microsoft.com/de-de/ef/core/managing-schemas/migrations/index
// initiale Migration erstellen:
// - Add-Migration InitialCreate
// - Update-Database
// Schema aktualisieren:
// - Add-Migration MeineTollenÄnderungen
// - drübergucken, ob ok
// - Update-Database
// beachte: SQLite kann dies ggf. nicht ('AlterColumnOperation')
// -> stattdessen:
// - DB-Datei und Migration-Ordner löschen
// - Projekt neu bauen
// - initiale Migration erstellen

// Seeding nach https://docs.microsoft.com/en-us/ef/core/modeling/data-seeding
// hasData() in EmployeeContect.OnModelCreating(ModelBuilder modelBuilder)
// - entspricht einer Migration
// -> Anwendung neues Seedings:
//   - Add-Migration <NAME SEED-MIGRATION>
//   - Update-Database
// - schiebt 1x die nötigen Daten rein -> Neustart DB sorgt *nicht* für erneutes Füllen
// -> Neuanwendung Seeding:
//   - Update-Database <NAME SEED-MIGRATION>
//   - nochmal Update-Database?

// https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/testing?view=aspnetcore-2.1
// Controller-Unit-Tests: nur Inhalte einer einzelnen Aktion getestet?
// - nicht enthalten: filters, routing, and model binding
// https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.1#introduction-to-integration-tests
// Unit-Tests: benutzen Mocks
// Integrationstests: benutzen *keine* Mocks, sondern die eigentlichen Objekte
// andererseits: wie mockt macn DbSet<Employee> ?
// ef-docs schlagen InMemory vor: https://docs.microsoft.com/en-us/aspnet/web-api/overview/testing-and-debugging/mocking-entity-framework-when-unit-testing-aspnet-web-api-2
// beachte: InMemory verhält sich gern mal anders https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/in-memory
// - erlaubt speichern von Daten, die gegen Constraint verstoßen
// beim eigentlichen Programm gibt .NET hingegen BadRequest()s zurück
// M$ sagt: Controller Unit Tests sollen nicht Modellvalidierung, sondern nur Verhalten bei Fehler testen
// https://docs.microsoft.com/de-de/aspnet/core/mvc/controllers/testing?view=aspnetcore-2.1

// wichtig: Testen gegen OkObjectResult erfordert Rückgabe mittles Ok(obj)
// beachte für Swagger und Tests https://docs.microsoft.com/en-us/aspnet/core/web-api/action-return-types?view=aspnetcore-2.1

// TODO Tests (inkl. Modellvalidierung)+
// TODO auf ungültige Patches prüfen (auch, ob Modell danach ok)
// TODO Swagger
// TODO Objekt bei [ProducesResponseType(400)] rein?
// TODO EmployeeContext-Testdaten raus

namespace EmployeeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        public EmployeeController(EmployeeContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public ActionResult<IEnumerable<Employee>> GetAll()
        {
            return base.Ok(this.context.Employees.ToList());
        }

        // ist es guter Stil, den Rückgabetyp zu variieren? (d.g. Guid vs. Employee)
        // macht m$ aber auch so
        [HttpGet("{id}", Name = "GetById")]
        [ProducesResponseType(200, Type = typeof(Employee))]
        [ProducesResponseType(404, Type = typeof(Guid))]
        public ActionResult<Employee> GetById(Guid id)
        {
            var employee = this.context.Employees.Find(id);
            if (employee == null)
            {
                return base.NotFound(id);
            }
            return base.Ok(employee);

        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Employee))]
        [ProducesResponseType(400, Type = typeof(ModelStateDictionary))]
        public ActionResult<Employee> Create(Employee employee)
        {
            // scheint nur für Test nötig zu sein?
            if (!base.ModelState.IsValid)
            {
                return base.BadRequest(base.ModelState);
            }
            this.context.Employees.Add(employee);
            this.context.SaveChanges();
            return base.CreatedAtAction("GetById", new { id = employee.Id }, employee);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(Employee))]
        [ProducesResponseType(404, Type = typeof(Guid))]
        public ActionResult<Employee> Delete(Guid id)
        {
            var employee = this.context.Employees.Find(id);
            if (employee == null)
            {
                return base.NotFound(id);
            }
            this.context.Employees.Remove(employee);
            this.context.SaveChanges();

            return base.Ok(employee);
        }

        // PATCH ist sehr bizarr und erwartet im JSON-Body eine Liste von kodierten Befehlen
        // siehe auch https://dotnetcoretutorials.com/2017/11/29/json-patch-asp-net-core/
        [HttpPatch("{id}")]
        [ProducesResponseType(200, Type = typeof(Employee))]
        [ProducesResponseType(404, Type = typeof(Guid))]
        public ActionResult<Employee> Patch(Guid id, JsonPatchDocument<Employee> patch)
        {
            var employee = this.context.Employees.Find(id);
            if (employee == null)
            {
                return base.NotFound(id);
            }
            patch.ApplyTo(employee);
            this.context.SaveChanges();

            return base.Ok(employee);
        }

        private readonly EmployeeContext context;
    }
}