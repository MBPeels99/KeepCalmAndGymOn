// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-28-2023
// ***********************************************************************
// <copyright file="Contract.cs" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.ComponentModel.DataAnnotations;

namespace KeepCalmGymApplication.Models
{
    /// <summary>
    /// Class Contract.
    /// </summary>
    public class Contract
    {
        /// <summary>
        /// Gets or sets the contract identifier.
        /// </summary>
        /// <value>The contract identifier.</value>
        public int ContractId { get; set; }
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// Gets or sets the type of the membership.
        /// </summary>
        /// <value>The type of the membership.</value>
        public string MembershipType { get; set; }

        // You can add navigation properties here if needed in future
        /// <summary>
        /// Gets or sets the member identifier.
        /// </summary>
        /// <value>The member identifier.</value>
        public int MemberId { get; set; }
        /// <summary>
        /// Gets or sets the member.
        /// </summary>
        /// <value>The member.</value>
        public Member? Member { get; set; }

    }
}
