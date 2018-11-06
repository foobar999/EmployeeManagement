using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        public Guid Id { get; set; }

        [Required]
        public String FirstName { get; set; }

        [Required]
        public String SecondName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Employee employee &&
                   this.Id.Equals(employee.Id) &&
                   this.FirstName == employee.FirstName &&
                   this.SecondName == employee.SecondName &&
                   EqualityComparer<DateTime?>.Default.Equals(this.DateOfBirth, employee.DateOfBirth);
        }
    }
}
