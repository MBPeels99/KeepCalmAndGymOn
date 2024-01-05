// ***********************************************************************
// Assembly         : KeepCalmGymApplication
// Author           : Gebruiker
// Created          : 08-24-2023
//
// Last Modified By : Gebruiker
// Last Modified On : 08-24-2023
// ***********************************************************************
// <copyright file="memberFormValidation.js" company="KeepCalmGymApplication">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
function isUsernameTaken(username) {
    /// <summary>
    /// Determines whether [is username taken] [the specified username].
    /// </summary>
    /// <param name="username">The username.</param>
    /// <returns><c>true</c> if [is username taken] [the specified username]; otherwise, <c>false</c>.</returns>
    return $.ajax({
        url: '/Members/IsUsernameTaken',
        type: 'GET',
        data: { username: username }
    });
}

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

        // Special handling for DateOfBirth field.
        if (field.name === "DateOfBirth") {
            if (!isValidDateField(field)) {
                allFieldsFilled = false;
            }
            continue;
        }

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
 * Validates if the given date input field has a valid date.
 * @param {HTMLInputElement} field - The date input field to validate.
 * @returns {boolean} Returns true if date is valid, otherwise returns false.
 */
function isValidDateField(field) {
    // This regex checks for a YYYY-MM-DD format.
    const regex = /^\d{4}-\d{2}-\d{2}$/;

    /// <var>The error message</var>
    var errorMessage = field.parentElement.querySelector('.text-danger');

    if (!field.value.trim()) {
        errorMessage.textContent = "This field is required.";
        field.classList.add('is-invalid');
        field.classList.remove('is-valid');
        return false;
    }

    if (!regex.test(field.value.trim())) {
        errorMessage.textContent = "Date is not in a valid format.";
        field.classList.add('is-invalid');
        field.classList.remove('is-valid');
        return false;
    }

    const date = new Date(field.value);
    if (date.toISOString().slice(0, 10) !== field.value) {
        errorMessage.textContent = "Date is not valid.";
        field.classList.add('is-invalid');
        field.classList.remove('is-valid');
        return false;
    }

    errorMessage.textContent = "";
    field.classList.remove('is-invalid');
    field.classList.add('is-valid');
    return true;
}

/**
 * Attempts to submit the form data via AJAX to the 'createMemberUrl'.
 * On successful submission, redirects to 'indexUrl'.
 */
function submitForm() {
    if (!areFieldsFilled()) {
        return;  // Exit if fields are not filled
    }

    var username = $('input[name="Username"]').val();

    isUsernameTaken(username).done(function (response) {
        /// <summary>
        /// </summary>
        /// <param name="response">The response.</param>
        if (response.isTaken) {
            alert('The username is already taken. Please choose another.');
            $('input[name="Username"]').addClass('is-invalid');
            $('span[asp-validation-for="Username"]').text('Username is already taken.');
            return;
        } else {

            var formData = {
                FirstName: $('input[name="FirstName"]').val(),
                LastName: $('input[name="LastName"]').val(),
                Username: $('input[name="Username"]').val(),
                DateOfBirth: $('input[name="DateOfBirth"]').val(),
                ContactDetails: $('input[name="ContactDetails"]').val(),
                Password: $('input[name="Password"]').val()
            };

            console.log(formData);

            $.ajax({
                url: createMemberUrl,
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
    });
}

/**
* Attempts to submit the form data via AJAX to the 'editMemberUrl'.
* On successful submission, redirects to 'indexUrl'. new
*/
function editFormSubmit(event) {
    event.preventDefault();

    /// <var>The form data</var>
    var formData = {
        FirstName: $('input[name="FirstName"]').val(),
        LastName: $('input[name="LastName"]').val(),
        Username: $('input[name="Username"]').val(),
        ContactDetails: $('input[name="ContactDetails"]').val(),
        Password: $('input[name="Password"]').val(),
        NewPassword: $('input[name="NewPassword"]').val()
    };

    console.log(formData);

    $.ajax({
        url: editMemberUrl,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        success: function (response) {
            /// <summary>
            /// </summary>
            /// <param name="response">The response.</param>
            console.log(response);
            if (response.success) {
                alert('Your profile has been updated!!!');
                window.location.href = response.redirectUrl;
            } else {
                alert('Error: ' + response.errorMessage);
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

function GenerateCalender(events) {
    $('#calender').fullCalendar('destroy');
    $('#calender').fullCalendar({
        contentHeight: 400,
        defaultDate: new Date(),
        timeFormat: 'h(:mm)a',
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,basicWeek,basicDay,agenda'
        },
        eventLimit: true,
        eventColor: '#378006',
        events: events,
        eventClick: function (event) {
            // When an event is clicked, redirect the user to the GymClass Details page
            /// <summary>
            /// </summary>
            /// <param name="event">The event.</param>
            window.location.href = '/GymClass/Details/' + event.id;
        }
    })
}

