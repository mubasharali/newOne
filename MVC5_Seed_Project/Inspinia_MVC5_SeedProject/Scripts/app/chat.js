function OnlineUsers(data) {
    var self = this;
    data = data || {};
    self.id = data.id;
    self.name = data.name;
    self.dpLink = '/Images/Users/p' + self.id + data.dpExtension;
    if (!data.dpExtension) {
        self.dpLink = '/Images/Users/default.jpg';
    }
    //self.profileLink = '/Users/Index/' + self.id;
    self.openProfile = function () {
        window.location.href = '/User/Index/' + self.id;
    }
}
function Message(data,sendMessageTo) {
    var self = this;
    data = data || {};
    self.id = data.id;
    self.name = data.name;
    self.sentFrom = data.sentFrom;
    self.sentTo = data.sentTo;
    self.sentFromName = data.sentFromName;
    self.sentToName = data.sentToName;
    self.message = data.message;
    self.time = (data.time);
    self.timeAgo = getTimeAgo(data.time);
    self.loginUserId = data.loginUserId;
    console.log(self.sentFrom);
    console.log(self.sentTo);
    console.log("will be sent to : " + sendMessageTo);
    if (sendMessageTo == self.sentFrom || sendMessageTo == self.sentTo) {
        
    } else {
        console.log("No entry");
        self.message = "";
    }
}
function ChatViewModel() {
    
    var self = this;
    self.hub = $.connection.chatHub;
    self.onlineUsersHub = $.connection.onlineUsers;
    self.showChat = ko.observableArray();
    self.newMessage = ko.observable();
    self.loginUserId1 = "";
    self.onlineUsers = ko.observableArray();
    self.sendMessageTo = ko.observable();
    self.sendMessgeToName = ko.observable();
    self.sendTo = function (data) {
        if (self.loginUserId1) {
            self.sendMessageTo(data.id);
            self.sendMessgeToName(data.name);
        } else {
            loginBtn();
        }
    }
    self.isOnline = ko.observable("Offline");
    
    var sub = self.sendMessageTo.subscribe(function (value) {
        self.loadMessages();
    })
    self.checkUserOnline = function () {
        if (window.location.pathname.indexOf("/User/Index/") > -1 || window.location.pathname.indexOf("/User/Profile/") > -1) {
            var userId = $("#userId").val();
            self.isOnline("Offline");
            $.each(self.onlineUsers(), function (key, value) {
                if (value.id == userId) {
                    self.isOnline("Online");
                }
            })
        }
    }
    self.onlineUsers.subscribe(function (value) {
        self.checkUserOnline();
    })
    self.getLoginUserId = function () {
        $.ajax({
            url: '/api/Chat/GetLoginUserId',
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'GET',
            success: function (data) {
                self.loginUserId1 = data;
            },
            error: function () {
                toastr.error("failed to check user login", "Error!");
                return null;
            }
        });
    };
    self.getLoginUserId();
    self.checkUserOnline();
    self.loadMessages = function () {
        $.ajax({
            url: '/api/Chat/GetChat?with=' + self.sendMessageTo(),
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'GET',
            success: function (data) {
                var msg = $.map(data, function (item) { return new Message(item,self.sendMessageTo()) });
                self.showChat(msg);
            },
            error: function () {
                toastr.error("failed to load message. Please refresh page and try again", "Error!");
                return null;
            }
        });
    }
    self.checkMsgEnter = function (d, e) {
        if (self.loginUserId1) {
            if (self.newMessage().length < 1000) {
                if (e.keyCode == 13) {
                    self.sendMessage();
                }
            } else {
                self.newMessage(self.newMessage().slice(0, -1));
                toastr.info("You reached the limit", "Message too long!");
            }
        } else {
            loginBtn();
        }
    }
    self.hub.client.loadNewMessage = function (data) {
        self.newMessage('');
        if (data != null) {
            self.showChat.push(new Message(data,self.sendMessageTo()));
        }
    }
    self.onlineUsersHub.client.showConnected = function (connectionId) {
        var mape = $.map(connectionId, function (item) { return new OnlineUsers(item) });
        self.onlineUsers(mape);
        self.checkUserOnline();
        console.log(self.onlineUsers());
    }
    self.getReceiverId = function (email) {
        $.ajax({
            url: '/api/Chat/GetIdByEmail?email=' + email,
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'POST',
            success: function (data) {
                return data;
            },
            error: function () {
                toastr.error("failed to send message", "Error!");
                return null;
            }
        });
    }
    self.sendMessage = function () {
        if (self.sendMessageTo()) {
            var msg = new Message();
            msg.sentTo = self.sendMessageTo();
            msg.message = self.newMessage();
            msg.message = $.trim(msg.message);
            if (msg.message != "") {
                self.hub.server.addMessage(msg).fail(function (err) { toastr.error("failed to send message", "Error!"); });
            } else {
                toastr.info("You cannot send empty message");
            }
        } else {
            toastr.info("Go to online users list or user profile to send message","send messge to whom?");
        }
    }

}

