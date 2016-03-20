
    function isValidEmailAddress(emailAddress) {
        var pattern = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        return pattern.test(emailAddress);
    };
function AccountViewModel() {
    var self = this;
    self.email = ko.observable();
    self.password = ko.observable();
    self.UserName = ko.observable();
    self.confirmPassword = ko.observable();
    self.error = ko.observable("");
    self.isPasswordSaved = ko.observable(true);
    self.checkEnterEmail = function (d, e) {
        self.error("");
        if (e.keyCode == 13) {
            self.submitEmail();
        }
    }
    self.loginBtn = function () {
        $("#inputEmail").modal('show');
    }
    self.hasConfirmPasswordFocus = ko.observable(false);
    self.checkEnterNewPassword = function (d, e) {
        self.error("");
        if (e.keyCode == 13) {
            self.hasConfirmPasswordFocus(true);
        }
    }
    self.checkEnterConfirmPassword = function (d, e) {
        self.error("");
        if (e.keyCode == 13) {
            self.registerNewUser();
        }
    }
    self.checkEnterName = function (d, e) {
        self.error("");
        if (e.keyCode == 13) {
            self.submitName();
        }
    }
    self.checkEnterPassword = function (d, e) {
        self.error("");
        if (e.keyCode == 13) {
            self.checkLoginUserPassword();
        }
    }
    self.registerOnEmail = function () {
        self.error("");
        $.ajax({
            url: '/Account/RegisterUser?email=' + self.email(),
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'POST',
            success: function (data) {
                if (data == "Done") {
                    location.reload();
                } else {
                    self.error("Some Error has occured.Please try again");
                }
            },
            error: function () {
                self.error("failed to register. Please refresh page and try again", "Error!");
            }
        });
    }
    self.submitEmail = function () {
        console.log(isValidEmailAddress(self.email()));
        if(isValidEmailAddress(self.email())){
            $.ajax({
                url: '/api/User/CheckEmail?email=' + self.email(),
                dataType: "json",
                contentType: "application/json",
                cache: false,
                type: 'POST',
                success: function (data) {
                    $("#inputEmail").modal('hide');
                    if (data == "NewUser") {
                        $("#newUser").modal('show');
                    } else {
                        self.isPasswordSaved(data.isPasswordSaved);
                        $("#oldUser").modal('show');
                        self.UserName(data.name);
                    }
                    self.error("");
                },
                error: function () {
                    self.error("failed to send Email. Please refresh page and try again", "Error!");
                }
            });
        }else{
            self.error("Please enter a valid email address");
        }
    }
    self.checkLoginUserPassword = function () {
        self.error("");
        $.ajax({
            url: '/Account/UserLogin?email='+self.email() + '&password=' + self.password(),
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'POST',
            success: function (data) {
                if (data == "Done") {
                    self.error("");
                        
                    location.reload();
                } else {
                    self.error("Incorrect password. Forget Password?");
                }
            },
            error: function () {
                self.error("failed to send password. Please refresh page and try again", "Error!");
            }
        });
    }
    self.registerNewUser = function () {
        self.error("");
        if (self.password() === self.confirmPassword()) {
            if (typeof self.password() != "undefined") {
                console.log(self.password());
                $.ajax({
                    url: '/Account/RegisterUser?email=' + self.email() + '&password=' + self.password(),
                    dataType: "json",
                    contentType: "application/json",
                    cache: false,
                    type: 'POST',
                    success: function (data) {
                        if (data == "Done") {
                            self.error("");
                            $("#newUser").modal('hide');
                            $("#inputName").modal('show');
                        } else {
                            self.error("Some error has occured");
                        }
                    },
                    error: function () {
                        self.error("failed to send password. Please refresh page and try again", "Error!");
                    }
                });
            }
            else {
                self.error("Please enter password");
            }
        }
        else {
            self.error("password does not match");
        }
    }
    self.submitName = function () {
        self.error("");
        self.UserName($.trim(self.UserName()));
        if (self.UserName()) {
            $.ajax({
                url: '/Account/SubmitName?name=' + self.UserName(),
                dataType: "json",
                contentType: "application/json",
                cache: false,
                type: 'POST',
                success: function (data) {
                    if (data == "Done") {
                        location.reload();
                    } else {
                        self.error("Some error has occured");
                    }
                },
                error: function () {
                    self.error("failed to send Name. Please refresh page and try again", "Error!");
                }
            });
        }
        else {
            self.error("Please enter valid name");
        }
    }
}