var pull;
(function ($) {
    "use strict";
    $(function () {
        top.pull.getAuditRecord();
        $("#realNameAudit").on('click', function () {
            //点击查看的话要表示已经查看了消息，要删除redis中的数据，同时数据库 中的数据要记录表示已经查看
            var type = "realNameAudit";
            top.pull.openMessage(type);
        });
        $("#agentAudit").on('click', function () {
            //点击查看的话要表示已经查看了消息，要删除redis中的数据，同时数据库 中的数据要记录表示已经查看
            var type = "agentAudit";
            top.pull.openMessage(type);
        });


        $("#WorkerApplyAudit").on('click', function () {
            //点击查看的话要表示已经查看了消息，要删除redis中的数据，同时数据库 中的数据要记录表示已经查看
            var type = "WorkerApply";
            top.pull.openMessage(type);
        });
        $("#MaterialTransferAudit").on('click', function () {
            //点击查看的话要表示已经查看了消息，要删除redis中的数据，同时数据库 中的数据要记录表示已经查看
            var type = "MaterialTransfer";
            top.pull.openMessage(type);
        });
        $("#enterpriseUserAudit").on('click', function () {
            //点击查看的话要表示已经查看了消息，要删除redis中的数据，同时数据库 中的数据要记录表示已经查看
            var type = "enterpriseUser";
            top.pull.openMessage(type);
        });

        $("#firstFailAudit").on('click', function () {
            //点击查看的话要表示已经查看了消息，要删除redis中的数据，同时数据库 中的数据要记录表示已经查看
            var type = "firstFail";
            top.pull.openMessage(type);
        });

        $("#privateCountLi").on('click', function () {
            //点击查看的话要表示已经查看了消息，要删除redis中的数据，同时数据库 中的数据要记录表示已经查看
            var type = "privateCount";
            top.pull.openMessage(type);
        });

        $("#AgreeAgent").on('click', function () {
            //点击查看的话要表示已经查看了消息，要删除redis中的数据，同时数据库 中的数据要记录表示已经查看
            var type = "agenterNum";
            top.pull.openMessage(type);
        });

        $("#sendMessage").on('click', function() {
            $("#myModal").modal('toggle');
        });
        $("#send").on('click', function () {
            top.pull.sendMessage();
        });
      

    });
    
    pull = {
        getAuditRecord: function() {
            var user = top.clients.user;
            var authorizeButton = top.clients.buttons;
            $.each(authorizeButton, function (index, item) {
                //代理商的话可以显示提单(审核失败)的消息
                if (item.F_EnCode == "radio_FirstAudit" || item.F_EnCode == "radio_LastAudit" || user.LoginType == 2) {
                    $("#realNameAudit").show();
                    return false;
                }
            });

            //管理员
            //if (user.LoginType == 1) {
            //    $("#agentAudit").show();
            //} else {
            //    //发送消息只能是代理商或者终端用户发给管理员
            //    $("#sendMessage").show();
            //}

            $.ajax({
                url: "/Home/GetAuditRecord",
                dataType: "json",
                async: false,
                success: function (data) {
                    if (!!data) {
                        if (data.firstCount != 0 || data.lastCount != 0 || data.numcount != 0) {
                            //$("#first").text(data.firstCount);
                            var count = parseInt(data.firstCount) + parseInt(data.lastCount) + parseInt(data.numcount);
                            $("#first").text(count);
                            $("#realNameAudit").show();
                        } else {
                            $("#realNameAudit").hide();
                        }
                        //初审失败
                        if (data.firstFialCount != 0) {
                            $("#firstAudit").text(data.firstFialCount);
                            $("#firstFailAudit").show();
                        } else {
                            $("#firstFailAudit").hide();
                        }
                        //代理审核
                        if (data.agentCount != 0) {
                            $("#agent").text(data.agentCount);
                            $("#agentAudit").show();
                        } else {
                            $("#agentAudit").hide();
                        }
                        //白名单
                        if (data.workApplyeCount != 0) {
                            $("#WorkerApply").text(data.workApplyeCount);
                            $("#WorkerApplyAudit").show();
                        } else {
                            $("#WorkerApplyAudit").hide();
                        }
                        //资料移交
                        if (data.materialTransferCount != 0) {
                            $("#MaterialTransfer").text(data.materialTransferCount);
                            $("#MaterialTransferAudit").show();
                        } else {
                            $("#MaterialTransferAudit").hide();
                        }
                        //企业用户
                        if (data.enterpriseUserCount != 0) {
                            $("#enterpriseUser").text(data.enterpriseUserCount);
                            $("#enterpriseUserAudit").show();
                        } else {
                            $("#enterpriseUserAudit").hide();
                        }
                        //号码协商
                        if (data.agreetAgentCount != 0) {
                            $("#agenterNum").text(data.agreetAgentCount);
                            $("#AgreeAgent").show();
                        } else {
                            $("#AgreeAgent").hide();
                        }

                        var count = parseInt(data.agreetAgentCount) + parseInt(data.firstCount) + parseInt(data.firstFialCount) + parseInt(data.lastCount) + parseInt(data.agentCount) + parseInt(data.numcount) + parseInt(data.workApplyeCount) + parseInt(data.materialTransferCount) + parseInt(data.enterpriseUserCount);
                        $("#AuditCount").text(count);

                        if (data.privateCount != 0) {
                            $("#privateCountSpan").text(data.privateCount);
                            $("#privateCountLi").show();
                        } else {
                            $("#privateCountLi").hide();
                        }
                        $("#privateCount").text(data.privateCount);
                    }
                }
            });
        },
        openMessage :function(type) {
            $.modalOpen({
                id: "RedisMessage",
                title: "系统消息",
                url: "/Home/Message?keyValue=" + type,
                width: "800px",
                height: "800px",
                btn: null,
                callBack: function (iframeId) {

                }
            });
        },
        sendMessage: function() {
            var message = $("#message").val();
            var user = top.clients.user;
            if (!message)
                return false;
            top.chat.server.sendMessageToAdmin(user.UserName, user.UserCode, message);
        }
    }
})(jQuery);