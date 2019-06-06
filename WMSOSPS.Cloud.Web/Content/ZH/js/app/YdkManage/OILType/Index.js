(function ($) {
    "use strict";
    $(function () {
        $.Index.Init();
        $.Index.gridList();
        $("#btn_search").click(function () {
            var $gridList = $("#gridList");
            $gridList.jqGrid('setGridParam', {
                postData: { keyword: $("#F_EnterCode").val() }
            }).trigger('reloadGrid', [{ page: 1 }]);
        });
       
    });
    $.Index = {
        Init: function () {
            $("#F_EnterCode").bindSelect({
                url: "/YdkManage/Ydk/GetEnterprise",
                id: "F_Code",
                text: "F_Name"
            });
        },
        gridList: function () {
            var $gridList = $("#gridList");
            $gridList.dataGrid({
                url: "/YdkManage/OILType/GetGridJson",
                height: $(window).height() - 128,
                colModel: [
                    { label: '主键', name: 'F_ID', hidden: true },
                    
                    { label: '油品名称', name: 'F_Name', width: 180, align: 'left' },
                    { label: '企业名称', name: 'F_EnterpriseName', width: 180, align: 'left' },
                  
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
                url: "/YdkManage/OILType/Form",
                width: "450px",
                height: "500px",
                callBack: function (iframeId) {
                    top.frames[iframeId].$.Form.submitForm();
                }
            });
        },
        btn_delete: function () {
            var keyValue = $("#gridList").jqGridRowValue().F_Code;
            if (keyValue == "" || keyValue == null) {
                return $.modalAlert("请选择一条记录", "warning");
            }
            $.modalConfirm("注：您确定要【删除】该项吗？", function (r) {
                if (r) {
                    $.ajax({
                        type: "Post",
                        url: "/YdkManage/OILType/DeleteInfo",
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
    };

})(jQuery);