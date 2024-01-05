// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-28-2023
// ***********************************************************************
// <copyright file="GymEmployee.cs" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace KeepCalmGymApplication.Models
{
    /// <summary>
    /// Class GymEmployee.
    /// Implements the <see cref="KeepCalmGymApplication.Models.Person" />
    /// </summary>
    /// <seealso cref="KeepCalmGymApplication.Models.Person" />
    public class GymEmployee : Person
    {
        /// <summary>
        /// Gets or sets the speciality.
        /// </summary>
        /// <value>The speciality.</value>
        public string? Speciality { get; set; }
        /// <summary>
        /// Gets or sets the certification.
        /// </summary>
        /// <value>The certification.</value>
        public string? Certification { get; set; }

        /// <summary>
        /// Gets or sets the gym classes.
        /// </summary>
        /// <value>The gym classes.</value>
        public ICollection<GymClass> GymClasses { get; set; } = new List<GymClass>();
    }

}
