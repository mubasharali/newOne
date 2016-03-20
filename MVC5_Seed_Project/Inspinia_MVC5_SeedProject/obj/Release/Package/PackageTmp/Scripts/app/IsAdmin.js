var isAdmin = ko.observable(false);
var loadAdmin = function () {
    $.ajax({
        url: '/api/Admin/isAdmin',
        dataType: "json",
        contentType: "application/json",
        cache: false,
        type: 'POST',
        success: function (data) {
            isAdmin(data);
        },
        error: function (jqXHR, status, thrownError) {
            toastr.error("failed to check admin.Please refresh page and try again", "Error");
        }
    });
}