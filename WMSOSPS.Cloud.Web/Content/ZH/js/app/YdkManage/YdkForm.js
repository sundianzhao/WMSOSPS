(function ($) {
    "use strict";
    var keyValue = $.request("keyValue");
    $(function () {
        $.Form.initControl();
        if (!!keyValue) {
            $.ajax({
                url: "/LimitManage/MaterLimit/GetFormJson",
                data: { keyValue: keyValue },
                dataType: "json",
                async: false,
                success: function (data) {
                    $("#form1").formSerialize(data);
                }
            });
        }
    });
    $.Form = {
        initControl: function () {
            $("#F_BelongTo").bindSelect({
                url: "/YdkManage/Ydk/GetEnterprise",
                id: "F_Code",
                text: "F_Name"
            });
            $("#F_BillMethod").bindSelect({
                url: "/YdkManage/Ydk/GetBillMethod",
                id: "F_ID",
                text: "F_Name"
            });
        },
        submitForm: function () {
            if (!$('#form1').formValid()) {
                return false;
            }
            $.ajax({
                type: "Post",
                url: "/YdkManage/Ydk/Sumbit",
                data: { Entity: $("#form1").formSerialize() },
                dataType: "json",
                success: function (data) {
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
