// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-28-2023
// ***********************************************************************
// <copyright file="PaymentController.cs" company="KeepCalmGymApplication">
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
    /// Provides functionality related to payments.
    /// </summary>
    public class PaymentController : Controller
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentController" /> class.
        /// </summary>
        /// <param name="context">The application context.</param>
        public PaymentController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of payments with an amount of 0.00.
        /// </summary>
        /// <returns>The view displaying the payments.</returns>
        public async Task<IActionResult> Index()
        {
            var payments = _context.Payments
                .Include(p => p.Member)
                .Where(p => p.Amount == 0.00m)
                .ToList();
            return View(payments);
        }

        /// <summary>
        /// Checks if a payment exists with the given identifier.
        /// </summary>
        /// <param name="id">The payment identifier.</param>
        /// <returns><c>true</c> if the payment exists; otherwise, <c>false</c>.</returns>
        private bool PaymentExists(int id)
        {
          return (_context.Payments?.Any(e => e.PaymentID == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Confirms a payment and updates its amount based on the associated member's contract.
        /// </summary>
        /// <param name="id">The identifier of the payment to confirm.</param>
        /// <returns>A redirection to the payment index upon success.</returns>
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Member)
                .FirstOrDefaultAsync(p => p.PaymentID == id);

            if (payment == null)
            {
                return NotFound();
            }

            var mostRecentContract = _context.Contracts
                .Where(c => c.MemberId == payment.MemberID)
                .OrderByDescending(c => c.StartDate)
                .FirstOrDefault();

            if (mostRecentContract == null)
            {
                // Handle the case where there's no contract for the member
                return NotFound();
            }

            // Set the payment amount based on the MembershipType
            switch (mostRecentContract.MembershipType)
            {
                case "Gold":
                    payment.Amount = 400m;
                    break;
                case "Silver":
                    payment.Amount = 300m;
                    break;
                case "Platinum":
                    payment.Amount = 500m;
                    break;
                default:
                    // Handle unexpected membership types if needed
                    break;
            }

            _context.Update(payment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Confirms multiple payments and updates their amounts based on the associated members' contracts.
        /// </summary>
        /// <param name="paymentIds">The identifiers of the payments to confirm.</param>
        /// <returns>A success response upon successful update.</returns>
        [HttpPost]
        public async Task<IActionResult> ConfirmAllPayments([FromBody] List<int> paymentIds)
        {
            foreach (var id in paymentIds)
            {
                // you might want to check if the payment exists, but for simplicity
                var payment = await _context.Payments
                    .Include(p => p.Member)
                    .FirstOrDefaultAsync(p => p.PaymentID == id);

                if (payment == null) continue;  // Skip if not found

                var mostRecentContract = _context.Contracts
                    .Where(c => c.MemberId == payment.MemberID)
                    .OrderByDescending(c => c.StartDate)
                    .FirstOrDefault();

                if (mostRecentContract == null) continue;

                // Set the payment amount (similar to what you did in ConfirmPayment)
                switch (mostRecentContract.MembershipType)
                {
                    case "Gold":
                        payment.Amount = 400m;
                        break;
                    case "Silver":
                        payment.Amount = 300m;
                        break;
                    case "Platinum":
                        payment.Amount = 500m;
                        break;
                }
                _context.Update(payment);
            }

            await _context.SaveChangesAsync();
            return Ok();  // Send success response
        }


    }
}
