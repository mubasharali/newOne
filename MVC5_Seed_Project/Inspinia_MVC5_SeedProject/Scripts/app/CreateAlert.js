function CreateAlert() {
    var self = this;
    self.tagsToFollow = ko.observable("");
    self.followSummary = ko.observable();
    self.followTagsBtn = function () {
        if (loginUserId1) {
            if (self.tagsToFollow() == "") {
                toastr.info("Please Enter the tags you want to follow");
                return;
            }
            $.ajax({
                url: '/api/Tag/FollowTags?tags=' + self.tagsToFollow() + '&isUnfollowAllowed=' + false,
                dataType: "json",
                contentType: "application/json",
                cache: false,
                type: 'POST',
                success: function (data) {
                    self.followSummary("You have successfully followed " + self.tagsToFollow());
                },
                error: function () {
                    toastr.error("Some error has occured. Please refresh page and try again", "Error!");
                }
            });
        } else {
            loginBtn();
        }
    }
    var getLoginUserId = function (loginUserId) {
        return loginUserId;
    }
    $('#select-tags-to-follow').selectize({
        valueField: 'name',
        labelField: 'name',
        searchField: 'name',
        options: [],
        maxItems: 20,
        render: {
            option: function (item, escape) {
                return '<div>' +
                    '<span class="title">' +
                        '<span class="name">' + escape(item.name) + '</span>' +

                    '</span>' +
                    '<span class="description">' + escape(item.info) + '</span>' +
                    '<ul class="meta">' +
                        '<li class="watchers"><span>' + escape(item.followers) + '</span> followers</li>' +
                        '<li class="forks"><span>' + escape(item.questions) + '</span> times used</li>' +
                    '</ul>' +
                '</div>';
            }
        },
        load: function (query, callback) {
            if (!query.length) return callback();
            $.ajax({
                url: '/api/Tag/SearchTags?s=' + encodeURIComponent(query),
                type: 'POST',
                error: function () {
                    callback();
                },
                success: function (res) {
                    callback(res.slice(0, 10));
                }
            });
        }
    });
}