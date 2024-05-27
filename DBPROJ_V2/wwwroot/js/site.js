// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    // Mutually exclusive button behavior
    $(".mutuallyExclusive").click(function () {
        $(".buttonL").not(this).removeClass("active"); // Deactivate other buttons
        $(this).addClass("active"); // Activate clicked button
    });

    // Handle button clicks and form submission
    $(".buttonL").click(function () {
        var userType = $(this).val();
        localStorage.setItem("userType", userType);
        $("#hiddenUserType").val(userType);
        $("#loginForm").submit();
    });
});
