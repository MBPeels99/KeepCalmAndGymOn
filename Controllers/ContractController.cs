// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-28-2023
// ***********************************************************************
// <copyright file="ContractController.cs" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using KeepCalmGymApplication.App_Data;
using KeepCalmGymApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KeepCalmGymApplication.Controllers
{
    /// <summary>
    /// Controller responsible for the management of contracts within the application.
    /// </summary>
    public class ContractController : Controller
    {
        /// <summary>
        /// The database context instance used for CRUD operations.
        /// </summary>
        private readonly AppDbContext _context;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<ContractController> _logger;
        /// <summary>
        /// The current person identifier
        /// </summary>
        private int? CurrentPersonId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractController" /> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="logger">The logger to be used for logging messages.</param>
        public ContractController(AppDbContext context, ILogger<ContractController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Renders the main contract view, displaying all contracts.
        /// </summary>
        /// <returns>The Index view with all contracts.</returns>
        public async Task<IActionResult> Index()
        {
            return _context.Contracts != null ?
                        View(await _context.Contracts.Include(c => c.Member).ToListAsync()) :
                        Problem("Entity set 'AppDbContext.Contracts'  is null.");
        }

        /// <summary>
        /// Displays details for a specific contract identified by its ContractId.
        /// </summary>
        /// <param name="id">The identifier of the contract.</param>
        /// <returns>The Details view for the given contract.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Contracts == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Member)
                .FirstOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        /// <summary>
        /// Renders a view for creating a new contract.
        /// </summary>
        /// <returns>The Create view for contracts.</returns>
        public IActionResult Create()
        {
            CurrentPersonId = HttpContext.Session.GetInt32("PersonID");
            if (!CurrentPersonId.HasValue)
            {
                return NotFound();
            }

            var member = _context.Members.Find(CurrentPersonId.Value);
            if (member == null)
            {
                return NotFound("Member not found.");
            }

            ViewBag.FirstName = member.FirstName;
            ViewBag.LastName = member.LastName;

            ViewBag.MembershipTypes = GetMembershipTypesFromJson();

            return View(new Contract());
        }


        /// <summary>
        /// Handles POST requests to create a new contract.
        /// </summary>
        /// <param name="contract">The contract details to create.</param>
        /// <returns>A JSON response indicating success or failure.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Contract contract)
        {
            CurrentPersonId = HttpContext.Session.GetInt32("PersonID");
            Console.WriteLine(CurrentPersonId);
            if (contract == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Contract data was not provided."
                });
            }


            if (CurrentPersonId.HasValue)
            {
                contract.MemberId = CurrentPersonId.Value;
            }
            else
            {
                // Handle error: Maybe return an error message or redirect the user.
                return Json(new
                {
                    success = false,
                    message = "PersonID not found in session."
                });
            }

            Console.WriteLine(contract.MemberId);
            Console.WriteLine(contract);

            if (ModelState.IsValid)
            {
                _context.Contracts.Add(contract);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Details", new { id = contract.ContractId })
                });
            }
            else
            {
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    foreach (var error in modelStateVal.Errors)
                    {
                        Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                    }
                }
                return Json(new
                {
                    success = false,
                    message = "There was an error while creating the contract. Please try again."
                });
            }
        }


        /// <summary>
        /// Renders the contract edit view for the given ContractId.
        /// </summary>
        /// <param name="id">The identifier of the contract.</param>
        /// <returns>The Edit view for the given contract.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Contracts == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }
            return View(contract);
        }

        /// <summary>
        /// Handles POST requests to edit the given contract.
        /// </summary>
        /// <param name="id">The identifier of the contract.</param>
        /// <param name="contract">The edited contract details.</param>
        /// <returns>A redirection to the Index view or displays the Edit view in case of errors.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [FromBody] Contract contract)
        {
            if (id != contract.ContractId || !ContractExists(id))
            {
                return NotFound();
            }

            contract.MemberId = CurrentPersonId.Value;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError($"Concurrency error while updating contract: {ex.Message}");
                    return BadRequest("Failed to update contract due to concurrency issues.");
                }
            }
            return View(contract);
        }

        /// <summary>
        /// Renders a confirmation view for deleting the given contract.
        /// </summary>
        /// <param name="id">The identifier of the contract to delete.</param>
        /// <returns>The Delete view for the given contract.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Contracts == null || !ContractExists(id.Value))
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Member)
                .FirstOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        /// <summary>
        /// Handles POST requests to confirm and perform the deletion of the given contract.
        /// </summary>
        /// <param name="id">The identifier of the contract to delete.</param>
        /// <returns>A redirection to the Index view.</returns>
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Determines if a contract exists in the database based on its ContractId.
        /// </summary>
        /// <param name="id">The identifier of the contract.</param>
        /// <returns>True if the contract exists, otherwise False.</returns>
        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.ContractId == id);
        }

        /// <summary>
        /// Retrieves membership types from a JSON file.
        /// </summary>
        /// <returns>A dictionary with membership types as key-value pairs.</returns>
        private Dictionary<string, string> GetMembershipTypesFromJson()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/data/contractDetails.json");
            var jsonData = System.IO.File.ReadAllText(path);
            var memberships = JsonConvert.DeserializeObject<dynamic>(jsonData).memberships;

            Dictionary<string, string> membershipTypes = new Dictionary<string, string>();
            foreach (var membership in memberships)
            {
                string key = membership.Name;
                string value = membership.Value.description + "\n" + string.Join("\n", membership.Value.details.ToObject<List<string>>());
                membershipTypes.Add(key, value);
            }

            return membershipTypes;
        }

        /// <summary>
        /// Checks the status of a contract for a given person.
        /// </summary>
        /// <param name="personId">The identifier of the person whose contract status is to be checked.</param>
        /// <returns>A JSON object describing the contract status.</returns>
        [HttpGet]
        public async Task<IActionResult> CheckContractStatus(int? personId)
        {
            if (!personId.HasValue) return NotFound("PersonID not provided.");

            var contract = await _context.Contracts
                .Where(c => c.MemberId == personId.Value)
                .OrderByDescending(c => c.EndDate)
                .FirstOrDefaultAsync();

            if (contract == null)
            {
                return Json(new { status = "NoContract" });
            }

            var currentDate = DateTime.Now;
            if (contract.EndDate > currentDate && contract.EndDate < currentDate.AddMonths(1))
            {
                return Json(new { status = "RenewSoon", endDate = contract.EndDate });
            }

            if (contract.EndDate <= currentDate)
            {
                return Json(new { status = "Expired", endDate = contract.EndDate });
            }

            return Json(new { status = "Active", endDate = contract.EndDate });
        }



    }
}
