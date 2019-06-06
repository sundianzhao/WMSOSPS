(function ($) {
    "use strict";
    $(function () {
        $.Index.Init();
        $.Index.gridList();
        $("#btn_search").click(function () {
            var $gridList = $("#gridList");
            $gridList.jqGrid('setGridParam', {
                postData: { keyword: $("#ydk").val() }
            }).trigger('reloadGrid', [{ page: 1 }]);
        });

    });
    $.Index = {
        Init: function () {
            $("#ydk").bindSelect({
                url: "/YdkManage/SAPOILType/GetYdkItem",
                id: "F_Code",
                text: "F_Name"
            });
        },
        gridList: function () {
            var $gridList = $("#gridList");
            $gridList.dataGrid({
                url: "/YdkManage/SAPOILType/GetGridJson",
                height: $(window).height() - 128,
                colModel: [
                    { label: '主键', name: 'F_ID', hidden: true },
                 
                    { label: '物料号', name: 'F_OILCODE', width: 180, align: 'left' },
                    { label: '油品名称', name: 'F_OILName', width: 180, align: 'left' },
                    { label: '品名', name: 'F_INVOICE', width: 180, align: 'left' },
                    { label: '型号', name: 'F_TYPE', width: 180, align: 'left' },
                    { label: '物料描述', name: 'F_DESCRIPTION', width: 180, align: 'left' },

                ],
                pager: "#gridPager",
                sortname: 'F_ID desc',
                viewrecords: true,
            });

        },
        btn_add: function () {
            $.modalOpen({
                id: "Form",
                title: "添加",
                url: "/YdkManage/SAPOILType/Form",
                width: "450px",
                height: "500px",
                callBack: function (iframeId) {
                    top.frames[iframeId].$.Form.submitForm();
                }
            });
        },
        btn_edit: function () {
            var keyValue = $("#gridList").jqGridRowValue().F_ID;
            if (keyValue == "" || keyValue == null) {
                return $.modalAlert("请选择一条记录", "warning");
            }
            $.modalOpen({
                id: "Form",
                title: "编辑",
                url: "/YdkManage/SAPOILType/Form?keyValue=" + keyValue,
                width: "450px",
                height: "500px",
                callBack: function (iframeId) {
                    top.frames[iframeId].$.Form.submitForm();
                }
            });
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
                        url: "/YdkManage/SAPOILType/DeleteInfo",
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
        btn_search: function () {
            var $gridList = $("#gridList");
            $gridList.jqGrid('setGridParam', {
                postData: { keyword: $("#ydk").val() }
            }).trigger('reloadGrid', [{ page: 1 }]);
        }
    };

})(jQuery);