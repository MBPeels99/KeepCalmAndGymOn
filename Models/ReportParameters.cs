// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-25-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-25-2023
// ***********************************************************************
// <copyright file="ReportParameters.cs" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace KeepCalmGymApplication.Models
{
    /// <summary>
    /// Class ReportParameters.
    /// </summary>
    public class ReportParameters
    {
        /// <summary>
        /// Gets or sets the name of the function.
        /// </summary>
        /// <value>The name of the function.</value>
        public string FunctionName { get; set; }
        /// <summary>
        /// Gets or sets the name of the json file.
        /// </summary>
        /// <value>The name of the json file.</value>
        public string JsonFileName { get; set; }
        /// <summary>
        /// Gets or sets the data property.
        /// </summary>
        /// <value>The data property.</value>
        public string DataProperty { get; set; }
        /// <summary>
        /// Gets or sets the label property.
        /// </summary>
        /// <value>The label property.</value>
        public string LabelProperty { get; set; }
    }
}

