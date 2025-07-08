using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeDepartmentCRUDApp.Migrations
{
    /// <inheritdoc />
    public partial class AddModuleOrganizationMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Employees",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "Departments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationName = table.Column<string>(type: "TEXT", nullable: false),
                    OrganizationEmail = table.Column<string>(type: "TEXT", nullable: false),
                    OrganizationGSTNo = table.Column<string>(type: "TEXT", nullable: false),
                    OrganizationAddress = table.Column<string>(type: "TEXT", nullable: false),
                    OrganizationPublishedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_OrganizationId",
                table: "Employees",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_OrganizationId",
                table: "Departments",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Organizations_OrganizationId",
                table: "Departments",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Organizations_OrganizationId",
                table: "Employees",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Organizations_OrganizationId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Organizations_OrganizationId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Employees_OrganizationId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Departments_OrganizationId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Departments");
        }
    }
}
