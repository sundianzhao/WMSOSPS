(function ($) {
    "use strict";
    $(function () {
        $.Index.Init();
        $.Index.gridList();
        $("#btn_search").click(function () {
            var $gridList = $("#gridList");
            $gridList.jqGrid('setGridParam', {
                postData: { location: $("#ydk").val() }
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
                url: "/YdkManage/Car/GetUnFinishedGridJson",
                height: $(window).height() - 128,
                colModel: [

                    { label: '提货单号', name: 'F_OrderNo', width: 180, align: 'left' },
                    { label: '车牌号', name: 'F_TruckNo', width: 180, align: 'left' },
                    { label: '设定量(吨)', name: 'F_OILDefine', width: 180, align: 'left' },
                    { label: '毛重(吨)', name: 'F_GrossWeight', width: 180, align: 'left' },
                    { label: '皮重(吨)', name: 'F_TruckWeight', width: 180, align: 'left' },
                    { label: '净重(吨)', name: 'F_NetWeight', width: 180, align: 'left' },
                    { label: '物料号', name: 'F_MaterialID', width: 180, align: 'left' },
                    {
                        label: '出场时间', name: 'F_LogoutTime', width: 180, align: 'left',
                        formatter: "date", formatoptions: { srcformat: 'Y-m-d', newformat: 'Y-m-d' }
                    },

                ],
                pager: "#gridPager",
                sortname: 'F_LogoutTime asc',
                viewrecords: true,
            });

        },
        btn_details: function () {
            var keyValue = $("#gridList").jqGridRowValue().F_OrderNo;
            if (keyValue == "" || keyValue == null) {
                return $.modalAlert("请选择一条记录", "warning");
            }
            $.modalOpen({
                id: "Form",
                title: "待结算车辆信息",
                url: "/YdkManage/Car/UnFinishedTruckForm?keyValue=" + keyValue,
                width: "450px",
                height: "500px",
                btn: null,
                callBack: null
                //callBack: function (iframeId) {
                //    top.frames[iframeId].$.Form.submitForm();
                //}
            });
        },
        btn_account: function () {
            var keyValue = $("#gridList").jqGridRowValue().F_OrderNo;
            if (keyValue == "" || keyValue == null) {
                return $.modalAlert("请选择一条记录", "warning");
            }
            $.modalOpen({
                id: "Form",
                title: "待结算车辆信息",
                url: "/YdkManage/Car/UnFinishedTruckForm?keyValue=" + keyValue,
                width: "450px",
                height: "500px",  
                callBack: function (iframeId) {
                    top.frames[iframeId].$.Form.submitForm();
                }
            });
        },
        btn_search: function () {
            var $gridList = $("#gridList");
            $gridList.jqGrid('setGridParam', {
                postData: { location: $("#ydk").val() }
            }).trigger('reloadGrid', [{ page: 1 }]);
        }
    };

})(jQuery);