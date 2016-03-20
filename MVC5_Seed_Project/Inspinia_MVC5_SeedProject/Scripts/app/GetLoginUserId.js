var loginUserId = ko.observable( "Visitor");
$.ajax({
    url: '/api/User/GetLoginUserId',
    dataType: "json",
    contentType: "application/json",
    cache: false,
    type: 'GET',
    success: function (data) {
        loginUserId( data);
    },
    error: function () {
        toastr.error("failed to fetch data. Please refresh page and try again", "Error!");
    }
});
createAd = function (link) {
    if (loginUserId() != "Visitor") {
        window.location.href = link;
    } else {
        loginBtn();
    }
}