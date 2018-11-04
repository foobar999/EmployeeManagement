using System;
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
    }
}
