// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-28-2023
// ***********************************************************************
// <copyright file="Member.cs" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.ComponentModel.DataAnnotations;

namespace KeepCalmGymApplication.Models
{
    /// <summary>
    /// Class Member.
    /// Implements the <see cref="KeepCalmGymApplication.Models.Person" />
    /// </summary>
    /// <seealso cref="KeepCalmGymApplication.Models.Person" />
    public class Member : Person
    {
        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>The date of birth.</value>
        [Display(Name = "Date of Birth")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        // Navigation property to the contract

        /// <summary>
        /// Gets or sets the gym attendances.
        /// </summary>
        /// <value>The gym attendances.</value>
        public ICollection<GymAttendance> GymAttendances { get; set; } = new List<GymAttendance>();
        /// <summary>
        /// Gets or sets the payments.
        /// </summary>
        /// <value>The payments.</value>
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        /// <summary>
        /// Gets or sets the gym class attendances.
        /// </summary>
        /// <value>The gym class attendances.</value>
        public ICollection<GymClassAttendance> GymClassAttendances { get; set; } = new List<GymClassAttendance>();

        //public Contract Contract { get; set; }
    }
}
