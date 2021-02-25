function TTLValidation(isEditing) {
    //Bool that will be returned when the validation is finished
    var noErrors = true;
    const dateToday = new Date(Date.now());

    //All the text fields are grabbed and put into a var to get validated
    var title = document.getElementById('txtContestTitle').value;
    var description = document.getElementById('txtContestText').value;
    var email = document.getElementById('txtEmail').value;
    var startDate = document.getElementById('dateContestStart').value;
    var endDate = document.getElementById('dateContestEnd').value;

    //Error html for textfields 
    var titleError = document.getElementById('txtContestTitle-error');
    var descriptionError = document.getElementById('txtContestText-error');
    var emailError = document.getElementById('txtEmail-error');
    var startDateError = document.getElementById('dateContestStart-error');
    var endDateError = document.getElementById('dateContestEnd-error');

    //declare regex patterns
    var emailRegex = /^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/;
    
    //Client-side Validation start
    //validation for the title
    if (!TTLRequired(title)) {
        titleError.innerHTML = "Please Include a Title";
        noErrors = false;
    }
    else {
        titleError.innerHTML = "";
    }

    //validation for the description
    if (!TTLRequired(description)) {
        descriptionError.innerHTML = "Please Include a Description";
        noErrors = false;
    }
    else if (description.length < 50) {
        descriptionError.innerHTML = "The body of the contest must include more information";
        noErrors = false;
    }
    else {
        descriptionError.innerHTML = "";
    }

    //validation for email
    if (!TTLRequired(email)) {
        emailError.innerHTML = "Please Include an Email";
        noErrors = false;
    }
    else if (!emailRegex.test(email)) {
        emailError.innerHTML = "Email is not in the right format. EX. JohnSmith@CAGIS.ca";
        noErrors = false;
    }
    else {
        emailError.innerHTML = ""
    }

    //validation for start date
    if (!TTLRequired(startDate)) {
        startDateError.innerHTML = "Please Include a Start Date for the Contest";
        noErrors = false;
    }
    else if (!isEditing) {
        if (Date.parse(startDate) < Date.parse(dateToday)) {
            startDateError.innerHTML = "The start date cannot be before the current date/time.";
            noErrors = false;
        }
        else {
            startDateError.innerHTML = "";
        }
    }
    else {
        startDateError.innerHTML = "";
    }

    //validation for end date
    if (!TTLRequired(endDate)) {
        endDateError.innerHTML = "Please Include an End Date for the Contest";
        noErrors = false;
    }
    else if (Date.parse(endDate) <= Date.parse(startDate)) {
        endDateError.innerHTML = "The end date cannot be before the start date.";
        noErrors = false;
    }
    else {
        endDateError.innerHTML = "";
    }

    return noErrors;
}

//check for empty input of required field
function TTLRequired(input) {
    if (input == null || input.trim() == "") {
        return false
    }
    return true;
}