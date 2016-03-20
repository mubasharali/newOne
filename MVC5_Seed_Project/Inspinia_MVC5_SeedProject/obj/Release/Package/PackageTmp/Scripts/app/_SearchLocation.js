
function SearchingLocation() {
    var self = this;
    self._isSearchCityLoading = ko.observable(false);
    self._isSearchPPLoading = ko.observable(false);
    console.log($.cookie("searchCity"));
    console.log($.cookie("searchPP"));
    searchingCity = ko.observable();
    searchingPP = ko.observable();
    searchingCities = ko.observableArray();
    searchingPPs = ko.observableArray();
    self.searchLocationBtn = function () {
        $("#setLocation").modal('hide');
    }
    //searchingCity.subscribe(function () {
        
    //})
    self.loadCities = function () {
        self._isSearchCityLoading(true);
        $.ajax({
            url: '/api/Location/GetCities',
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'GET',
            success: function (data) {
                self._isSearchCityLoading(false);
                $.each((data), function (i, item) { searchingCities.push(item) });
                searchingCity($.cookie("searchCity"));
                $('#searching-city').selectize({
                    sortField: {
                        field: 'text',
                        direction: 'asc'
                    },
                });
            },
            error: function (jqXHR, status, thrownError) {
                self._isSearchCityLoading(false);
                toastr.error("failed to load Cities.Please refresh page and try again", "Error");
            }
        });
    }
    self.loadPopularPlaces = function () {
        self._isSearchPPLoading(true);
        searchingPPs.removeAll();
        $.ajax({
            url: '/api/Location/GetPopularPlaces?city=' + searchingCity(),
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'GET',
            success: function (data) {
                self._isSearchPPLoading(false);
                $.each((data), function (i, item) { searchingPPs.push(item) });
                searchingPP($.cookie("searchPP"));
                $('#searching-popularPlace').selectize({

                    sortField: {
                        field: 'text',
                        direction: 'asc'
                    },
                });

                //--------------------
               
                
               

                //-----------------
            },
            error: function (jqXHR, status, thrownError) {
                self._isSearchPPLoading(false);
                toastr.error("failed to load Famous Places.Please refresh page and try again", "Error");
            }
        });
    }
    self.loadCities();
    var sub = searchingCity.subscribe(function (value) {
        if (!searchingCity()) {
            searchingPP("");
        }
        $.cookie("searchCity", searchingCity(), { path: '/'});
        self.loadPopularPlaces();
    })
    searchingPP.subscribe(function (value) {
        $.cookie("searchPP", searchingPP(), { path: '/' });
    })
}