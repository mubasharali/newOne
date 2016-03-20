
var title = ko.observable($("#search").val());
var tags = ko.observable("");
var minPrice = ko.observable(0);
var maxPrice = ko.observable(5000000);
var bedroom = ko.observable();
var bathroom = ko.observable();
var minArea = ko.observable(0);
var maxArea = ko.observable(5000);
var floor = ko.observable();
var availablebedrooms = ["1", "2", "3", "4", "5"];
var subcategory = ko.observable();
var availablesubcategories = ["apartment", "house", "plot & land", "Shop", "Office", "PG & Flatmates", "other commerical places"]; //reference over Realestate/create page + HomeController/CheckCategory()
minPrice.subscribe(function () {
    RefreshSearch();
});
maxPrice.subscribe(function () {
    RefreshSearch();
});
minArea.subscribe(function () {
    RefreshSearch();
});
maxArea.subscribe(function () {
    RefreshSearch();
});
floor.subscribe(function () {
    RefreshSearch();
});
bathroom.subscribe(function () {
    RefreshSearch();
});
bedroom.subscribe(function () {
    RefreshSearch();
});
subcategory.subscribe(function () {
    console.log(subcategory());
    RefreshSearch();
});
tags.subscribe(function () {
    RefreshSearch();
})
function TreeViewModel() {
    var self = this;
    self.isLoading = ko.observable(false);
    searchingCity.subscribe(function () {
        self.isLoading(false);
        RefreshSearch();
    })
    searchingPP.subscribe(function () {
        self.isLoading(false);
        RefreshSearch();
    })
    self.showAds = ko.observableArray();
    
}

function RefreshSearch() {
    var self = this;
    if (self.isLoading()) {
        return;
    }
    self.isLoading(true);
    $.ajax({
        url: '/api/RealEstate/SearchRealEstateAds?tags=' + tags() + '&title=' + title() + '&minPrice=' + minPrice() + '&maxPrice=' + maxPrice() + '&city=' + searchingCity() + '&pp=' + searchingPP() + '&minArea=' + minArea() + '&maxArea=' + maxArea() + '&floor=' + floor() + '&bedroom=' + bedroom() + '&bathroom=' + bathroom() + '&subcategory=' + subcategory() ,
        dataType: "json",
        contentType: "application/json",
        cache: false,
        type: 'POST',
        success: function (data) {
            self.isLoading(false);
            var mappedads = $.map(data, function (item) { return new Ad(item); });
            $("#FirstLoading").css("display", "block");
            self.showAds(mappedads);
            $("#select-rooms").selectize();
            $("#select-subcategory").selectize();
        },
        error: function () {
            self.isLoading(false);
            toastr.error("failed to search. Please refresh page and try again", "Error!");
        }
    });
}
var saveResult = function (data) {
    minPrice(data.fromNumber);
    maxPrice(data.toNumber);
};
$("#ionrange_1").ionRangeSlider({
    min: 0,
    max: 5000000,
    type: 'double',
    prefix: "Rs ",
    maxPostfix: "+",
    prettify: false,
    hasGrid: true,
    from: minPrice,
    to: maxPrice,
    onFinish: saveResult
});
var saveResultsForArea = function (data) {
    minArea(data.fromNumber);
    maxArea(data.toNumber);
};
$("#ionrange_area").ionRangeSlider({
    min: 0,
    max: 5000,
    type: 'double',
    prefix: "ft ",
    maxPostfix: "+",
    prettify: false,
    hasGrid: true,
    from: minArea,
    to: maxArea,
    onFinish: saveResultsForArea
});
