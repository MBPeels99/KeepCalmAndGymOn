// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-28-2023
// ***********************************************************************
// <copyright file="GymEmployeesController.cs" company="KeepCalmGymApplication">
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
using Microsoft.Extensions.Logging;


namespace KeepCalmGymApplication.Controllers
{
    /// <summary>
    /// Provides methods for CRUD operations related to Gym Employees.
    /// </summary>
    public class GymEmployeesController : Controller
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly AppDbContext _context;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GymEmployeesController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GymEmployeesController" /> class.
        /// </summary>
        /// <param name="context">The application's database context.</param>
        /// <param name="logger">The logger instance.</param>
        public GymEmployeesController(AppDbContext context, ILogger<GymEmployeesController> logger)
        {
            _context = context;
            _logger = logger;
        }


        /// <summary>
        /// Fetches and displays a list of gym employees.
        /// </summary>
        /// <returns>The index view with a list of gym employees.</returns>
        public async Task<IActionResult> Index()
        {
              return _context.GymEmployees != null ? 
                          View(await _context.GymEmployees.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.GymEmployees'  is null.");
        }

        /// <summary>
        /// Fetches details for a specific gym employee.
        /// </summary>
        /// <param name="id">The ID of the gym employee to retrieve details for.</param>
        /// <returns>The details view of the specified gym employee.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GymEmployees == null)
            {
                return NotFound();
            }

            var gymEmployee = await _context.GymEmployees
                .FirstOrDefaultAsync(m => m.PersonID == id);
            if (gymEmployee == null)
            {
                return NotFound();
            }

            return View(gymEmployee);
        }

        /// <summary>
        /// Displays the form to create a new gym employee.
        /// </summary>
        /// <returns>The create view.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Attempts to create a new gym employee in the database.
        /// </summary>
        /// <param name="gymEmployee">The gym employee data to add.</param>
        /// <returns>A result indicating the success or failure of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GymEmployee gymEmployee)
        {
            if (ModelState.IsValid)
            {

                // Use BCrypt to hash the password
                gymEmployee.Password = BCrypt.Net.BCrypt.HashPassword(gymEmployee.Password);

                try
                {
                    _context.Add(gymEmployee);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Created the gym employee. ModelState is valid.");

                    HttpContext.Session.SetInt32("PersonID", gymEmployee.PersonID);
                    HttpContext.Session.SetString("UserName", gymEmployee.Username);
                    HttpContext.Session.SetString("UserType", "GymEmployee");
                    return Json(new { success = true, redirectUrl = Url.Action("Details", new { id = gymEmployee.PersonID }) });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to create the gym employee: {ex.Message}");
                    return Content($"Error: {ex.Message}");
                }
            }
            else
            {
                var errorList = ModelState.SelectMany(m => m.Value.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();
                _logger.LogError($"Failed to create the gym employee. ModelState errors: {string.Join(", ", errorList)}");
                return BadRequest(errorList); // Send the errors back as a response
            }
        }



        /// <summary>
        /// Displays the form to edit an existing gym employee.
        /// </summary>
        /// <param name="id">The ID of the gym employee to edit.</param>
        /// <returns>The edit view for the specified gym employee.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GymEmployees == null)
            {
                return NotFound();
            }

            var gymEmployee = await _context.GymEmployees.FindAsync(id);
            if (gymEmployee == null)
            {
                return NotFound();
            }
            return View(gymEmployee);
        }

        /// <summary>
        /// Attempts to update a gym employee in the database.
        /// </summary>
        /// <param name="id">The ID of the gym employee to update.</param>
        /// <param name="gymEmployee">The updated gym employee data.</param>
        /// <returns>A result indicating the success or failure of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [FromBody] GymEmployee gymEmployee)
        {
            if (id != gymEmployee.PersonID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymEmployee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError($"Failed to update the gym employee: {ex.Message}");
                    if (!GymEmployeeExists(gymEmployee.PersonID))
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
            return View(gymEmployee);
        }


        /// <summary>
        /// Displays the confirmation page to delete a gym employee.
        /// </summary>
        /// <param name="id">The ID of the gym employee to delete.</param>
        /// <returns>The delete confirmation view for the specified gym employee.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GymEmployees == null)
            {
                return NotFound();
            }

            var gymEmployee = await _context.GymEmployees
                .FirstOrDefaultAsync(m => m.PersonID == id);
            if (gymEmployee == null)
            {
                return NotFound();
            }

            return View(gymEmployee);
        }

        /// <summary>
        /// Attempts to delete a gym employee from the database.
        /// </summary>
        /// <param name="id">The ID of the gym employee to delete.</param>
        /// <returns>A result indicating the success or failure of the operation.</returns>
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GymEmployees == null)
            {
                return Problem("Entity set 'AppDbContext.GymEmployees'  is null.");
            }
            var gymEmployee = await _context.GymEmployees.FindAsync(id);
            if (gymEmployee != null)
            {
                _context.GymEmployees.Remove(gymEmployee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if a gym employee exists in the database.
        /// </summary>
        /// <param name="id">The ID of the gym employee to check.</param>
        /// <returns>True if the gym employee exists, false otherwise.</returns>
        private bool GymEmployeeExists(int id)
        {
          return (_context.GymEmployees?.Any(e => e.PersonID == id)).GetValueOrDefault();
        }
    }
}
