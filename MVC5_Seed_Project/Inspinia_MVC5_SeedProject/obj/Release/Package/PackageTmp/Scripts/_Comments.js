//function commentsFile() {
    //var myhub = $.connection.AdComments;
    function commentReply(data) {
        var self = this;
        data = data || {};
        self.description = ko.observable(data.description);
        self.postedByName = data.postedByName;
        self.postedById = data.postedById;
        self.postedByLink = '/User/Index/' + self.postedById;
        self.profileLink = $.cookie("AWSURL") + $.cookie("AWSUserFolder") + 'p' + self.postedById + data.imageExtension;
        if (!data.imageExtension) {
            self.profileLink = '/Images/Users/default.jpg';
        }
        self.time = getTimeAgo(data.time);
        self.id = data.id;
        self.isliked = ko.observable(data.isUp);
        self.isunliked = ko.observable(data.isDown);
        self.voteUpCount = ko.observable(data.voteUpCount || 0);
        self.voteDownCount = ko.observable(data.voteDownCount || 0);
        self.isEditing = ko.observable(false);
        self.checkEnter1Editing = function (id, d, e) {
            if (e.keyCode == 13) {
                self.updateCommentReply(id);
            }
        }
        self.editCommentReply = function () {
            self.isEditing(true);
        }
        self.updateCommentReply = function (d) {
            var com = {
                Id: self.id,
                description: self.description(),
                postedBy: self.postedById,
                time: new Date($.now()),
                commentId: d,
            };
            $.ajax({
                url: '/api/Comment/updateCommentReply',
                dataType: "json",
                contentType: "application/json",
                cache: false,
                type: 'POST',
                data: ko.toJSON(com),
                success: function (data) {
                    self.isEditing(false);
                },
                error: function () {
                    toastr.error("failed to update comment", "Error!");
                }
            });
            //self.hub.server.UpdateComment(msg).fail(function (err) { toastr.error("failed to update comment", "Error!"); });
        }
        function CommentReplyVote(data) {
            voteUpC = data.voteUpCount;
            voteDownC = data.voteDownCount;
            self.voteUpCount(voteUpC);
            self.voteDownCount(voteDownC);
        }
        self.likeCommentReply = function (id, loginUserId, isup) {
            if (loginUserId) {
                $.ajax({
                    url: '/api/Comment/likeCommentReply?id=' + id + '&isup=' + isup,
                    dataType: "json",
                    contentType: "application/json",
                    cache: false,
                    type: 'POST',
                    success: function (data) {
                        var ret = $.map(data, function (item) { return new CommentReplyVote(item) });
                        if (isup) {
                            self.isliked(true);
                            self.isunliked(false);
                        } else {
                            self.isliked(false);
                            self.isunliked(true);
                        }
                    },
                    error: function (xhr, status, error) {
                        var err = eval("(" + xhr.responseText + ")");
                        toastr.info(err.Message);
                    }
                });
            }
            else {
                loginBtn();
            }
        }
    }
    function comment(data,hub) {
        var self = this;
        self.hub = hub;
        data = data || {};
        self.loginUserId = data.islogin || "";
        self.description = ko.observable(data.description);
        self.voteUpCount = ko.observable();
        self.voteDownCount = ko.observable();
        self.postedByName = data.postedByName;
        self.postedById = data.postedById;
        self.postedByLink = '/User/Index/' + self.postedById;
        self.profileLink = $.cookie("AWSURL") + $.cookie("AWSUserFolder") + 'p' + self.postedById + data.imageExtension;
        if (!data.imageExtension) {
            self.profileLink = '/Images/Users/default.jpg';
        }
        if (self.loginUserId == "" || data.loginUserProfileExtension == null) {
            self.loginUserProfileLink = '/Images/Users/default.jpg';
        } else {
            self.loginUserProfileLink = '/Images/Users/p' + self.loginUserId + data.loginUserProfileExtension;
        }

        self.time = getTimeAgo(data.time);
        self.adId = data.adId;
        self.id = data.id;

        self.isliked = ko.observable(data.isUp);
        self.isunliked = ko.observable(data.isDown);
        self.voteUpCount = ko.observable(data.voteUpCount || 0);
        self.voteDownCount = ko.observable(data.voteDownCount || 0);

        self.isFocus = ko.observable(true);
        self.isEditing = ko.observable(false);
        self.isVisible = ko.observable(false);
        self.showCommentReply = ko.observableArray();
        if (data.commentReply) {
            var reply = $.map(data.commentReply, function (item) { return new commentReply(item); });
            self.showCommentReply(reply);
        }
        self.toggleComment = function (item, event) {
            self.isVisible(!self.isVisible());
        }

        self.checkEnter1 = function (d, e) {
            if (self.loginUserId) {
                if (e.keyCode == 13) {
                    self.addCommentReply(self.loginUserId);
                }
            } else {
                loginBtn();
            }
        }
        self.checkEnterEditing = function (d, e) {
            if (e.keyCode == 13) {
                self.updateComment();
            }
        }
        self.editComment = function () {
            //self.description(self.description().slice(0, -1));
            self.isEditing(true);
        }
        self.updateComment = function () {
            var com = {
                Id: self.id,
                description: self.description(),
                postedBy: self.postedById,
                time: new Date($.now()),
                adId: self.adId,
            };
            $.ajax({
                url: '/api/Comment/updateComment',
                dataType: "json",
                contentType: "application/json",
                cache: false,
                type: 'POST',
                data: ko.toJSON(com),
                success: function (data) {
                    self.isEditing(false);
                },
                error: function () {
                    toastr.error("failed to update comment", "Error!");
                }
            });
        }
        self.deleteCommentReply = function (comment) {
            $.ajax({
                url: '/api/Comment/DeleteCommentReply/' + comment.id,
                dataType: "json",
                contentType: "application/json",
                cache: false,
                type: 'POST',
                success: function (data) {
                    self.showCommentReply.remove(comment);
                },
                error: function () {
                    toastr.error("failed to delete reply", "Error!");
                }
            });
        }
        function CommentVote(data) {
            voteUpC = data.voteUpCount;
            voteDownC = data.voteDownCount;
            self.voteUpCount(voteUpC);
            self.voteDownCount(voteDownC);
        }
        self.likeComment = function (id, loginUserId, isup) {
            if (loginUserId) {
                $.ajax({
                    url: '/api/Comment/likeComment?id=' + id + '&isup=' + isup,
                    dataType: "json",
                    contentType: "application/json",
                    cache: false,
                    type: 'POST',
                    success: function (data) {
                        var ret = $.map(data, function (item) { return new CommentVote(item) });
                        if (isup) {
                            self.isliked(true);
                            self.isunliked(false);
                        } else {
                            self.isliked(false);
                            self.isunliked(true);
                        }
                    },
                    error: function (xhr, status, error) {
                        var err = eval("(" + xhr.responseText + ")");
                        toastr.info(err.Message);
                    }
                });
            }
            else {
                loginBtn();
            }
        }
        self.newCommentReply = ko.observable();
        self.addCommentReply = function (islog) { //change
            if (islog) {
                var reply = new commentReply();
                reply.commentId = self.id;
                reply.time = new Date($.now());
                reply.description(self.newCommentReply());
                if (reply.description() != null && reply.description().trim() != "") {
                    //myhub.server.AddCommentReply(reply).fail(function (err) { toastr.error("failed to post comment reply", "Error!"); });

                    $.ajax({
                        url: '/api/Comment/PostCommentReply',
                        dataType: "json",
                        contentType: "application/json",
                        cache: false,
                        type: 'POST',
                        data: ko.toJSON(reply),
                        success: function (data) {
                            self.showCommentReply.push(new commentReply(data));
                            self.newCommentReply('');
                        },
                        error: function () {
                            toastr.error("failed to post comment reply", "Error!");
                        }
                    });
                }
            } else {
                loginBtn();
            }
        }
        //myhub.client.appendCommentReplyToMe = function (reply) {
        //    self.showCommentReply.push(new commentReply(reply));
        //    self.newCommentReply('');
        //}
        //myhub.client.appendCommentReply = function (reply) {
        //    self.showCommentReply.push(new commentReply(reply));
        //}
    }

