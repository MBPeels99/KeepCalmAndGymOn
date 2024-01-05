// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-28-2023
// ***********************************************************************
// <copyright file="GymAttendance.cs" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.ComponentModel.DataAnnotations;

namespace KeepCalmGymApplication.Models
{
    /// <summary>
    /// Class GymAttendance.
    /// </summary>
    public class GymAttendance
    {
        /// <summary>
        /// Gets or sets the attendance identifier.
        /// </summary>
        /// <value>The attendance identifier.</value>
        [Key]
        public int AttendanceID { get; set; }
        /// <summary>
        /// Gets or sets the member identifier.
        /// </summary>
        /// <value>The member identifier.</value>
        public int MemberID { get; set; }
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        public DateTime Date { get; set; }
        /// <summary>
        /// Gets or sets the check in.
        /// </summary>
        /// <value>The check in.</value>
        public DateTime CheckIn { get; set; }
        /// <summary>
        /// Gets or sets the check out.
        /// </summary>
        /// <value>The check out.</value>
        public DateTime CheckOut { get; set; }

        // Navigation property
        /// <summary>
        /// Gets or sets the member.
        /// </summary>
        /// <value>The member.</value>
        public Member Member { get; set; }
    }
}
