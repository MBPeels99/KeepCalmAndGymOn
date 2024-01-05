// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-28-2023
// ***********************************************************************
// <copyright file="GymAttendanceController.cs" company="KeepCalmGymApplication">
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
    /// Represents a controller that handles gym attendance related operations.
    /// </summary>
    public class GymAttendanceController : Controller
    {
        /// <summary>
        /// The database context instance used for CRUD operations.
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="GymAttendanceController" /> class.
        /// </summary>
        /// <param name="context">The database context instance.</param>
        public GymAttendanceController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves the list of gym attendances.
        /// </summary>
        /// <returns>A view of gym attendances.</returns>
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.GymAttendances.Include(g => g.Member);
            return View(await appDbContext.ToListAsync());
        }

        /// <summary>
        /// Retrieves details of a specific gym attendance.
        /// </summary>
        /// <param name="id">The identifier of the gym attendance.</param>
        /// <returns>A view of the gym attendance details or NotFound if the ID does not exist.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GymAttendances == null)
            {
                return NotFound();
            }

            var gymAttendance = await _context.GymAttendances
                .Include(g => g.Member)
                .FirstOrDefaultAsync(m => m.AttendanceID == id);
            if (gymAttendance == null)
            {
                return NotFound();
            }

            return View(gymAttendance);
        }

        /// <summary>
        /// Renders the gym attendance creation view.
        /// </summary>
        /// <returns>A view for creating a new gym attendance.</returns>
        public IActionResult Create()
        {
            ViewData["MemberID"] = new SelectList(_context.Members, "PersonID", "PersonID");
            return View();
        }

        /// <summary>
        /// Handles the creation of a new gym attendance.
        /// </summary>
        /// <param name="gymAttendance">The gym attendance to be created.</param>
        /// <returns>Redirects to the index page or returns the creation view with errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AttendanceID,MemberID,Date,CheckIn,CheckOut")] GymAttendance gymAttendance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gymAttendance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberID"] = new SelectList(_context.Members, "PersonID", "PersonID", gymAttendance.MemberID);
            return View(gymAttendance);
        }

        /// <summary>
        /// Retrieves the edit view for a specific gym attendance.
        /// </summary>
        /// <param name="id">The identifier of the gym attendance to be edited.</param>
        /// <returns>The edit view of the gym attendance or NotFound if the ID does not exist.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GymAttendances == null)
            {
                return NotFound();
            }

            var gymAttendance = await _context.GymAttendances.FindAsync(id);
            if (gymAttendance == null)
            {
                return NotFound();
            }
            ViewData["MemberID"] = new SelectList(_context.Members, "PersonID", "PersonID", gymAttendance.MemberID);
            return View(gymAttendance);
        }

        /// <summary>
        /// Handles the editing of a gym attendance.
        /// </summary>
        /// <param name="id">The identifier of the gym attendance.</param>
        /// <param name="gymAttendance">The updated gym attendance data.</param>
        /// <returns>Redirects to the index page or returns the edit view with errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AttendanceID,MemberID,Date,CheckIn,CheckOut")] GymAttendance gymAttendance)
        {
            if (id != gymAttendance.AttendanceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymAttendance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymAttendanceExists(gymAttendance.AttendanceID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberID"] = new SelectList(_context.Members, "PersonID", "PersonID", gymAttendance.MemberID);
            return View(gymAttendance);
        }

        /// <summary>
        /// Retrieves the delete confirmation view for a specific gym attendance.
        /// </summary>
        /// <param name="id">The identifier of the gym attendance to be deleted.</param>
        /// <returns>The delete confirmation view or NotFound if the ID does not exist.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GymAttendances == null)
            {
                return NotFound();
            }

            var gymAttendance = await _context.GymAttendances
                .Include(g => g.Member)
                .FirstOrDefaultAsync(m => m.AttendanceID == id);
            if (gymAttendance == null)
            {
                return NotFound();
            }

            return View(gymAttendance);
        }

        /// <summary>
        /// Handles the deletion of a gym attendance.
        /// </summary>
        /// <param name="id">The identifier of the gym attendance to be deleted.</param>
        /// <returns>Redirects to the index page.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GymAttendances == null)
            {
                return Problem("Entity set 'AppDbContext.GymAttendances'  is null.");
            }
            var gymAttendance = await _context.GymAttendances.FindAsync(id);
            if (gymAttendance != null)
            {
                _context.GymAttendances.Remove(gymAttendance);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if a gym attendance exists.
        /// </summary>
        /// <param name="id">The identifier of the gym attendance.</param>
        /// <returns>True if the gym attendance exists, otherwise false.</returns>
        private bool GymAttendanceExists(int id)
        {
          return (_context.GymAttendances?.Any(e => e.AttendanceID == id)).GetValueOrDefault();
        }
    }
}
