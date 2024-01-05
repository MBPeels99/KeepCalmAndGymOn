// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-28-2023
// ***********************************************************************
// <copyright file="ReportController.cs" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KeepCalmGymApplication.App_Data;
using KeepCalmGymApplication.Models;
using System.Globalization;
using Newtonsoft.Json;

namespace KeepCalmGymApplication.Controllers
{
    /// <summary>
    /// Represents a controller for handling report operations.
    /// </summary>
    public class ReportController : Controller
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportController" /> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public ReportController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns the index view of reports.
        /// </summary>
        /// <returns>The index view of reports.</returns>
        public async Task<IActionResult> Index()
        {
              return _context.Reports != null ? 
                          View(await _context.Reports.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.Reports'  is null.");
        }

        // GET: Report/Details/5
        /// <summary>
        /// Detailses the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IActionResult.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reports == null)
            {
                return NotFound();
            }

            var report = await _context.Reports
                .FirstOrDefaultAsync(m => m.ReportID == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }
        /// <summary>
        /// Reports the exists.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool ReportExists(int id)
        {
          return (_context.Reports?.Any(e => e.ReportID == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Retrieves the parameters for a specific report.
        /// </summary>
        /// <param name="reportId">The identifier of the report.</param>
        /// <returns>The parameters of the specified report.</returns>
        [HttpGet]
        public async Task<IActionResult> GetReportParameters(int reportId)
        {
            var report = await _context.Reports.FirstOrDefaultAsync(r => r.ReportID == reportId);
            if (report == null)
            {
                return NotFound();
            }

            return Json(new
            {
                functionName = report.FunctionName,
                ReportType = new
                {
                    type = report.ChartType,
                    label = report.Label,
                    labelProperty = report.LabelProperty,
                    dataProperty = report.DataProperty,
                    backgroundColor = report.BackgroundColor,
                    borderColor = report.BorderColor
                }
            });
        }


        /// <summary>
        /// Retrieves membership growth data over a specified time.
        /// </summary>
        /// <param name="year">The year for which to retrieve the data.</param>
        /// <returns>The membership growth data for the specified year.</returns>
        [HttpGet]
        public async Task<IActionResult> MembershipGrowthOverTime(int year)
        {
            var contractDataRaw = await _context.Contracts
                .Where(c => c.StartDate.Year == year)
                .Select(c => new { c.StartDate })
                .ToListAsync();

            var contractData = contractDataRaw
                .GroupBy(c => new { Year = c.StartDate.Year, Month = c.StartDate.Month })
                .Select(group => new
                {
                    Date = $"{group.Key.Year}-{group.Key.Month.ToString("00", CultureInfo.InvariantCulture)}",
                    Count = group.Count()
                })
                .OrderBy(result => result.Date)
                .ToList();

            return Json(contractData);
        }

        /// <summary>
        /// Retrieves labels for function names.
        /// </summary>
        /// <returns>The labels for function names.</returns>
        [HttpGet]
        public IActionResult GetLabelsFunctionName()
        {
            // Here, I'm using a basic approach to return all months. This might need adjustments based on your use case.
            var labels = Enumerable.Range(1, 12).Select(month => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month));
            return Json(labels);
        }


        /// <summary>
        /// Retrieves attendance trend data over a specified time.
        /// </summary>
        /// <param name="year">The year for which to retrieve the data.</param>
        /// <returns>The attendance trend data for the specified year.</returns>
        [HttpGet]
        public async Task<IActionResult> AttendanceTrendOverTime(int year)
        {
            var attendanceDataRaw = await _context.GymAttendances
                                                  .Where(a => a.Date.Year == year)
                                                  .ToListAsync();

            var attendanceDataProcessed = attendanceDataRaw
                .GroupBy(a => new { Year = a.Date.Year, Month = a.Date.Month })
                .Select(group => new
                {
                    Date = $"{group.Key.Year}-{group.Key.Month.ToString("00", CultureInfo.InvariantCulture)}",
                    Count = group.Count()
                })
                .OrderBy(result => result.Date)
                .ToList();

            return Json(attendanceDataProcessed);
        }


        /// <summary>
        /// Retrieves membership revenue data over a specified time.
        /// </summary>
        /// <param name="year">The year for which to retrieve the data.</param>
        /// <returns>The membership revenue data for the specified year.</returns>
        [HttpGet]
        public async Task<IActionResult> MembershipRevenueOverTime(int year)
        {
            var revenueDataRaw = await _context.Payments
                                               .Where(p => p.Date.Year == year)
                                               .ToListAsync();

            var revenueDataProcessed = revenueDataRaw
                .GroupBy(p => new { Year = p.Date.Year, Month = p.Date.Month })
                .Select(group => new
                {
                    Date = $"{group.Key.Year}-{group.Key.Month.ToString("00", CultureInfo.InvariantCulture)}",
                    Revenue = group.Sum(p => p.Amount)
                })
                .OrderBy(result => result.Date)
                .ToList();

            return Json(revenueDataProcessed);
        }


        /// <summary>
        /// Retrieves the popular gym classes for a specific year.
        /// </summary>
        /// <param name="year">The year for which to retrieve the data.</param>
        /// <returns>The popular gym classes for the specified year.</returns>
        [HttpGet]
        public async Task<IActionResult> PopularGymClassesForYear(int year)
        {
            var classDataProcessed = await _context.GymClasses
                .Where(gc => gc.Date.Year == year)
                .Join(_context.GymClassAttendances,
                      gc => gc.ClassID,
                      gca => gca.ClassID,
                      (gc, gca) => gc.ClassName)
                .GroupBy(className => className)
                .Select(group => new
                {
                    ClassName = group.Key,
                    AttendanceCount = group.Count()
                })
                .OrderByDescending(result => result.AttendanceCount)
                .Take(5)
                .ToListAsync();

            return Json(classDataProcessed);
        }

    }
}
