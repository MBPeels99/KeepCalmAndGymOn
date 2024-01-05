// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-28-2023
// ***********************************************************************
// <copyright file="LoginViewModel.cs" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.ComponentModel.DataAnnotations;

namespace KeepCalmGymApplication.ViewModels
{
    /// <summary>
    /// Class LoginViewModel.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        [Required]
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the type of the user.
        /// </summary>
        /// <value>The type of the user.</value>
        public string UserType { get; set; }  // Member or GymEmployee
    }
}
