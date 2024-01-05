// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-28-2023
// ***********************************************************************
// <copyright file="GymClassController.cs" company="KeepCalmGymApplication">
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
using Newtonsoft.Json;
using System.IO;

namespace KeepCalmGymApplication.Controllers
{
    /// <summary>
    /// Controller for managing gym classes.
    /// </summary>
    public class GymClassController : Controller
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly AppDbContext _context;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<GymClassController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GymClassController" /> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="logger">The logger instance.</param>
        public GymClassController(AppDbContext context, ILogger<GymClassController> logger)
        {
            _context = context;
            _logger = logger;
        }


        /// <summary>
        /// GET method for gym class index page.
        /// </summary>
        /// <returns>The view with list of gym classes.</returns>
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.GymClasses.Include(g => g.Instructor);
            return View(await appDbContext.ToListAsync());
        }

        /// <summary>
        /// GET method for gym class details.
        /// </summary>
        /// <param name="id">The gym class identifier.</param>
        /// <returns>The view of specific gym class details.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GymClasses == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses
                .Include(g => g.Instructor)
                .FirstOrDefaultAsync(m => m.ClassID == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        /// <summary>
        /// GET method for gym class creation page.
        /// </summary>
        /// <returns>The view to create a new gym class.</returns>
        public async Task<IActionResult> Create()
        {
            ViewData["InstructorID"] = await GetGymEmployeeSelectList();
            return View();
        }

        /// <summary>
        /// POST method to create a new gym class.
        /// </summary>
        /// <param name="gymClassDTO">The gym class data transfer object.</param>
        /// <returns>JSON result indicating the status of the creation process.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GymClassDTO gymClassDTO)
        {
            // Convert DTO to actual model
            GymClass gymClass = new GymClass
            {
                ClassName = gymClassDTO.ClassName,
                InstructorID = gymClassDTO.InstructorID,
                Date = DateTime.Parse(gymClassDTO.Date),
                Time = TimeSpan.Parse(gymClassDTO.Time),
                Capacity = gymClassDTO.Capacity,
                Category = gymClassDTO.Category  
            };


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(gymClass);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("GymClass created successfully.");
                    return Json(new { success = true, redirectUrl = Url.Action("Details", new { id = gymClass.ClassID }) });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error creating GymClass: {ex.Message}");
                    return Json(new { success = false, message = ex.Message });
                }
            }
            else
            {
                _logger.LogError("/GymClass/Create is not valid");
            }

            var errorList = ModelState.SelectMany(m => m.Value.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();
            _logger.LogError($"Failed to create GymClass. ModelState errors: {string.Join(", ", errorList)}");
            return Json(new { success = false, errorMessage = string.Join(", ", errorList) });
        }

        /// <summary>
        /// GET method for gym class edit page.
        /// </summary>
        /// <param name="id">The gym class identifier.</param>
        /// <returns>The view to edit the specific gym class.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GymClasses == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }

            ViewData["InstructorID"] = await GetGymEmployeeSelectList();
            return View(gymClass);
        }

        /// <summary>
        /// POST method to update a gym class.
        /// </summary>
        /// <param name="id">The gym class identifier.</param>
        /// <param name="gymClass">The gym class model.</param>
        /// <returns>JSON result indicating the status of the update process.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [FromBody] GymClass gymClass)
        {
            if (id != gymClass.ClassID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymClass);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"GymClass with ID {id} updated successfully.");
                    return Json(new { success = true, redirectUrl = Url.Action("Details", new { id = gymClass.ClassID }) });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError($"Concurrency Exception while updating GymClass with ID {id}: {ex.Message}");
                    if (!GymClassExists(gymClass.ClassID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error updating GymClass: {ex.Message}");
                    return Json(new { success = false, message = ex.Message });
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            _logger.LogError($"Validation errors while updating GymClass: {string.Join(", ", errors)}");
            return Json(new { success = false, errorMessage = string.Join(", ", errors) });
        }

        /// <summary>
        /// POST method to confirm and delete a gym class.
        /// </summary>
        /// <param name="id">The gym class identifier.</param>
        /// <returns>Redirect to the gym class index page.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GymClasses == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses
                .Include(g => g.Instructor)
                .FirstOrDefaultAsync(m => m.ClassID == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        /// <summary>
        /// POST method to confirm and delete a gym class.
        /// </summary>
        /// <param name="id">The gym class identifier.</param>
        /// <returns>Redirect to the gym class index page.</returns>

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GymClasses == null)
            {
                return Problem("Entity set 'AppDbContext.GymClasses'  is null.");
            }
            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass != null)
            {
                _context.GymClasses.Remove(gymClass);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // <summary>
        /// <summary>
        /// Gyms the class exists.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <font color="red">Badly formed XML comment.</font>
        private bool GymClassExists(int id)
        {
            return (_context.GymClasses?.Any(e => e.ClassID == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Gets the classes for the calendar view.
        /// </summary>
        /// <returns>JSON result of gym classes for calendar view.</returns>
        [HttpGet]
        public IActionResult GetClassesForCalendar()
        {
            var gymClasses = _context.GymClasses.ToList();

            var events = gymClasses.Select(g => new
            {
                id = g.ClassID,
                title = g.ClassName,
                start = g.Date.Add(g.Time),
                end = g.Date.Add(g.Time).AddHours(1),
                color = "blue"
            }).ToList();

            return Json(events);
        }

        /// <summary>
        /// Gets the list of gym instructors.
        /// </summary>
        /// <returns>JSON result of gym instructors.</returns>
        [HttpGet]
        public async Task<IActionResult> GetInstructors()
        {
            var instructors = await GetGymEmployeeSelectList();
            return Json(instructors);
        }


        /// <summary>
        /// Gets the list of gym employees for a selection dropdown.
        /// </summary>
        /// <returns>List of gym employees as <see cref="SelectListItem" />.</returns>
        private async Task<IEnumerable<SelectListItem>> GetGymEmployeeSelectList()
        {
            Console.WriteLine("GetGymEMployeeSelectList is being called");
            return await _context.GymEmployees
                                 .Select(e => new SelectListItem
                                 {
                                     Value = e.PersonID.ToString(),
                                     Text = $"{e.FirstName} {e.LastName}"
                                 })
                                 .ToListAsync();
        }
    }
}
