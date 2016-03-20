
function Model(data) {
    var self = this;
    data = data || {};
    self.modelName = data.model;
}
function Company(data) {
    var self = this;
    data = data || {};
    self.companyName = data.companyName;
    self.showModels = ko.observableArray();
    if (data.models) {
        var models = $.map(data.models, function (item) { return new Model(item) });
        self.showModels(models);
    }
}
function treeModel() {
    var self = this;
    self.showCompanies = ko.observableArray();
    self.showLaptopBrands = ko.observableArray();
    self.showCarBrands = ko.observableArray();
    self.loadTree = function () {
        $.ajax({
            url: '/api/Electronic/GetMobileTree',
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'POST',
            success: function (data) {
                var dat = $.map(data, function (item) { return new Company(item) });
                self.showCompanies(dat);
                $.ajax({
                    url: '/api/Electronic/GetLaptopTree',
                    dataType: "json",
                    contentType: "application/json",
                    cache: false,
                    type: 'POST',
                    success: function (data) {
                        var dat = $.map(data, function (item) { return new Company(item) });
                        self.showLaptopBrands(dat);
                        $.ajax({
                            url: '/api/Vehicle/GetCarTree',
                            dataType: "json",
                            contentType: "application/json",
                            cache: false,
                            type: 'POST',
                            success: function (data) {
                                var dat = $.map(data, function (item) { return new Company(item) });
                                self.showCarBrands(dat);

                                $("#navigation").jstree({
                                    "themes": {
                                        "theme": "classic"
                                    },
                                    "core": {
                                        "themes": {
                                            "icons": false
                                        }
                                    },
                                    "plugins": ["search"]
                                });
                                var to = false;
                                $('#treeSearch').keyup(function () {
                                    if (to) { clearTimeout(to); }
                                    to = setTimeout(function () {
                                        var v = $('#treeSearch').val();
                                        $('#navigation').jstree(true).search(v);
                                    }, 250);
                                });
                            },
                            error: function (jqXHR, status, thrownError) {
                                toastr.error("failed to laod category tree. Please refresh page", "Error");
                            }
                        })
                    },
                    error: function (jqXHR, status, thrownError) {
                        toastr.error("failed to laod category tree. Please refresh page", "Error");
                    }
                });
            },
            error: function (jqXHR, status, thrownError) {
                toastr.error("failed to laod category tree. Please refresh page", "Error");
            }
        });
       
    }
    self.loadTree();
}