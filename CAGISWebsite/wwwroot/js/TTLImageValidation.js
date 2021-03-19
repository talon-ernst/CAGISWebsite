function TTLCheckImageExtension() {
    //fields relevant to image upload
    var fileUpload = document.getElementById("imgImageUpload");
    var errorLabel = document.getElementById("imgImageUpload-error");

    var fileName = fileUpload.value;
    //check extension of file to ensure it's one of the default image types
    var ext = fileName.substr(fileName.lastIndexOf('.') + 1).toLowerCase();
    if (!(ext == "jpeg" || ext == "jpg" || ext == "png" || ext == "gif"))
    {
        errorLabel.innerHTML = "Invalid image file, must select a *.jpeg, *.jpg, *.gif, or *.png file.";
        //clear selection if not an image
        fileUpload.value = "";
        return;
    }
    //get image size
    const filesize = fileUpload.files[0].size;
    const limitSize = 1024 * 1024 * 5; //limit size = 5MB

    if (filesize > limitSize)
    {
        errorLabel.innerHTML = "File is too big, please upload image with a size less than 5MB.";
        //clear selection if image is too big
        fileUpload.value = "";
        return;
    }

    //check image size
    let img = new Image()
    img.src = window.URL.createObjectURL(fileUpload.files[0])
    img.onload = () => {
        if (img.width < 100 && img.height < 100)
        {
            errorLabel.innerHTML = "Image too small, please upload an image 100x100 or bigger.";
            //clear selection if image selection is too small
            fileUpload.value = "";
            return;
        }
    }
    //clear error if no errors
    errorLabel.innerHTML = "";
}