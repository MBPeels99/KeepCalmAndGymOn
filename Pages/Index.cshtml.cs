// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-24-2023
// ***********************************************************************
// <copyright file="Index.cshtml.cs" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KeepCalmGymApplication.Pages
{
    /// <summary>
    /// Class IndexModel.
    /// Implements the <see cref="PageModel" />
    /// </summary>
    /// <seealso cref="PageModel" />
    public class IndexModel : PageModel
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<IndexModel> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexModel"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public IndexModel(ILogger<IndexModel> logger)
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