using Microsoft.EntityFrameworkCore;
using System;

namespace EmployeeManagement.Models
{
    public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Employee>().HasData(
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