(function ($) {
    "use strict";
    var keyValue = $.request("keyValue");
    $(function () { 
        if (!!keyValue) {
            $.ajax({
                url: "/YdkManage/Car/GetLoginFormJson",
                data: { F_OrderNo: keyValue },
                dataType: "json",
                async: false,
                success: function (data) {
                    $("#form1").formSerialize(data);
                }
            });
        }
    });
    $.Form = {
        submitAuditForm: function () {
            if (!$('#form1').formValid()) {
                return false;
            } 
            $.loading(true);
            $.ajax({
                type: "Post",
                url: "/YdkManage/Car/AuditForm",
                data: { F_OrderNo: $("#F_OrderNo").val(), TruckWeight: $("#F_TruckWeight").val(), GrossWeight: $("#F_GrossWeight").val(), NetWeight: $("#F_NetWeight").val() },
                dataType: "json",
                success: function (data) {
                    $.loading(false);
                    if (data.state) {
                        $.modalMsg("操作成功", "success");
                        $.currentWindow().$("#gridList").trigger("reloadGrid");
                        $.modalClose();
                    }
                    else {
                        $.modalAlert(data.message, "error");
                    }
                }
            })
        }
    };
})(jQuery);
