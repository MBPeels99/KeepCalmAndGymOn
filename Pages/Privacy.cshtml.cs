// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-24-2023
// ***********************************************************************
// <copyright file="Privacy.cshtml.cs" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KeepCalmGymApplication.Pages
{
    /// <summary>
    /// Class PrivacyModel.
    /// Implements the <see cref="PageModel" />
    /// </summary>
    /// <seealso cref="PageModel" />
    public class PrivacyModel : PageModel
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<PrivacyModel> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivacyModel"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Called when [get].
        /// </summary>
        public void OnGet()
        {
        }
    }
}