
var title = ko.observable($("#search").val());
var category = ko.observable($("#category").val());
var subcategory = ko.observable($("#subcategory").val());

var availableCategories = ["Dog", "Bird", "Cat", "Fish", "Farm Animals", "others", "Pet Adoption", "Pet Accessories", "Pet Food", "Pet training", "Pet Clinics"];
var selectedCategory = ko.observable();

var tags = ko.observable("");
var minPrice = ko.observable(0);
var maxPrice = ko.observable(50000);
minPrice.subscribe(function () {
    RefreshSearch();
});
maxPrice.subscribe(function () {
    RefreshSearch();
});
tags.subscribe(function () {
    RefreshSearch();
})
selectedCategory.subscribe(function () {
    console.log(selectedCategory());
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
        if (searchingCity() != null && searchingCity() != "undefined") {
            self.isLoading(false);
            RefreshSearch();
        }
        
    })
    self.showAds = ko.observableArray();
    

}
function RefreshSearch() {
    var self = this;
    if (self.isLoading()) {
        return;
    }
    if (category() == "Animals") {
        subcategory(selectedCategory());
    }
    self.isLoading(true);
    $.ajax({
        url: '/api/Search/SearchAds?tags=' + tags() + '&title=' + title() + '&minPrice=' + minPrice() + '&maxPrice=' + maxPrice() + '&city=' + searchingCity() + '&pp=' + searchingPP() + '&category=' + category() + '&subcategory=' + subcategory() ,
        dataType: "json",
        contentType: "application/json",
        cache: false,
        type: 'POST',
        success: function (data) {
            self.isLoading(false);
            var mappedads = $.map(data, function (item) { return new Ad(item); });
            self.showAds(mappedads);
            $("#FirstLoading").css({ "display": "block" });
        },
        error: function () {
            self.isLoading(false);
            toastr.error("failed to search. Please refresh page and try again", "Error!");
            $("#FirstLoading").css({ "display": "block" });
        }
    });
}
var saveResult = function (data) {
    minPrice(data.fromNumber);
    maxPrice(data.toNumber);
};
$("#ionrange_1").ionRangeSlider({
    min: 0,
    max: 50000,
    type: 'double',
    prefix: "Rs",
    maxPostfix: "+",
    prettify: false,
    hasGrid: true,
    from: minPrice,
    to: maxPrice,
    onFinish: saveResult
});
