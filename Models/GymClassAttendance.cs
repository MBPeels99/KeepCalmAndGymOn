// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-28-2023
// ***********************************************************************
// <copyright file="GymClassAttendance.cs" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeepCalmGymApplication.Models
{
    /// <summary>
    /// Class GymClassAttendance.
    /// </summary>
    public class GymClassAttendance
    {
        /// <summary>
        /// Gets or sets the class identifier.
        /// </summary>
        /// <value>The class identifier.</value>
        [ForeignKey("GymClass")]
        public int ClassID { get; set; }

        /// <summary>
        /// Gets or sets the member identifier.
        /// </summary>
        /// <value>The member identifier.</value>
        [ForeignKey("Member")]
        public int MemberID { get; set; }

        // Navigation properties
        /// <summary>
        /// Gets or sets the gym class.
        /// </summary>
        /// <value>The gym class.</value>
        public GymClass GymClass { get; set; }
        /// <summary>
        /// Gets or sets the member.
        /// </summary>
        /// <value>The member.</value>
        public Member Member { get; set; }
    }
}
