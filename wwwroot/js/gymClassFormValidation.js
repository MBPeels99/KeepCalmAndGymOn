// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-24-2023
// ***********************************************************************
// <copyright file="gymClassFormValidation.js" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
/**
 * Validates if all input fields in the form are filled correctly.
 * @returns {boolean} Returns true if all fields are filled, otherwise returns false.
 */
function areFieldsFilled() {
    /// <summary>
    /// Ares the fields filled.
    /// </summary>
    var inputs = document.querySelectorAll('input.form-control, select.form-control');
    var allFieldsFilled = true;

    for (var i = 0; i < inputs.length; i++) {
        var field = inputs[i];
        var errorMessage = field.parentElement.querySelector('.text-danger');

        // Validation for Capacity: number and greater than 0
        if (field.name === 'Capacity' && (!field.value.trim() || isNaN(field.value) || parseInt(field.value) <= 0)) {
            errorMessage.textContent = "Capacity is required and must be a valid number greater than 0.";
            field.classList.add('is-invalid');
            field.classList.remove('is-valid');
            allFieldsFilled = false;
        }
        // Generic validation for other fields
        else if (!field.value.trim()) {
            errorMessage.textContent = "This field is required.";
            field.classList.add('is-invalid');
            field.classList.remove('is-valid');
            allFieldsFilled = false;
        } else {
            errorMessage.textContent = "";
            field.classList.remove('is-invalid');
            field.classList.add('is-valid');
        }
    }

    return allFieldsFilled;
}

document.addEventListener("DOMContentLoaded", function () {
    // Bind the function to the submit event of the form
    /// <summary>
    /// </summary>
    document.querySelector("form").addEventListener("submit", function (event) {
        /// <summary>
        /// </summary>
        /// <param name="event">The event.</param>
        if (!areFieldsFilled()) {
            event.preventDefault();
        }
    });
});

/**
 * Attempts to submit the Gym Class form data via AJAX.
 * On successful submission, redirects to a designated URL or handles errors.
 */
function submitGymClassForm() {
    /// <summary>
    /// Submits the gym class form.
    /// </summary>
    if (!areFieldsFilled()) {
        return;  // Exit if fields are not filled
    }

    var formData = {
        ClassName: $('input[name="ClassName"]').val(),
        InstructorID: parseInt($('select[name="InstructorID"]').val()),
        Date: $('input[name="Date"]').val(),
        Time: $('input[name="Time"]').val(),
        Capacity: parseInt($('input[name="Capacity"]').val()),
        Category: $('select[name="Category"]').val(),
    };

    console.log(formData);

    $.ajax({
        url: createGymClassUrl,  // You will need to set this variable similarly to 'createEmployeeUrl' for the Gym Class creation.
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        success: function (response) {
            /// <summary>
            /// </summary>
            /// <param name="response">The response.</param>
            if (response.success) {
                window.location.href = response.redirectUrl;  // Redirect to the provided URL on success
            } else {
                alert('Error: ' + response.message); // Display any error message from the response.
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            /// <summary>
            /// </summary>
            /// <param name="jqXHR">The jq XHR.</param>
            /// <param name="textStatus">The text status.</param>
            /// <param name="errorThrown">The error thrown.</param>
            alert('An error occurred... Check the console for more information.');
            console.log("Test error here");
            console.log(textStatus, errorThrown);
        }
    });
}
