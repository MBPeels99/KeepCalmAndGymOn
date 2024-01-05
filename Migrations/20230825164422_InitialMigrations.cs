// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-25-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-28-2023
// ***********************************************************************
// <copyright file="20230825164422_InitialMigrations.cs" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepCalmGymApplication.Migrations
{
    /// <summary>
    /// Represents the initial migrations for the application's database.
    /// </summary>
    public partial class InitialMigrations : Migration
    {
        /// <summary>
        /// Builds the database tables and relationships.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        /// <remarks><para>
        /// That is, builds the operations that will take the database from the state left in by the
        /// previous migration so that it is up-to-date with regard to this migration.
        /// </para>
        /// <para>
        /// This method must be overridden in each class that inherits from <see cref="T:Microsoft.EntityFrameworkCore.Migrations.Migration" />.
        /// </para>
        /// <para>
        /// See <see href="https://aka.ms/efcore-docs-migrations">Database migrations</see> for more information and examples.
        /// </para></remarks>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create GymEmployees table.
            migrationBuilder.CreateTable(
                name: "GymEmployees",
                columns: table => new
                {
                    PersonID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Speciality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Certification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactDetails = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymEmployees", x => x.PersonID);
                });
            // Create Members table.
            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    PersonID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactDetails = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.PersonID);
                });
            // Create Reports table.
            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    ReportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FunctionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChartType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LabelProperty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataProperty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackgroundColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BorderColor = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.ReportID);
                });
            // Create GymClasses table with foreign key to GymEmployees.
            migrationBuilder.CreateTable(
                name: "GymClasses",
                columns: table => new
                {
                    ClassID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstructorID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Time = table.Column<TimeSpan>(type: "time", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymClasses", x => x.ClassID);
                    table.ForeignKey(
                        name: "FK_GymClasses_GymEmployees_InstructorID",
                        column: x => x.InstructorID,
                        principalTable: "GymEmployees",
                        principalColumn: "PersonID",
                        onDelete: ReferentialAction.Restrict);
                });
            // Create Contracts table with foreign key to Members.
            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    ContractId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MembershipType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.ContractId);
                    table.ForeignKey(
                        name: "FK_Contracts_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "PersonID",
                        onDelete: ReferentialAction.Cascade);
                });
            // Create GymAttendances table with foreign key to Members.
            migrationBuilder.CreateTable(
                name: "GymAttendances",
                columns: table => new
                {
                    AttendanceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOut = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymAttendances", x => x.AttendanceID);
                    table.ForeignKey(
                        name: "FK_GymAttendances_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "PersonID",
                        onDelete: ReferentialAction.Restrict);
                });
            // Create Payments table with foreign key to Members.
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK_Payments_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "PersonID",
                        onDelete: ReferentialAction.Restrict);
                });
            // Create GymClassAttendances table with foreign keys to GymClasses and Members.
            migrationBuilder.CreateTable(
                name: "GymClassAttendances",
                columns: table => new
                {
                    ClassID = table.Column<int>(type: "int", nullable: false),
                    MemberID = table.Column<int>(type: "int", nullable: false),
                    GymClassClassID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymClassAttendances", x => new { x.ClassID, x.MemberID });
                    table.ForeignKey(
                        name: "FK_GymClassAttendances_GymClasses_ClassID",
                        column: x => x.ClassID,
                        principalTable: "GymClasses",
                        principalColumn: "ClassID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GymClassAttendances_GymClasses_GymClassClassID",
                        column: x => x.GymClassClassID,
                        principalTable: "GymClasses",
                        principalColumn: "ClassID");
                    table.ForeignKey(
                        name: "FK_GymClassAttendances_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "PersonID",
                        onDelete: ReferentialAction.Restrict);
                });
            // Indexes to optimize lookup performances.
            migrationBuilder.CreateIndex(
                name: "IX_Contracts_MemberId",
                table: "Contracts",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_GymAttendances_MemberID",
                table: "GymAttendances",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_GymClassAttendances_GymClassClassID",
                table: "GymClassAttendances",
                column: "GymClassClassID");

            migrationBuilder.CreateIndex(
                name: "IX_GymClassAttendances_MemberID",
                table: "GymClassAttendances",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_GymClasses_InstructorID",
                table: "GymClasses",
                column: "InstructorID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_MemberID",
                table: "Payments",
                column: "MemberID");
        }

        /// <summary>
        /// Drops the created tables, essentially reversing the operations done in the Up method.
        /// </summary>
        /// <param name="migrationBuilder">The migration builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "GymAttendances");

            migrationBuilder.DropTable(
                name: "GymClassAttendances");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "GymClasses");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "GymEmployees");
        }
    }
}
