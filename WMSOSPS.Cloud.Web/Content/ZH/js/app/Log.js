(function ($) {
    "use strict";

    $(function () {
        $.log.init();
        $("#time_horizon a.btn-default").click(function () {
            $("#time_horizon a.btn-default").removeClass("active");
            $(this).addClass("active");
            $('#btn_search').trigger("click");
        });
        $("#btn_search").click(function () {
            var timeType = $("#time_horizon a.active").attr('data-value');
            var queryJson = {
                keyword: $("#txt_keyword").val(),
                timeType: timeType,
                level: $('#level').val(),
                LogType: $("#LogType").val(),
                UserName: $("#userName").val(),
                startTime: $("#startTime").val(),
                endTime: $("#EndTime").val()
            }
            var $gridList = $("#gridList");
            $gridList.jqGrid('setGridParam', {
                postData: { queryJson: JSON.stringify(queryJson) },
                page: 1
            }).trigger('reloadGrid', [{ page: 1 }]);
        });
    });
    $.log = {
        init: function () {
            var $gridList = $("#gridList");
            var timeType = $("#time_horizon a.active").attr('data-value');
            var queryJson = {
                keyword: $("#txt_keyword").val(),
                timeType: timeType,
                level: $('#level').val(),
                LogType: $("#LogType").val(),
                UserName: $("#userName").val().trim(),
                startTime: $("#startTime").val(),
                endTime: $("#EndTime").val()
            }
            $gridList.dataGrid({
                url: "/SystemSecurity/Log/GetGridJson",
                postData: { queryJson: JSON.stringify(queryJson) },
                height: $(window).height() - 128,
                colModel: [
                    { label: '主键', name: 'Id', hidden: true },
                    { label: '账户', name: 'UserName', width: 80, align: 'left' },
                    { label: '日志级别', name: 'LogLevel', width: 60, align: 'left' },
                    { label: '日志器', name: 'Logger', width: 80, align: 'left', hidden: true },
                    { label: '创建时间', name: 'CreateTime', width: 120, align: 'left' },
                    { label: '记录信息', name: 'Message', width: 250, align: 'left' },
                    { label: '异常信息', name: 'Exception', width: 250, align: 'left' },
                    { label: '当前url', name: 'CurrentUrl', width: 180, align: 'left' },
                    { label: '上一个url', name: 'PrevUrl', width: 180, align: 'left' },
                    { label: '操作对象', name: 'ToObject', width: 50, align: 'left' },
                    { label: 'ip地址', name: 'UserIP', width: 80, align: 'left' },
                    { label: '线程', name: 'Thread', width: 30, align: 'left', hidden: true }
                ],
                pager: "#gridPager",
                sortname: 'CreateTime desc',
                viewrecords: true,
                rowList: [20, 50, 100],
                rowNum: 20,
                page: 1,
                beforeSelectRow: function (index) {
                    //var rowData = $("#gridList").jqGrid('getRowData', index);
                    //if (!!rowData && !!rowData.Id) {
                    //    $.modalAlert("【记录信息】:" + rowData.Message + "<br/><br/><br/>" + "【异常信息】:" + rowData.Exception);
                    //}
                },
                ondblClickRow: function (index) {
                    var rowData = $("#gridList").jqGrid('getRowData', index);
                    if (!!rowData && !!rowData.Id) {
                        $.modalAlert("【记录信息】:" + rowData.Message + "<br/><br/><br/>" + "【异常信息】:" + rowData.Exception);
                    }
                }
            });

        },
    };
})(jQuery);