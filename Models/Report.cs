// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-25-2023
// ***********************************************************************
// <copyright file="Report.cs" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.ComponentModel.DataAnnotations;

namespace KeepCalmGymApplication.Models
{
    /// <summary>
    /// Class Report.
    /// </summary>
    public class Report
    {
        /// <summary>
        /// Gets or sets the report identifier.
        /// </summary>
        /// <value>The report identifier.</value>
        [Key]
        public int ReportID { get; set; }

        /// <summary>
        /// Gets or sets the name of the report.
        /// </summary>
        /// <value>The name of the report.</value>
        [Display(Name = "Report Name")]
        public string ReportName { get; set; }  // Changed from ReportType to be more descriptive

        /// <summary>
        /// Gets or sets the name of the function.
        /// </summary>
        /// <value>The name of the function.</value>
        public string FunctionName { get; set; }  // Added this field to store function name
        /// <summary>
        /// Gets or sets the type of the chart.
        /// </summary>
        /// <value>The type of the chart.</value>
        public string ChartType { get; set; }  // Replaces "Parameters" for clarity

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        [Display(Name = "Label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the label property.
        /// </summary>
        /// <value>The label property.</value>
        [Display(Name = "Label Property")]
        public string LabelProperty { get; set; }

        /// <summary>
        /// Gets or sets the data property.
        /// </summary>
        /// <value>The data property.</value>
        [Display(Name = "Data Property")]
        public string DataProperty { get; set; }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        [Display(Name = "Background Color")]
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>The color of the border.</value>
        [Display(Name = "Border Color")]
        public string BorderColor { get; set; }
    }
}
