function Tag(data) {
    var self = this;
    data = data || {};
    self.id = data.id;
    self.name = data.name;
}
function Company(data) {
    var self = this;
    data = data || {};
    self.title = data.title;
    self.shortabout = data.shortabout;
    self.postedByName = data.postedByName;
    self.postedById = data.postedById;
    self.id = data.id;
    self.link = "/CompanyPage/" + self.id + '/' + convertToSlug(self.title);
    self.logoLink = '/Images/Company/default_logo.png';
    if (data.logoExtension) {
        self.logoLink = $.cookie("AWSURL") + $.cookie("AWSCompanyFolder") + self.id + "/logo" + data.logoExtension;
    }
    self.category = data.category;
    self.city = data.city;
    self.popularPlace = data.popularPlace;
    self.exectLocation = data.exectLocation;
    self.contactNo1 = data.contactNo1;
    self.contactNo2 = data.contactNo2;
    self.showTags = ko.observableArray();


    if (data.tags != null) {
        var tags = $.map(data.tags, function (item) { return new Tag(item); });
        self.showTags(tags);
    }
}