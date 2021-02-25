function TTLValidation() {
    //Bool that will be returned when the validation is finished
    var noErrors = true;

    //All the text fields are grabbed and put into a var to get validated
    var username = document.getElementById('txtUserName').value;
    var email = document.getElementById('txtEmail').value;
    var password = document.getElementById('txtPassword').value;
    var confirmPassword = document.getElementById('txtConfirmPassword').value;

    //Error html for textfields 
    var usernameError = document.getElementById('txtUserName-error');
    var emailError = document.getElementById('txtEmail-error');
    var passwordError = document.getElementById('txtPassword-error');
    var confirmPasswordError = document.getElementById('txtConfirmPassword-error');

    //declare regex patterns
    var emailRegex = /^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/;
    var passwordRegex = /^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9]).{6,}$/;
    var userNameRegex = /^[a-zA-Z0-9]*$/;

    //Client-side Validation start
    //validation for the username
    if (!TTLRequired(username)) {
        usernameError.innerHTML = "User Name cannot be empty";
        noErrors = false;
    }
    else if (!userNameRegex.test(username)) {
        usernameError.innerHTML = "User Name cannot have spaces or special characters";
        noErrors = false;
    }
    else {
        usernameError.innerHTML = "";
    }

    //validation for email
    if (!TTLRequired(email)) {
        emailError.innerHTML = "Email cannot be empty";
        noErrors = false;
    }
    else if (!emailRegex.test(email)) {
        emailError.innerHTML = "Email is not in the right format. EX. JohnSmith@CAGIS.ca";
        noErrors = false;
    }
    else {
        emailError.innerHTML = ""
    }

    //validation for password
    if (!TTLRequired(password)) {
        passwordError.innerHTML = "Password is required.";
        noErrors = false;
    }
    else if (password.length < 6) {
        passwordError.innerHTML = "Password must be at least 6 characters long.";
        noErrors = false;
    }
    else if (!passwordRegex.test(password)) {
        passwordError.innerHTML = "Password must have at least one capital letter, one number, and one symbol(!@#$&*).";
        noErrors = false;
    }
    else {
        passwordError.innerHTML = "";
    }

    //validation for confirmation password
    if (!TTLRequired(confirmPassword)) {
        confirmPasswordError.innerHTML = "Please Confirm the Password";
        noErrors = false;
    }
    else {
        confirmPasswordError.innerHTML = "";
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