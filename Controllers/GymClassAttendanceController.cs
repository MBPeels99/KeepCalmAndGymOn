// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-28-2023
// ***********************************************************************
// <copyright file="GymClassAttendanceController.cs" company="KeepCalmGymApplication">
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

namespace KeepCalmGymApplication.Controllers
{
    /// <summary>
    /// Handles the operations related to gym class attendance.
    /// </summary>
    public class GymClassAttendanceController : Controller
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="GymClassAttendanceController" /> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public GymClassAttendanceController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of available gym classes that aren't full.
        /// </summary>
        /// <returns>A view with the list of available classes.</returns>
        public async Task<IActionResult> Index()
        {
            // Fetch available classes that aren't full
            var availableClasses = await _context.GymClasses
                .Where(g => g.GymClassAttendances.Count < g.Capacity)
                .Include(g => g.Instructor)
                .ToListAsync();
            return View(availableClasses);
        }


        /// <summary>
        /// Displays the details of a specific gym class attendance.
        /// </summary>
        /// <param name="id">The class ID.</param>
        /// <returns>A view with the details of the gym class attendance.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GymClassAttendances == null)
            {
                return NotFound();
            }

            var gymClassAttendance = await _context.GymClassAttendances
                .Include(g => g.GymClass)
                .Include(g => g.Member)
                .FirstOrDefaultAsync(m => m.ClassID == id);
            if (gymClassAttendance == null)
            {
                return NotFound();
            }

            return View(gymClassAttendance);
        }

        /// <summary>
        /// Displays the form for creating a new gym class attendance.
        /// </summary>
        /// <returns>A view with the form.</returns>
        public IActionResult Create()
        {
            ViewData["ClassID"] = new SelectList(_context.GymClasses, "ClassID", "ClassID");
            ViewData["MemberID"] = new SelectList(_context.Members, "PersonID", "PersonID");
            return View();
        }

        /// <summary>
        /// Processes the submission of a new gym class attendance.
        /// </summary>
        /// <param name="gymClassAttendance">The gym class attendance to create.</param>
        /// <returns>A redirection to the index page on success, otherwise the form view with the current data.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClassID,MemberID")] GymClassAttendance gymClassAttendance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gymClassAttendance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassID"] = new SelectList(_context.GymClasses, "ClassID", "ClassID", gymClassAttendance.ClassID);
            ViewData["MemberID"] = new SelectList(_context.Members, "PersonID", "PersonID", gymClassAttendance.MemberID);
            return View(gymClassAttendance);
        }

        /// <summary>
        /// Displays the form for editing a gym class attendance.
        /// </summary>
        /// <param name="id">The ID of the gym class attendance to edit.</param>
        /// <returns>A view with the form populated with the current data of the gym class attendance.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GymClassAttendances == null)
            {
                return NotFound();
            }

            var gymClassAttendance = await _context.GymClassAttendances.FindAsync(id);
            if (gymClassAttendance == null)
            {
                return NotFound();
            }
            ViewData["ClassID"] = new SelectList(_context.GymClasses, "ClassID", "ClassID", gymClassAttendance.ClassID);
            ViewData["MemberID"] = new SelectList(_context.Members, "PersonID", "PersonID", gymClassAttendance.MemberID);
            return View(gymClassAttendance);
        }

        /// <summary>
        /// Allows a member to join or leave a class.
        /// </summary>
        /// <param name="id">The ID of the class to join or leave.</param>
        /// <returns>A redirection to the index page.</returns>
        [HttpPost]
        public async Task<IActionResult> JoinClass(int id)
        {
            var personId = HttpContext.Session.GetInt32("PersonID");

            var existingAttendance = await _context.GymClassAttendances
                        .FirstOrDefaultAsync(a => a.ClassID == id && a.MemberID == personId);

            if (existingAttendance != null)
            {
                // Remove existing attendance
                _context.GymClassAttendances.Remove(existingAttendance);
            }
            else
            {
                // Add new attendance
                var attendance = new GymClassAttendance { ClassID = id, MemberID = (int)personId };
                _context.GymClassAttendances.Add(attendance);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Gets the IDs of the classes that a member has joined.
        /// </summary>
        /// <returns>A JSON array of the class IDs.</returns>
        [HttpGet]
        public async Task<IActionResult> GetJoinedClasses()
        {
            var personId = HttpContext.Session.GetInt32("PersonID");
            var joinedClassIds = await _context.GymClassAttendances
                             .Where(a => a.MemberID == personId)
                             .Select(a => a.ClassID)
                             .ToListAsync();
            return Json(joinedClassIds);
        }


    }
}
