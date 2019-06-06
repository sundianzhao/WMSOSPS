(function ($) {
    "use strict";
    var type = $.request("type");
    var keyValue = $.request("keyValue");
    var rows;
    $(function () {
        //$("#watchTitle").val($("#watchTitlehid").val());
        $.NoticeInfoForm.setClass();
       // $.NoticeInfoForm.initialize();
        if (type == "add") {
            $.NoticeInfoForm.KindEditor();
            $.NoticeInfoForm.btn_add();
        } else if (type == "edit") {
            $.NoticeInfoForm.GetFormJson();
            $.NoticeInfoForm.KindEditor();
            $.NoticeInfoForm.btn_edit();
        }
        else if (type == "audit") {
            $.NoticeInfoForm.GetFormJson();
            $.NoticeInfoForm.KindEditor();
            $.NoticeInfoForm.btn_edit();
            $("#btnInfo_audit").show();
        }
    });
    var editor;
    $.NoticeInfoForm = {
        KindEditor: function () {
            KindEditor.ready(function (K) {
                editor = K.create('#watchContend', {
                    themeType: 'simple',
                    resizeType: 0,
                    uploadJson: hostUrl + '/Ashx/upload_json.ashx',
                    fileManagerJson: hostUrl + '/Ashx/file_manager_json.ashx', 
                    //items: ['undo', 'redo', '|', 'preview', 'print', 'template', 'code', 'cut', 'copy', 'paste',
                    //        'plainpaste', 'wordpaste', '|', 'justifyleft', 'justifycenter', 'justifyright',
                    //        'justifyfull', 'insertorderedlist', 'insertunorderedlist', 'indent', 'outdent', 'subscript',
                    //        'superscript', 'clearhtml', 'quickformat', 'selectall', '|', 'fullscreen', '/',
                    //        'formatblock', 'fontname', 'fontsize', '|', 'forecolor', 'hilitecolor', 'bold',
                    //        'italic', 'underline', 'strikethrough', 'lineheight', 'removeformat', 'image', 'multiimage', 'media', '|', 'emoticons', 'table', 'hr', 'baidumap', 'pagebreak',
                    //        'anchor', 'link', 'unlink'
                    //]
                    items: ['undo', 'redo', '|', 'preview', 'print', 'template', 'code', 'cut', 'copy', 'paste',
       'plainpaste', 'wordpaste', '|', 'justifyleft', 'justifycenter', 'justifyright',
       'justifyfull', 'insertorderedlist', 'insertunorderedlist', 'indent', 'outdent', 'subscript',
       'superscript', 'clearhtml', 'quickformat', 'selectall', '|', 'fullscreen', '/',
       'formatblock', 'fontname', 'fontsize', '|', 'forecolor', 'hilitecolor', 'bold',
       'italic', 'underline', 'strikethrough', 'lineheight', 'removeformat', '|', 'image', 'multiimage',
       'flash', 'media', 'insertfile', 'table', 'hr', 'emoticons', 'baidumap', 'pagebreak',
       'anchor', 'link', 'unlink'
                    ]
                });
                if (type == "edit" || type == "audit") {
                    editor.html(rows.Content);
                }
            });
        },
        setClass: function () {
            $('#classRadio input').iCheck({
                checkboxClass: 'icheckbox_square-green',
                radioClass: 'iradio_square-green',
                increaseArea: '20%'
            });
        },
       
        GetFormJson: function () {
            $.ajax({
                url: "/SystemManage/Notice/GetNoticeFormJson",
                data: { keyValue: keyValue },
                dataType: "json",
                async: false,
                success: function (data) {
                    rows = data;
                    if (!rows.IsTop)
                        $("#IsTopNo").iCheck('check');
                    else
                        $("#IsTopYes").iCheck('check');
                    if (!rows.IsUse)
                        $("#IsUseNo").iCheck('check');
                    else
                        $("#IsUseYes").iCheck('check');

                }
            });
        }, 
        btn_edit: function () {
            $("#form1").formEdit(rows);
            $("#Title").val(rows.Title);
          
            if (!rows.IsTop)
                $("#IsTopNo").iCheck('check');
            else
                $("#IsTopYes").iCheck('check');
 
            if (!rows.IsUse)
                $("#IsUseNo").iCheck('check');
            else
                $("#IsUseYes").iCheck('check');

        },
        submitForm: function () {
            editor.sync(); 
            var data = $("#form1").formSerialize(); 
            data += "&Type=" + type + "&nId=" + keyValue;
            $.submitForm({
                url: "/SystemManage/Notice/SubmitForm",
                param: encodeURI(data),
                success: function () {
                    $.currentWindow().$("#gridList").trigger("reloadGrid");
                }
            });
        },
        LoadGriad: function () {
            var iframeId = top.$(".NFine_iframe:visible").attr("id");
            top.frames[iframeId].$("#gridList").trigger("reloadGrid");
        },
        Audit: function (state) {
            var failmsg = $("#FailMsg").val();
            if (state == 2 && failmsg=="") {
                return $.modalMsg("请填写失败原因", "error");
            }
            $.ajax({
                type: "Post",
                url: "/SystemManage/Notice/AuditSuccess?keyValue=" + keyValue + "&state=" + state + "&failmsg=" + failmsg,
                data: {   },
                dataType: "json",
                async: false,
                success: function (data) {
                    if (data.state) {
                        $.modalMsg("操作成功", "success");
                        $.NoticeInfoForm.LoadGriad();
                        $.modalClose();
                    }
                    else {
                        $.modalAlert("操作失败", "error");
                    }
                }
            });
        },

    }
})(jQuery);