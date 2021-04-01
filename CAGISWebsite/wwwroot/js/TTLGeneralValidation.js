function TTLValidation(contentType, actionName) {
    //Bool that will be returned when the validation is finished
    var noErrors = true;

    //Avoid validation if action take was not Create or Edit
    if (actionName == "Add Category") {
        noErrors = TTLCategoryValidation(contentType, noErrors)
    }
    else if (actionName != "Remove Image") {
        //determine the type of validation
        if (contentType == "Blog") {
            noErrors = TTLBlogValidation(noErrors);
        }
        else if (contentType == "Activity") {
            noErrors = TTLActivityValidation(noErrors);
        }
        else if (contentType == "DYK") {
            noErrors = TTLDYKValidation(noErrors);
        }
    }

    return noErrors;
}

//validation for blog
function TTLBlogValidation(noErrors){

    //All the text fields are grabbed and put into a var to get validated
    var title = document.getElementById('txtBlogTitle').value;
    var description = document.getElementById('txtBlogText').value;

    //Error html for textfields 
    var titleError = document.getElementById('txtBlogTitle-error');
    var descriptionError = document.getElementById('txtBlogText-error');

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
        descriptionError.innerHTML = "The body of the blog must include more information";
        noErrors = false;
    }
    else {
        descriptionError.innerHTML = "";
    }

    return noErrors;
}

//validation for activity
function TTLActivityValidation(noErrors) {

    //All the text fields are grabbed and put into a var to get validated
    var title = document.getElementById('txtActivityTitle').value;
    var description = document.getElementById('txtActivityText').value;

    //Error html for textfields 
    var titleError = document.getElementById('txtActivityTitle-error');
    var descriptionError = document.getElementById('txtActivityText-error');

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
        descriptionError.innerHTML = "The body of the activity must include more information";
        noErrors = false;
    }
    else {
        descriptionError.innerHTML = "";
    }

    return noErrors;
}

//validation for Did You Know
function TTLDYKValidation(noErrors) {

    //All the text fields are grabbed and put into a var to get validated
    var title = document.getElementById('txtDYKTitle').value;
    var description = document.getElementById('txtDYKText').value;

    //Error html for textfields 
    var titleError = document.getElementById('txtDYKTitle-error');
    var descriptionError = document.getElementById('txtDYKText-error');

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
        descriptionError.innerHTML = "The body of a 'Did You Know' must include more information";
        noErrors = false;
    }
    else {
        descriptionError.innerHTML = "";
    }

    return noErrors;
}

//validation for categories
function TTLCategoryValidation(contentType, noErrors) {
    var idString = "txt" + contentType + "Category";
    var idStringError = idString + "-error";
    //variables for validation
    var category = document.getElementById(idString).value;
    var categoryError = document.getElementById(idString + '-error');
    //get values from dropdown
    var categories = document.getElementById('categoryDropdown');
    //Client-side Category Validation start
    if (!TTLRequired(category)) {
        categoryError.innerHTML = "Cannot Add A Blank Category";
        noErrors = false;
    }
    else {
        //iterate through all current categories to ensure new category isn't a duplicate
        var noDuplicates = true;
        for (var i = 0; i < categories.length; i++) {
            if (categories.options[i].innerHTML == category) {
                noDuplicates = false;
                break;
            }
        }
        if (noDuplicates) {
            categoryError.innerHTML = "";
        }
        else {
            categoryError.innerHTML = "Cannot Add A Duplicate Category";
            noErrors = false;
        }
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

