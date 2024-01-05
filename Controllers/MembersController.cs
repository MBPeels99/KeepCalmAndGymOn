// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-28-2023
// ***********************************************************************
// <copyright file="MembersController.cs" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using KeepCalmGymApplication.App_Data;
using KeepCalmGymApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace KeepCalmGymApplication.Controllers
{
    /// <summary>
    /// Manages CRUD operations for members.
    /// </summary>
    public class MembersController : Controller
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<MembersController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MembersController" /> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="logger">The logger.</param>
        public MembersController(AppDbContext context, ILogger<MembersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Displays a list of members.
        /// </summary>
        /// <returns>A view that displays the members.</returns>
        public async Task<IActionResult> Index()
        {
            return _context.Members != null ?
                        View(await _context.Members.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.Members'  is null.");
        }

        /// <summary>
        /// Displays details for a specific member.
        /// </summary>
        /// <param name="id">The member identifier.</param>
        /// <returns>A view that displays the member details.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.PersonID == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        /// <summary>
        /// Displays a view to create a new member.
        /// </summary>
        /// <returns>A view to create a member.</returns>
        public IActionResult Create()
        {
            Console.WriteLine("Please work");
            return View();
        }

        /// <summary>
        /// Creates a new member.
        /// </summary>
        /// <param name="member">The member to create.</param>
        /// <returns>The result of the creation process.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Member member)
        {
            if (ModelState.IsValid)
            {
                // Use BCrypt to hash the password
                member.Password = BCrypt.Net.BCrypt.HashPassword(member.Password);

                try
                {
                    _context.Add(member);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Created the member. ModelState is valid.");
                    // Store the user details in session
                    HttpContext.Session.SetInt32("PersonID", member.PersonID);
                    HttpContext.Session.SetString("UserName", member.Username);
                    HttpContext.Session.SetString("UserType", "Member");
                    return Json(new { success = true, redirectUrl = Url.Action("Details", new { id = member.PersonID }) });
                }
                catch (Exception ex)
                {
                    return Content($"Error: {ex.Message}");
                }
            }
            else
            {
                var errorList = ModelState.SelectMany(m => m.Value.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();
                Console.WriteLine($"Failed to create the member. ModelState errors: {string.Join(", ", errorList)}");
                return BadRequest(errorList); // Send the errors back as a response
            }
        }


        /// <summary>
        /// Displays a view to edit a member.
        /// </summary>
        /// <param name="id">The member identifier.</param>
        /// <returns>A view to edit a member.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        /// <summary>
        /// Edits an existing member.
        /// </summary>
        /// <param name="id">The member identifier.</param>
        /// <param name="memberInput">The member data to update.</param>
        /// <returns>The result of the edit process.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [FromBody] MemberEditViewModel memberInput)
        {
            _logger.LogInformation($"Starting the Edit process for member with ID: {id}");

            var originalMember = await _context.Members.FindAsync(id);
            if (originalMember == null)
            {
                Console.WriteLine($"Member with ID {id} not found in database.");
                return NotFound();
            }

            if (originalMember.Username != memberInput.Username)
            {
                bool isUsernameTaken = await _context.Members.AnyAsync(m => m.Username == memberInput.Username);
                Console.WriteLine($"Checking if username '{memberInput.Username}' is taken: {isUsernameTaken}");

                if (isUsernameTaken)
                {
                    ModelState.AddModelError("Username", "Username is already taken.");
                }
            }

            if (!string.IsNullOrEmpty(memberInput.Password))
            {
                bool isOldPasswordCorrect = VerifyPassword(memberInput.Password, originalMember.Password);
                Console.WriteLine($"Verification of old password: {isOldPasswordCorrect}");

                if (!isOldPasswordCorrect)
                {
                    ModelState.AddModelError("OldPassword", "Old password is incorrect.");
                }
                else if (!string.IsNullOrEmpty(memberInput.NewPassword))
                {
                    originalMember.Password = HashPassword(memberInput.NewPassword);
                    Console.WriteLine("Hashed and updated the password.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    originalMember.FirstName = memberInput.FirstName;
                    originalMember.LastName = memberInput.LastName;
                    originalMember.ContactDetails = memberInput.ContactDetails;
                    // Note: We're not changing DateOfBirth here as per your requirements.

                    _context.Update(originalMember);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Successfully updated member with ID: {id}");
                    return Json(new { success = true, redirectUrl = Url.Action("Details", new { id = id }) });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine($"Concurrency Exception while updating member with ID {id}: {ex.Message}");
                    if (!MemberExists(memberInput.PersonID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            Console.WriteLine($"Validation errors while updating member: {string.Join(", ", errors)}");
            return Json(new { success = false, errorMessage = string.Join(", ", errors) });
        }



        /// <summary>
        /// Displays a view to confirm the deletion of a member.
        /// </summary>
        /// <param name="id">The member identifier.</param>
        /// <returns>A view to confirm deletion.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            _logger.LogInformation($"Attempting to retrieve member for deletion with ID: {id}");

            if (id == null || _context.Members == null)
            {
                _logger.LogWarning("Delete operation was halted due to null id or null context.Members");
                return NotFound();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.PersonID == id);
            if (member == null)
            {
                _logger.LogWarning($"No member found with ID: {id} for deletion");
                return NotFound();
            }

            _logger.LogInformation($"Member with ID: {id} retrieved for deletion");
            return View(member);
        }

        /// <summary>
        /// Deletes a member.
        /// </summary>
        /// <param name="PersonID">The member identifier.</param>
        /// <returns>The result of the delete operation.</returns>
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed([FromForm] int PersonID)
        {
            _logger.LogInformation($"Attempting to confirm deletion for member with ID: {PersonID}");

            var member = await _context.Members.FindAsync(PersonID);
            if (member != null)
            {
                _logger.LogInformation($"Member with ID: {PersonID} found. Starting deletion process...");

                _context.Members.Remove(member);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully deleted member with ID: {PersonID}");
            }
            else
            {
                _logger.LogWarning($"Failed to delete member. No member found with ID: {PersonID}");
            }

            //return RedirectToAction(nameof(Index));
            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Checks if a member exists.
        /// </summary>
        /// <param name="id">The member identifier.</param>
        /// <returns><c>true</c> if the member exists; otherwise, <c>false</c>.</returns>
        private bool MemberExists(int id)
        {
            return (_context.Members?.Any(e => e.PersonID == id)).GetValueOrDefault();
        }

        /// <summary>
        /// GET method to check if a user is currently logged in.
        /// </summary>
        /// <returns>A JSON response indicating the user's logged-in status and their details if logged in.</returns>
        [HttpGet]
        public IActionResult IsLoggedIn()
        {
            var username = HttpContext.Session.GetString("UserName");
            var personId = HttpContext.Session.GetInt32("PersonID");

            if (!string.IsNullOrEmpty(username) && personId.HasValue)
            {
                return Ok(new { status = "Logged in", userName = username, personId = personId.Value });
            }
            return Ok(new { status = "Logged out" });
        }

        /// <summary>
        /// Checks if the provided username is already taken by another user in the database.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns>A JSON response indicating whether the username is taken.</returns>
        [HttpGet]
        public async Task<IActionResult> IsUsernameTaken(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username is required.");
            }

            var memberWithUsername = await _context.Members.FirstOrDefaultAsync(m => m.Username == username);
            return Ok(new { isTaken = memberWithUsername != null });
        }

        /// <summary>
        /// Hashes the provided password using BCrypt.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hashed version of the provided password.</returns>
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Checks if the specified password matches the hashed password.
        /// </summary>
        /// <param name="inputPassword">The password to verify.</param>
        /// <param name="hashedPassword">The hashed version of the password to compare against.</param>
        /// <returns>True if the passwords match, otherwise false.</returns>
        private bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
        }

        /// <summary>
        /// Retrieves the gym classes scheduled for the specified user.
        /// </summary>
        /// <param name="personId">The ID of the user/member.</param>
        /// <returns>A JSON representation of the scheduled gym classes.</returns>
        [HttpGet]
        public IActionResult GetClassesForUserCalendar(int personId)
        {
            var gymClasses = _context.GymClassAttendances
                                    .Include(a => a.GymClass)
                                    .Where(a => a.MemberID == personId)
                                    .Select(a => a.GymClass)
                                    .ToList();

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
        /// Checks the payment eligibility of a member.
        /// </summary>
        /// <param name="personId">The ID of the member to check.</param>
        /// <returns>A JSON representation of the member's payment status and eligibility.</returns>
        [HttpGet]
        public async Task<IActionResult> CheckPaymentEligibility(int personId)
        {
            // Check if user has a contract
            var hasContract = await _context.Contracts.AnyAsync(c => c.MemberId == personId);
            if (!hasContract)
            {
                return Json(new { status = "NoContract" });
            }

            // Retrieve the last payment
            var lastPayment = await _context.Payments
                .Where(p => p.MemberID == personId)
                .OrderByDescending(p => p.Date)
                .FirstOrDefaultAsync();

            if (lastPayment == null)
            {
                return Json(new { status = "NoPayment" });
            }

            // Check if the last payment date is more than a month ago
            if (lastPayment.Date < DateTime.Now.AddMonths(-1))
            {
                return Json(new { status = "Eligible" });
            }

            return Json(new { status = "PaymentExists", paymentId = lastPayment.PaymentID, paymentDate = lastPayment.Date, amount = lastPayment.Amount });

        }


        /// <summary>
        /// Records a payment for a member in the database.
        /// </summary>
        /// <param name="payment">The payment data to be recorded.</param>
        /// <returns>A JSON response indicating the success or failure of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> StorePayment(Payment payment)
        {
            if (payment == null)
            {
                return Json(new { success = false, message = "Invalid payment data" });
            }

            try
            {
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // return the PaymentID
                return Json(new { success = true, message = "Payment recorded successfully", paymentId = payment.PaymentID });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error storing payment: ", ex);
                return Json(new { success = false, message = "Error storing payment" });
            }
        }


        /// <summary>
        /// Retrieves payment details and associated membership type for a specified member.
        /// </summary>
        /// <param name="memberId">The ID of the member to retrieve details for.</param>
        /// <returns>A JSON response containing the membership type or an error message if not found.</returns>
        [HttpGet]
        public async Task<IActionResult> PaymentDetails(int memberId)
        {
            var contract = await _context.Contracts.SingleOrDefaultAsync(c => c.MemberId == memberId);
            if (contract == null)
            {
                return NotFound(new { success = false, message = "No contract found for this member" });
            }

            var membershipType = contract.MembershipType;
            return Json(new { success = true, membershipType = membershipType });
        }


    }
}
