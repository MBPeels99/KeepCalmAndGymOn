// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-24-2023
// ***********************************************************************
// <copyright file="AppDbContext.cs" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using KeepCalmGymApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace KeepCalmGymApplication.App_Data
{
    /// <summary>
    /// Class AppDbContext.
    /// Implements the <see cref="DbContext" />
    /// </summary>
    /// <seealso cref="DbContext" />
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Gets or sets the contracts.
        /// </summary>
        /// <value>The contracts.</value>
        public DbSet<Contract> Contracts { get; set; }
        /// <summary>
        /// Gets or sets the members.
        /// </summary>
        /// <value>The members.</value>
        public DbSet<Member> Members { get; set; }
        /// <summary>
        /// Gets or sets the gym employees.
        /// </summary>
        /// <value>The gym employees.</value>
        public DbSet<GymEmployee> GymEmployees { get; set; }
        /// <summary>
        /// Gets or sets the gym attendances.
        /// </summary>
        /// <value>The gym attendances.</value>
        public DbSet<GymAttendance> GymAttendances { get; set; }
        /// <summary>
        /// Gets or sets the payments.
        /// </summary>
        /// <value>The payments.</value>
        public DbSet<Payment> Payments { get; set; }
        /// <summary>
        /// Gets or sets the gym class attendances.
        /// </summary>
        /// <value>The gym class attendances.</value>
        public DbSet<GymClassAttendance> GymClassAttendances { get; set; }
        /// <summary>
        /// Gets or sets the gym classes.
        /// </summary>
        /// <value>The gym classes.</value>
        public DbSet<GymClass> GymClasses { get; set; }
        /// <summary>
        /// Gets or sets the reports.
        /// </summary>
        /// <value>The reports.</value>
        public DbSet<Report> Reports { get; set; }


        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<GymClassAttendance>()
                .HasOne(gca => gca.Member)
                .WithMany(m => m.GymClassAttendances)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<GymAttendance>()
                .HasOne(p => p.Member)
                .WithMany(b => b.GymAttendances)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Member)
                .WithMany(b => b.Payments)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GymClass>()
                .HasOne(gc => gc.Instructor)
                .WithMany(ge => ge.GymClasses)
                .HasForeignKey(gc => gc.InstructorID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GymClassAttendance>()
                .HasOne(gca => gca.GymClass)
                .WithMany()
                .HasForeignKey(gca => gca.ClassID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GymClassAttendance>()
                .HasKey(gca => new { gca.ClassID, gca.MemberID });

        }


    }
}
