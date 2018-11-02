using System;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        //[Required]
        // TODO gehört sowas hier hin oder in die SQL-Tabellendefinition?
        public Guid Id { get; set; }
        public String FirstName { get; set; }
        public String SecondName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
