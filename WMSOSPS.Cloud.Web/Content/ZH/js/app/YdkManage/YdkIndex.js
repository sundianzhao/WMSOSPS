(function ($) {
    "use strict"; 
    $(function () { 
        $.Index.gridList();
        $("#btn_search").click(function () {
            var $gridList = $("#gridList");
            $gridList.jqGrid('setGridParam', {
                postData: { keyword: $("#txt_keyword").val() }
            }).trigger('reloadGrid', [{ page: 1 }]);
        });
    });
    $.Index = {
        gridList: function () {
            var $gridList = $("#gridList");
            $gridList.dataGrid({
                url: "/YdkManage/Ydk/GetGridJson", 
                height: $(window).height() - 128,
                colModel: [ 
                    { label: '主键', name: 'F_Code', hidden: true },
                    {
                        label: '编号', name: '', width: 180, align: 'left',
                        formatter: function (cellvalue, options, rowObject) {
                            return rowObject.F_Code;
                        }
                    },
                    { label: '名称', name: 'F_Name', width: 180, align: 'left' },
                    { label: '所属企业', name: 'F_EnterpriseName', width: 180, align: 'left' },
                    { label: '结算方式', name: 'F_BillName', width: 180, align: 'left' },

                    { label: '误差(kg)', name: 'F_AllowError', width: 180, align: 'left' },
                    { label: 'IP地址', name: 'F_IPAddress', width: 180, align: 'left' }, 
                    { label: '地点', name: 'F_Position', width: 120, align: 'left' }, 
                ],
                pager: "#gridPager",
                sortname: 'F_Code desc',
                viewrecords: true,
            });

        },
        btn_add: function () {
            $.modalOpen({
                id: "Form",
                title: "添加",
                url: "/YdkManage/Ydk/YdkForm",
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
                        url: "/YdkManage/Ydk/DeleteInfo",
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