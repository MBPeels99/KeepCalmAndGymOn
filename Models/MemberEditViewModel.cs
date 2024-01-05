// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-28-2023
// ***********************************************************************
// <copyright file="MemberEditViewModel.cs" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace KeepCalmGymApplication.Models
{
    /// <summary>
    /// Class MemberEditViewModel.
    /// Implements the <see cref="KeepCalmGymApplication.Models.Member" />
    /// </summary>
    /// <seealso cref="KeepCalmGymApplication.Models.Member" />
    public class MemberEditViewModel : Member
    {
        /// <summary>
        /// Creates new password.
        /// </summary>
        /// <value>The new password.</value>
        public string NewPassword { get; set; }
    }

}
