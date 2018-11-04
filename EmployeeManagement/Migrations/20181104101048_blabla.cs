using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EmployeeManagement.Migrations
{
    public partial class blabla : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "DateOfBirth", "FirstName", "SecondName" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(1945, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hans", "Wurst" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "DateOfBirth", "FirstName", "SecondName" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(1933, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jim", "Beam" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));
        }
    }
}
