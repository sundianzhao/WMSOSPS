(function ($) {
    "use strict";
    var keyValue = $.request("keyValue");
    $(function () {
        if (!!keyValue) {
            $.ajax({
                url: "/YdkManage/Car/GetGrossFormJson",
                data: { F_OrderNo: keyValue },
                dataType: "json",
                async: false,
                success: function (data) {
                    $("#form1").formSerialize(data);
                    if (data.F_OBillImage != "" && data.F_OBillImage != null) {
                        $("#pic").attr('src', data.F_OBillImage);
                        $("#pic").show();
                    }
                }
            });
        }
    });
    $.Form = {
        fun_xx: function (value) {
            $("#liBasicInfo").removeClass("active");
            $("#liImgInfo").removeClass("active");
          
            $("#BasicInfo").hide();
            $("#ImgInfo").hide();
        
            $("#li" + value + "").addClass("active");
            $("#" + value + "").show();
        },
        Chakan: function () {
            var data = [];
            var First = $("#pic").attr('src');; 
            data.push(First);
            parent.PictureView(data, First);
        }
    };
})(jQuery);
