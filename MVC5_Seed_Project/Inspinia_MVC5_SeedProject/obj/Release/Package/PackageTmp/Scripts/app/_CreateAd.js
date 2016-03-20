function UserCompanies_CreateAd(self ) {

    self.postAdUsing = ko.observable();
    self.showCompanies = ko.observableArray();
    self.loadCompanies = function () {
        $.ajax({
            url: '/api/Company/GetCompaniesOfLoginUser',
            dataType: "json",
            contentType: "application/json",
            cache: false,
            // type: 'POST',
            success: function (data) {
                $.each((data), function (i, item) { self.showCompanies.push(item) });
            },
            error: function (jqXHR, status, thrownError) {
                toastr.error("failed to load data.Please refresh page and try again", "Error");
            }
        });
    }
    self.loadCompanies();
    
}