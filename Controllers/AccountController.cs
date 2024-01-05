// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-28-2023
// ***********************************************************************
// <copyright file="AccountController.cs" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using KeepCalmGymApplication.App_Data;
using KeepCalmGymApplication.Models;
using KeepCalmGymApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace KeepCalmGymApplication.Controllers
{
    /// <summary>
    /// Represents the Account Controller handling user authentication and related activities.
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// The database context instance used for CRUD operations.
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController" /> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Renders the login view.
        /// </summary>
        /// <returns>The login view.</returns>
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Handles the POST request for user login.
        /// </summary>
        /// <param name="model">The login view model containing user inputs.</param>
        /// <returns>Redirects to the user's details page if successful, otherwise returns the login view.</returns>
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            // Validate the provided model data
            if (ModelState.IsValid)
            {
                Person person = null;

                // Determine user authentication based on UserType
                // Check if the user is a Member
                if (model.UserType == "Member")
                {
                    person = _context.Members.SingleOrDefault(m => m.Username == model.Username);
                }
                // Check if the user is a GymEmployee
                else if (model.UserType == "GymEmployee")
                {
                    person = _context.GymEmployees.SingleOrDefault(e => e.Username == model.Username);
                }

                // If we found a matching person in the database
                if (person != null)
                {
                    bool validPassword = false;

                    // Verify the hashed password with the one entered by the user
                    try
                    {
                        validPassword = BCrypt.Net.BCrypt.Verify(model.Password, person.Password.ToString());
                    }
                    catch (Exception ex)
                    {
                        // Add error message for the frontend user
                        ModelState.AddModelError("", "Error while processing login. Please try again later.");
                        // Consider logging the exception for debugging purposes
                        // TODO: Log the exception for debugging
                    }

                    // If password verification was successful
                    if (validPassword)
                    {
                        var existingUserType = HttpContext.Session.GetString("UserType");

                        // Check if user details are not already stored in session
                        if (existingUserType.IsNullOrEmpty())
                        {
                            // Cache the user details in session for subsequent requests
                            HttpContext.Session.SetInt32("PersonID", person.PersonID);
                            HttpContext.Session.SetString("UserName", person.Username);
                            HttpContext.Session.SetString("UserType", model.UserType);
                        }

                        // If user is a member, handle their attendance data
                        if (model.UserType == "Member")
                        {
                            // Update the member's check-out time if needed
                            UpdateGymAttendanceCheckOut(person.PersonID);

                            // Register the member's current visit
                            _context.GymAttendances.Add(new GymAttendance
                            {
                                MemberID = person.PersonID,
                                Date = DateTime.Today,
                                CheckIn = DateTime.Now,
                                CheckOut = DateTime.Now
                            });

                            // Persist the attendance data in the database
                            _context.SaveChanges();
                        }

                        // Redirect the user to their respective details page
                        return RedirectToAction("Details", model.UserType + "s", new { id = person.PersonID });
                    }
                }

                // If username or password didn't match, notify the user
                ModelState.AddModelError("", "Invalid login attempt.");
            }

            // In case of any failures, redisplay the login form
            return View(model);
        }


        /// <summary>
        /// Handles the user logout.
        /// </summary>
        /// <returns>Redirects to the home page after logout.</returns>
        public IActionResult Logout()
        {
            // Retrieve the ID of the person currently in session
            int? personId = HttpContext.Session.GetInt32("PersonID");

            // If a valid person ID exists in session
            if (personId.HasValue)
            {
                // Retrieve the attendance record for today's date for this person
                var attendanceRecord = _context.GymAttendances
                                               .Where(g => g.MemberID == personId.Value && g.Date == DateTime.Today)
                                               .FirstOrDefault();

                // If an attendance record is found, update the checkout time
                if (attendanceRecord != null)
                {
                    attendanceRecord.CheckOut = DateTime.Now;
                    _context.SaveChanges(); // Persist the changes to the database
                }
            }

            // Clear all session data as part of the logout process
            HttpContext.Session.Clear();

            // Redirect the user to the home page after logging out
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Updates the gym attendance checkout time.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        private void UpdateGymAttendanceCheckOut(int memberId)
        {
            // Retrieve the attendance record for today's date for the given member
            var attendanceRecord = _context.GymAttendances
                                           .Where(g => g.MemberID == memberId && g.Date == DateTime.Today)
                                           .FirstOrDefault();

            // If an attendance record is found and the checkout time hasn't been set
            if (attendanceRecord != null && attendanceRecord.CheckOut == DateTime.MinValue)
            {
                // Set the checkout time to one hour after check-in time
                attendanceRecord.CheckOut = attendanceRecord.CheckIn.AddHours(1);
                _context.SaveChanges(); // Persist the changes to the database
            }
        }

    }
}
