// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-28-2023
// ***********************************************************************
// <copyright file="GymClass.cs" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KeepCalmGymApplication.Models
{
    /// <summary>
    /// Class GymClass.
    /// </summary>
    public class GymClass
    {
        /// <summary>
        /// Gets or sets the class identifier.
        /// </summary>
        /// <value>The class identifier.</value>
        [Key]
        public int ClassID { get; set; }
        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
        /// <value>The name of the class.</value>
        public string ClassName { get; set; }
        /// <summary>
        /// Gets or sets the instructor identifier.
        /// </summary>
        /// <value>The instructor identifier.</value>
        [ForeignKey("Instructor")]
        public int InstructorID { get; set; }
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>The time.</value>
        public TimeSpan Time { get; set; }
        /// <summary>
        /// Gets or sets the capacity.
        /// </summary>
        /// <value>The capacity.</value>
        public int Capacity { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>The category.</value>
        public string Category { get; set; }


        // Navigation property for Instructor (assuming Instructor is a type of Member)
        /// <summary>
        /// Gets or sets the instructor.
        /// </summary>
        /// <value>The instructor.</value>
        public GymEmployee Instructor { get; set; }
        /// <summary>
        /// Gets or sets the gym class attendances.
        /// </summary>
        /// <value>The gym class attendances.</value>
        public ICollection<GymClassAttendance> GymClassAttendances { get; set; }

    }
}
