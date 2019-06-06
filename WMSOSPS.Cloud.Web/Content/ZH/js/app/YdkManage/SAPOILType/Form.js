(function ($) {
    "use strict";
    var keyValue = $.request("keyValue");
    $(function () {
        $.Form.initControl();
        if (!!keyValue) {
            $.ajax({
                url: "/YdkManage/SAPOILType/GetFormJson",
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
            $("#F_OILName").bindSelect({
                url: "/YdkManage/SAPOILType/GetOILTypes",
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
                url: "/YdkManage/SAPOILType/Sumbit",
                data: { Entity: $("#form1").formSerialize(), keyValue: keyValue },
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
