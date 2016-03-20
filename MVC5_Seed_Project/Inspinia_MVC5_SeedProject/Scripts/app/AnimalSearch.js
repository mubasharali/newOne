
var title = ko.observable($("#search").val());
var tags = ko.observable("");
var minPrice = ko.observable(0);
var maxPrice = ko.observable(50000);
var availableCategories = ["Dog", "Bird", "Cat", "Fish","Farm Animals","others","Pet Adoption","Pet Accessories","Pet Food","Pet training","Pet Clinics"];
var selectedCategory = ko.observable();
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
        url: '/api/Search/SearchAds?tags=' + tags() + '&title=' + title() + '&minPrice=' + minPrice() + '&maxPrice=' + maxPrice() + '&city=' + searchingCity() + '&pp=' + searchingPP() + '&category=Animals&subcategory=' + selectedCategory(),
        dataType: "json",
        contentType: "application/json",
        cache: false,
        type: 'POST',
        success: function (data) {
            self.isLoading(false);
            var mappedads = $.map(data, function (item) { return new Ad(item); });
            $("#FirstLoading").css("display", "block");
            self.showAds(mappedads);
            $('#select-brand').selectize();
            $('#select-category').selectize();
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
