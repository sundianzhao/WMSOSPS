(function ($) {
    "use strict";
    var keyValue = $.request("keyValue");
    $(function () {
        $.Form.initControl();
        if (!!keyValue) {
            $.ajax({
                url: "/YdkManage/OILType/GetFormJson",
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
            $("#F_EnterpriseCode").bindSelect({
                url: "/YdkManage/Ydk/GetEnterprise",
                id: "F_Code",
                text: "F_Name"
            }); 
        },
        submitForm: function () {
            if (!$('#form1').formValid()) {
                return false;
            }
            $.ajax({
                type: "Post",
                url: "/YdkManage/OILType/Sumbit",
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
