function Tag(data) {
    var self = this;
    data = data || {};
    self.id = data.id;
    self.name = data.name;
    self.link = "/Tag/" + data.id  + "/" + convertToSlug(self.name);
}
function Bid(data) {
    var self = this;
    data = data || {};
    self.price = data.price;
}
var imageUrls = [];
function adImages(data, adId, imagesCount) {
    return $.cookie("AWSURL") + $.cookie("AWSFolderName") + adId + '_' + imagesCount + data.imageExtension;
}
function Location(data) {
    var self = this;
    data = data || {};
    self.cityId = data.cityId;
    self.cityName = data.cityName;
    self.popularPlaceId = data.popularPlaceId;
    self.popularPlace = data.popularPlace;
    self.exectLocation = data.exectLocation;
}
function convertToSlug(Text) {
    return Text
        .toLowerCase()
        .replace(/[^\w ]+/g, '')
        .replace(/ +/g, '-')
    ;
}
function Ad(data) {
    var self = this;

    self.images = ko.observable();
    self.options = {};
    self.imageIndex = ko.observable(1);

    data = data || {};
    self.title = data.title;
    
    self.description = ko.observable(data.description);
    self.postedByName = data.postedByName;
    self.postedById = data.postedById;
    self.id = data.id;
    self.time = getTimeAgo(data.time);
    self.loginUserId = data.islogin || "";
    self.showMobileAd = ko.observableArray();
    self.isReported = ko.observable(data.isReported);
    self.reportedCount = ko.observable(data.reportedCount);
    self.showImages = ko.observable();
    self.views = data.views;
    self.price = data.price || "";
    self.link = "/Details/" + data.id + "/" + convertToSlug( data.title);
    var loginUserId1 = getLoginUserId(self.loginUserId); //to access self.loginUserId outside this function
    //biding
    self.highestBid = ko.observable();
    self.showBidings = ko.observableArray();
    self.isBidingAllowed = false;
    self.placeBid = ko.observable(false);
    self.bidAmount = ko.observable();

    self.showTags = ko.observableArray();
    self.adLocation = ko.observable();
    self.sellerProfile = function () {
        window.location.href = "/User/Index/" + self.postedById;
    }
    if (data.location != null) {
        self.adLocation(new Location(data.location));
    }
    self.isNegotiable = "";
    if (data.isNegotiable == "b") {
        self.isBidingAllowed = true;
    } else if (data.isNegotiable == "y") {
        self.isNegotiable = 'Negotiable';
    } else if (data.isNegotiable == "n") {
        self.isNegotiable = '<strike>Negotiable</strike>';
    }
    if (self.price == "") {
        self.isNegotiable = "";
    }

    if (data.bid != null) {
        var biding = $.map(data.bid, function (item) { return new Bid(item); });
        self.highestBid(Math.max.apply(Math, data.bid.map(function (o) { return o.price; })));
        //self.showBidings(biding);
    }
    if (data.adTags != null) {
        var tags = $.map(data.adTags, function (item) { return new Tag(item); });
        self.showTags(tags);
    }
    if (data.adImages) {
        var imagesCount = 1;
        var img = $.map(data.adImages, function (item, ia) { return adImages(item, data.id, imagesCount++); });
        self.showImages(img);
        self.images(img);
    }
    self.report = function (id, loginUserId) {
        if (loginUserId) {
            $.ajax({
                url: '/api/Electronic/reportAd?id=' + id,
                dataType: "json",
                contentType: "application/json",
                cache: false,
                type: 'POST',
                success: function (data) {
                    self.isReported(true);
                    self.reportedCount(data);
                },
                error: function (xhr, status, error) {
                    var err = eval("(" + xhr.responseText + ")");
                    toastr.info(err.Message);
                }
            })
        } else {
            toastr.info("You must be login to report this ad", "Na na!");
        }
    }
}