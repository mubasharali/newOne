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

function TreeViewModel() {
    var self = this;

    self.showAds = ko.observableArray();
    self.brand = ko.observable("");
    self.model = ko.observable("");

    self.showCompanies = ko.observableArray();
    self.loadTree = function () {
        $.ajax({
            url: '/api/Electronic/GetLaptopTree',
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'POST',
            success: function (data) {
                var dat = $.map(data, function (item) { return new Company(item) });
                self.showCompanies(dat);
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
                }).on('changed.jstree', function (e, data) {
                    self.brand(data.instance.get_node(data.node.parent).text);
                    self.model(data.instance.get_node(data.selected[0]).text);
                    console.log(self.brand());
                    console.log(self.model());
                    if (self.brand() == undefined) {
                        self.brand(self.model());
                        self.model("");
                    }
                    console.log(self.brand());
                    console.log(self.model());
                    self.loadad();
                })
  // create the instance
  .jstree();
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
        });
    }
    self.loadTree();

    self.loadad = function () {
        url_address = '/api/Electronic/SearchLaptopAds?brand=' + self.brand() + '&model=' + self.model();
        $.ajax({
            url: url_address,
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'POST',
            success: function (data) {
                var mappedads = $.map(data, function (item) { return new ad(item); });
                self.showAds(mappedads);

            },
            error: function () {
                toastr.error("Unable to load data. Please try again", "Error");
            }
        });
    }
    self.loadad();
}
