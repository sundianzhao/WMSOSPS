(function ($) {
    "use strict";
    var _ty = $.request("ty"); 
    $(function () { 
        $("#upLoadFolder").val(_ty);
        $.Index.gridList();
        $("#btn_search").click(function () {
            var $gridList = $("#gridList");
            $gridList.jqGrid('setGridParam', {
                postData: { d1: $("#F_StatusTime1").val(), d2: $("#F_StatusTime2").val(), type:_ty }
            }).trigger('reloadGrid', [{ page: 1 }]);
        });
    });
    $.Index = {
        gridList: function () {
            var $gridList = $("#gridList");
            $gridList.dataGrid({
                url: "/SystemManage/UploadContent/GetGridJson",
                postData: { type: _ty },
                height: $(window).height() - 128,
                colModel: [
                    { label: '主键', name: 'F_ID', hidden: true },
                    { label: '路径', name: 'F_Url', hidden: true },
                    { label: '文件名称', name: 'F_Filename', width: 180, align: 'left' }, 
                    {
                        label: '下载', name: '', width: 120, align: 'left',
                        formatter: function (cellvalue, options, rowObject) {
                            if (rowObject.F_Url) {
                                return "<a href=\"javascript:void(0);\"  target=\"_blank\" onclick=\"$.Index.UploadWord('" + AttachCK + rowObject.F_Url + "')\">点击下载</a>"; 
                            }
                        }
                    },
                    { label: '上传时间', name: 'F_DateTime', width: 120, align: 'left' },
                    { label: '上传人', name: 'F_UserName', width: 120, align: 'left' },
                    { label: '描述', name: 'F_Description', width: 280, align: 'left' }  
                ],
                pager: "#gridPager",
                sortname: 'F_DateTime desc',
                viewrecords: true, 
            });

        },
        btn_add: function () {
            $("#FilePath").val('');
            $("#Description").val('');
            $("#F_Date").val('');
            $("#myModalUpload").modal("show");
        },
       
        btn_delete: function () {
            var keyValue = $("#gridList").jqGridRowValue().F_ID;
            if (keyValue == "" || keyValue == null) {
                return $.modalAlert("请选择一条记录", "warning");
            }
            $.modalConfirm("注：您确定要【删除】该项吗？", function (r) {
                if (r) {
                    $.ajax({
                        type: "Post",
                        url: "/SystemManage/UploadContent/DeleteInfo",
                        data: { keyValue: keyValue },
                        dataType: "json",
                        success: function (data) {
                            if (data.state) {
                                $.modalMsg("操作成功", "success");
                                $.currentWindow().$("#gridList").trigger("reloadGrid");
                            }
                            else {
                                $.modalAlert("操作失败", "error");
                            }
                        }
                    })
                }
            });
        },
        UploadFile: function () {
            var _file = $("#FilePath").val();
            if (_file == "" || _file == null) {
                $.modalAlert("请上传文件", "warning"); return;
            }
            var _date = $("#F_Date").val();
            if (_date == "" || _date == null) {
                $.modalAlert("请设置上传时间", "warning"); return;
            }
            $.loading(true, "上传中...");
           

            var formData = new FormData($("#formAttach")[0]);
            $.ajax({
                url: url,
                type: "POST",
                data: formData,
                dataType: "json",
                async: false,
                cache: false,
                contentType: false,
                processData: false,
                success: function (data) {
                     
                    if (data.success == false) {
                        $.loading(false);
                        $.modalAlert(data.message, "warning"); return;
                    } 
                    var _filename = data[0].File;
                    var _filepath = data[0].FilePath;
                    var src = AttachCK;
                    var strsrc = src + data[0].FilePath.replace(/\\\\/g, "/").replace(/\\/g, "/");
                    //alert(_filename);
                    //alert(_filepath);
                    //alert(strsrc);
                    //alert(data[0].htmlpath);
                    $.Index.SubmitData(_filename, _filepath);
                },
                error: function (data) {
                    $.loading(false);
                    $.modalAlert("上传失败", "error"); return;
                }
            });
        },
        SubmitData: function (filename,filepath) {
            $.ajax({
                type: "Post",
                url: "/SystemManage/UploadContent/AddUploadContent",
                data: {
                    Type: _ty, FileName: filename, FilePath: filepath,
                    F_Description: $("#Description").val(),
                    f_date: $("#F_Date").val()
                },
                dataType: "json",
                success: function (data) {
                    $.loading(false);
                    if (data.state) {
                        $("#btnCloseUpload").click();
                        $.modalMsg("操作成功", "success");
                        $.currentWindow().$("#gridList").trigger("reloadGrid");
                    }
                    else {
                        $.modalAlert(data.msg, "error");
                    }
                }
            })
        },
        UploadWord: function (value) {
            window.open(value);
        }
    };

})(jQuery);