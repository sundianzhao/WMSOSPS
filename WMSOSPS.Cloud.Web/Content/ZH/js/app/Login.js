(function ($) {
    "use strict";

    $(function () {
        $.login.init();
        $.login.IsScanning();
        $("#changeTab").on("click", function () {
            $("#ScanLogin").hide();
            $("#ManualLogin").show();
        });
    });
    $.login = {
        formMessage: function (msg) {
            $('.login_tips').find('.tips_msg').remove();
            $('.login_tips').append('<div class="tips_msg"><i class="fa fa-question-circle"></i>' + msg + '</div>');
        },
        loginClick: function () {
            var $username = $("#txt_account");
            var $password = $("#txt_password");
            var $code = $("#txt_code");
            if ($username.val() == "") {
                $username.focus();
                $.login.formMessage('请输入用户名/手机号/邮箱。');
                return false;
            } else if ($password.val() == "") {
                $password.focus();
                $.login.formMessage('请输入登录密码。');
                return false;
            } else if ($code.val() == "") {
                $code.focus();
                $.login.formMessage('请输入验证码。');
                return false;
            } else {
                var display = $('#sms_div').css('display');
                if (display == "none") {
                    $("#login_button").attr('disabled', 'disabled').find('span').html("loading...");
                    $.ajax({
                        url: "/Login/CheckLogin",
                        data: { username: $.trim($username.val()), password: $.trim($password.val()), code: $.trim($code.val()) },
                        type: "post",
                        dataType: "json",
                        success: function (data) {
                            if (data.Success) {
                                $("#loginType").val(data.LoginType);
                                if (data.IsSmsLogin == "1") {
                                    if (data.Msg != "") {
                                        alert(data.Msg);
                                    }
                                    $("#sms_div").show();
                                    $("#IsSmsLogin").val(data.IsSmsLogin);
                                    $("#MoPhone").val(data.MoPhone);
                                    $("#login_button").removeAttr('disabled').find('span').html("登录");
                                    $("#switchCode").trigger("click");
                                    $code.val('');
                                    $.login.formMessage(data.Msg);
                                    SetCookie("the_cookie", null);
                                    // getsms();
                                    // StartTime();
                                } else {
                                   $.login.login($.trim($username.val()), $.trim($password.val()));
                                }
                            } else {
                                $("#login_button").removeAttr('disabled').find('span').html("登录");
                                $("#switchCode").trigger("click");
                                $code.val('');
                                $.login.formMessage(data.Msg);
                            }
                        }
                    });
                } else {
                    //验证手机短信登录
                    var smscode = $("#msg_code").val();
                    if (smscode == "") {
                        $.modalAlert("请输入短信验证码！","warning");
                        return false;
                    }
                    $.ajax({
                        url: "../../Ashx/sms.ashx?t=1&smscode=" + smscode,
                        cache: false,
                        success: function (data) {
                            data = eval("(" + data + ")");
                            if (data.success == 1) {
                                $.login.login($.trim($username.val()), $.trim($password.val()));
                                //验证正确
                                //$.ajax({
                                //    url: "/Login/CheckPhone",
                                //    data: { username: $.trim($username.val()), password: $.trim($password.val()), code: $.trim($code.val()).toLocaleLowerCase() },
                                //    type: "post",
                                //    dataType: "json",
                                //    success: function (data) {
                                //        if (result.sucess) {
                                //            $.login.login(username, password);
                                //        }
                                //        else {
                                //            alert(result.message);
                                //        }
                                //    },
                                //    error: function (err) { }
                                //});
                            } else if (data.success == -1) {
                                //验证失败
                                $.login.formMessage(data.message);
                            } else if (data.success == -2) {
                                //重新登陆
                                $.login.formMessage(data.message);
                                //window.location.href = "login.aspx";
                            }
                            else if (data.success == -3) {
                                //alert(data.message); //验证失败
                                $.login.formMessage(data.message);
                            }
                        }
                    });
                }
            }
        },
        login: function (username, password) {
            $.ajax({
                url: "/Login/Login",
                data: { username: username, password: password, code: $.trim($("#txt_code").val()), loginType: $("#loginType").val() },
                type: "post",
                dataType: "json",
                success: function (result) { 
                    if (result.success) {
                        window.location.href = "../../home/index";
                    }
                    else {
                        if (result.success == false) {
                            //alert(result.message);
                            //alert('账号或密码错误!');
                            $("#login_button").removeAttr('disabled').find('span').html("登录");
                            $("#txt_code").val('');
                            $.login.formMessage(result.message);
                        }
                        else {
                            $("#switchCode").trigger("click");
                            $('#txt_account').val('');
                            $('#txt_code').val('');
                            $('#txt_account').focus();
                            $("#login_button").removeAttr('disabled').find('span').html("登录");
                        }
                    }
                },
                error: function (err) { }
            });
        },
        init: function () {
            $('.wrapper').height($(window).height());
            $(".container").css("margin-top", ($(window).height() - $(".container").height()) / 2 - 50);
            $(window).resize(function (e) {
                $('.wrapper').height($(window).height());
                $(".container").css("margin-top", ($(window).height() - $(".container").height()) / 2 - 50);
            });
            $("#switchCode").click(function () {
                $("#imgcode").attr("src", "/Login/GetAuthCode?time=" + Math.random());
            });
            var login_error = top.$.cookie('nfine_login_error');
            if (login_error != null) {
                switch (login_error) {
                    case "overdue":
                        $.login.formMessage("系统登录已超时,请重新登录");
                        break;
                    case "OnLine":
                        $.login.formMessage("您的帐号已在其它地方登录,请重新登录");
                        break;
                    case "-1":
                        $.login.formMessage("系统未知错误,请重新登录");
                        break;
                }
                top.$.cookie('nfine_login_error', '', { path: "/", expires: -1 });
            }
            $("#login_button").click(function () {
                $.login.loginClick();
            });
            $("#asms").click(function () {
                $.login.getsms();
            });
            document.onkeydown = function (e) {
                if (!e) e = window.event;
                if ((e.keyCode || e.which) == 13) {
                    document.getElementById("login_button").focus();
                    document.getElementById("login_button").click();
                }
            }
        },
        //获取短信验证码
        getsms: function () {
            var username = document.getElementById("txt_account").value;
            var password = document.getElementById("txt_password").value;
            if (username == "" || password == "") {
                $.modalAlert("用户名或密码为空","warning");
                $("#switchCode").trigger("click");
                $('#txt_password').val('');
                $('#txt_code').val('');
                $('#txt_password').focus();
                return false;
            }
            var sec = 59; //设置秒数
            var secCookie = GetCookie("the_cookie"); //秒数

            if (secCookie == null || isNaN(secCookie)) {
                SetCookie("the_cookie", sec);
                secCookie = GetCookie("the_cookie"); //秒数
            }
            if (secCookie >= 0 && secCookie < sec) return false;
            $.login.StartTime();

            $.ajax({
                url: "../../Ashx/sms.ashx?usertype=" + $("#loginType").val() + "&workid=" + username + "&pwd=" + password,
                cache: false,
                success: function (data) {
                    if (data) {
                        data = eval("(" + data + ")");
                        if (data.success == -2) {
                            //重新登陆
                            $.modalAlert(data.message,"warning");
                            window.location.href = "/Login/Index";
                        }
                    }
                }
            });
        },

        StartTime: function () {

            var secCookie = GetCookie("the_cookie"); //秒数

            if (secCookie == null || isNaN(secCookie)) {
                return;
            }
            else {
                $("#login_button").attr("disabled", false);
            }
            $("#asms").css({ "color": "#b9bbba", "cursor": "default" });
            var t = setInterval(function () {

                $("#sec").text(secCookie);
                secCookie--;
                SetCookie("the_cookie", secCookie);

                if (secCookie < 0) {

                    clearInterval(t);
                    SetCookie("the_cookie", null);
                    $("#sec").text("");
                    $("#asms").css({ "cursor": "auto", "color": "blue" });

                    $("#asms").hover(function () {
                        var ss = $("#sec").text();
                        if (ss == 0) {
                            $("#asms").css("color", "Red");
                        }
                    }, function () {
                        if (ss == 0) {
                            $("#asms").css("color", "Blue");
                        }
                    });
                }

            }, 1000);

            setTimeout(function () {
                $.ajax({
                    url: "../../Ashx/sms.ashx?t=2",
                    cache: false
                });
            }, 120000);

        },
        IsScanning: function () {
            var t;
            var parameter = {};
            parameter["registerQR"] = $("#registerQR").val();
            parameter["ticket"] = $("#Ticket").val();
            $.ajax({
                type: 'post',
                url: '/Login/IsScanning?t=' + Math.random(),
                data: parameter,
                dataType: "json",
                success: function (result) {

                    if (result.state == 'success') {
                        var openId = result.message;
                        clearTimeout(t);
                        //window.location.href = '/WebSite/NewIndex';
                        $.login.loginScan(openId);
                        return false;
                    }
                    else {
                        t = setTimeout(function () {
                            $.login.IsScanning();
                        }, 1000);
                    }
                }
            });
        },
        loginScan: function (openId) {
            jshelper.ajaxPost("/Login/LoginScan", JSON.stringify({ openId: openId }), function (data) {
                if (data.Success) {
                    $.login.login(data.Account, data.PassWord);
                } else {
                    $.modalAlert(data.message, "error");
                }
            });
        }
    };

})(jQuery);