// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-24-2023
// ***********************************************************************
// <copyright file="gymEmployeeFormValidation.js" company="KeepCalmGymApplication">
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
    var inputs = document.querySelectorAll('input.form-control');
    var allFieldsFilled = true;

    for (var i = 0; i < inputs.length; i++) {
        var field = inputs[i];
        var errorMessage = field.parentElement.querySelector('.text-danger');
        if (!field.value.trim()) {
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

/**
 * Attempts to submit the form data via AJAX to the 'createEmployeeUrl'.
 * On successful submission, redirects to 'indexUrl'.
 */
function submitEmployeeForm() {
    /// <summary>
    /// Submits the employee form.
    /// </summary>
    if (!areFieldsFilled()) {
        return;  // Exit if fields are not filled
    }

    var formData = {
        FirstName: $('input[name="FirstName"]').val(),
        LastName: $('input[name="LastName"]').val(),
        ContactDetails: $('input[name="ContactDetails"]').val(),
        Speciality: $('input[name="Speciality"]').val(),
        Certification: $('input[name="Certification"]').val(),
        Password: $('input[name="Password"]').val(),
        Username: $('input[name="Username"]').val()

    };

    console.log(formData);

    $.ajax({
        url: createEmployeeUrl,  // Assuming you will set this variable in your view like you did with the member form.
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
                alert('Error: ' + response.message); // Assuming you want to send an error message in the response.
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            /// <summary>
            /// </summary>
            /// <param name="jqXHR">The jq XHR.</param>
            /// <param name="textStatus">The text status.</param>
            /// <param name="errorThrown">The error thrown.</param>
            alert('An error occurred... Check the console for more information.');
            console.log(textStatus, errorThrown);
        }
    });
}

