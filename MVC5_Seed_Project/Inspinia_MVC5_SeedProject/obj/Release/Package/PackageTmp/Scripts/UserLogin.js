
    function isValidEmailAddress(emailAddress) {
        var pattern = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        return pattern.test(emailAddress);
    };
function AccountViewModel() {
    var self = this; //this
    self.isUserLoginLoading = ko.observable(false);
    self.email = ko.observable();
    self.password = ko.observable();
    self.UserName = ko.observable();
    self.confirmPassword = ko.observable();
    self.loginError = ko.observable("");
    self.isPasswordSaved = ko.observable(true);
    self.checkEnterEmail = function (d, e) {
        self.loginError("");
        if (e.keyCode == 13) {
            self.submitEmail();
        }
    }
    self.loginBtn = function () {
        $("#inputEmail").modal('show');
        self.loginError("");
    }
    self.hasConfirmPasswordFocus = ko.observable(false);
    self.checkEnterNewPassword = function (d, e) {
        self.loginError("");
        if (e.keyCode == 13) {
            self.hasConfirmPasswordFocus(true);
        }
    }
    self.checkEnterConfirmPassword = function (d, e) {
        self.loginError("");
        if (e.keyCode == 13) {
            self.registerNewUser();
        }
    }
    self.checkEnterName = function (d, e) {
        self.loginError("");
        if (e.keyCode == 13) {
            self.submitName();
        }
    }
    self.checkEnterPassword = function (d, e) {
        self.loginError("");
        if (e.keyCode == 13) {
            self.checkLoginUserPassword();
        }
    }
    self.registerOnEmail = function () {
        self.loginError("");
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
                    self.loginError("Some Error has occured.Please try again");
                }
            },
            error: function () {
                self.loginError("failed to register. Please refresh page and try again");
            }
        });
    }
    self.submitEmail = function () {
        if (isValidEmailAddress(self.email())) {
            self.isUserLoginLoading(true);
            $.ajax({
                url: '/api/User/CheckEmail?email=' + self.email(),
                dataType: "json",
                contentType: "application/json",
                cache: false,
                type: 'POST',
                success: function (data) {
                    self.isUserLoginLoading(false);
                    $("#inputEmail").modal('hide');
                    if (data == "NewUser") {
                        $("#newUser").modal('show');
                    } else {
                        self.isPasswordSaved(data.isPasswordSaved);
                        $("#oldUser").modal('show');
                        self.UserName(data.name);
                    }
                    self.loginError("");
                },
                error: function () {
                    self.isUserLoginLoading(false);
                    self.loginError("failed to send Email. Please refresh page and try again");
                }
            });
        }else{
            self.loginError("Please enter a valid email address");
        }
    }
    self.checkLoginUserPassword = function () {
        self.loginError("");
        self.isUserLoginLoading(true);
        $.ajax({
            url: '/Account/UserLogin?email='+self.email() + '&password=' + self.password(),
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'POST',
            success: function (data) {
                self.isUserLoginLoading(false);
                if (data == "Done") {
                    self.loginError("");
                        
                    location.reload();
                } else {
                    self.loginError("Incorrect password. Forget Password?");
                }
            },
            error: function () {
                self.isUserLoginLoading(false);
                self.loginError("failed to send password. Please refresh page and try again");
            }
        });
    }
    self.registerNewUser = function () {
        self.loginError("");
        if (self.password() === self.confirmPassword()) {
            if (typeof self.password() != "undefined") {
                self.isUserLoginLoading(true);
                $.ajax({
                    url: '/Account/RegisterUser?email=' + self.email() + '&password=' + self.password(),
                    dataType: "json",
                    contentType: "application/json",
                    cache: false,
                    type: 'POST',
                    success: function (data) {
                        self.isUserLoginLoading(false);
                        if (data == "Done") {
                            self.loginError("");
                            $("#newUser").modal('hide');
                            $("#inputName").modal('show');
                        } else {
                            self.loginError("Some error has occured");
                        }
                    },
                    error: function () {
                        self.isUserLoginLoading(false);
                        self.loginError("failed to send password. Please refresh page and try again");
                    }
                });
            }
            else {
                self.loginError("Please enter password");
            }
        }
        else {
            self.loginError("password does not match");
        }
    }
    self.submitName = function () {
        self.loginError("");
        self.UserName($.trim(self.UserName()));
        if (self.UserName()) {
            self.isUserLoginLoading(true);
            $.ajax({
                url: '/Account/SubmitName?name=' + self.UserName(),
                dataType: "json",
                contentType: "application/json",
                cache: false,
                type: 'POST',
                success: function (data) {
                    self.isUserLoginLoading(false);
                    if (data == "Done") {
                        // location.reload();
                        $("#inputName").modal('hide');
                        $("#EmailSent").modal('show');

                    } else {
                        self.loginError("Some error has occured.Please refresh page.");
                    }
                },
                error: function () {
                    self.isUserLoginLoading(false);
                    self.loginError("failed to send Name. Please refresh page and try again");
                }
            });
        }
        else {
            self.loginError("Please enter valid name");
        }
    }
    self.reloadPage = function () {
        location.reload();
    }
}