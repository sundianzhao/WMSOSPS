(function ($) {
    "use strict";
    var UserCode = top.clients.user;
    $(function () {
        if (UserCode.LoginType==2) {
            $("#divt1").hide();
            $("#divt2").hide();
        }
        $.Index.gridList();
        $("#btn_search").click(function () {
            var $gridList = $("#gridList");
            $gridList.jqGrid('setGridParam', {
                postData: { title: $("#Title").val(), d1: $("#F_StatusTime1").val(), d2: $("#F_StatusTime2").val() }
            }).trigger('reloadGrid', [{ page: 1 }]);
        });
    });
    $.Index = {
        gridList: function () {
            var $gridList = $("#gridList");
            $gridList.dataGrid({
                url: "/SystemManage/Notice/GetGridJson",
                height: $(window).height() - 128,
                colModel: [
                    { label: '主键', name: 'nID', hidden: true },
                    { label: 'AuditStatus', name: 'AuditStatus', hidden: true },
                    {
                        label: '标题', name: 'Title', width: 700, align: 'left',
                        formatter: function (cellvalue, options, rowObject) {
                            return "<a href='javascript:void(0)' onclick=$.Index.Search('" + rowObject.nID + "')>" + cellvalue + "</a>";
                        }
                    },
                    (UserCode.LoginType == 1) ? 
                        { label: '创建人', name: 'CreateWorkerID', width: 180, align: 'left' } :
                        { label: '创建人', name: 'CreateWorkerID', width: 180, align: 'left', hidden: true  },
                    (UserCode.LoginType == 1) ?
                        {
                            label: '是否置顶', name: 'IsTop', width: 80, align: 'left',
                            formatter: function (cellvalue, options, rowObject) {
                                if (cellvalue)
                                    return "是";
                                else
                                    return '否';
                            }

                        } : {
                            label: '是否置顶', name: 'IsTop', width: 80, align: 'left', hidden: true 
                        },
                     (UserCode.LoginType == 1 ) ? 
                    {
                        label: '审核状态', name: '', width: 80, align: 'left',
                        formatter: function (cellvalue, options, rowObject) {
                            if (rowObject.AuditStatus == 0)
                                return '<span class=\"label label-warning\">待审核</span>';
                            else if (rowObject.AuditStatus == 1) {
                                return '<span class=\"label label-success\">审核通过</span>';
                            }
                            else if (rowObject.AuditStatus == 2) {
                                return '<span class=\"label label-error\">审核失败</span>';
                            }
                        }
                    }:
                    { label: '审核状态', name: '', hidden: true },
                     (UserCode.LoginType == 1 ) ? 
                    {
                        label: '审核人', name: 'Aditor', width: 80, align: 'left'
                    } : { label: '审核人', name: 'Aditor', hidden: true },
                   { label: '发布时间', name: 'JoinTime', width: 180, align: 'left' },
                     (UserCode.LoginType == 1 ) ? 
                   { label: '审核备注', name: 'FailMsg', width: 280, align: 'left' } : { label: '审核备注', name: 'FailMsg', hidden: true },
                ],
                pager: "#gridPager",
                sortname: 'IsTop desc,JoinTime desc',
                viewrecords: true,
                beforeSelectRow: function (index) {
                    var row = $("#gridList").jqGrid('getRowData', index);
                    if (row.AuditStatus = 2) {
                        $("#NF-edit").parent().show();
                        $("#NF-delete").parent().show();
                    }
                    else {
                        $("#NF-edit").parent().hide();
                        $("#NF-delete").parent().hide();
                    }
                }
            });

        },
        btn_add: function () {
            $.modalOpen({
                id: "Form",
                title: "添加公告",
                url: "/SystemManage/Notice/Form?type=" + "add",
                width: "800px",
                height: "600px",
                btn: ["提交", "取消"],
                callBack: function (iframeId) {
                    top.frames[iframeId].$.NoticeInfoForm.submitForm();
                }
            });
        },
        btn_audit: function () {
            var keyValue = $("#gridList").jqGridRowValue().nID;
            if (keyValue == "" || keyValue == null) {
                return $.modalAlert("请选择一条记录", "warning");
            }
            $.modalOpen({
                id: "Form",
                title: "审核",
                url: "/SystemManage/Notice/Form?type=" + "audit&keyValue=" + keyValue,
                width: "800px",
                height: "600px",
                btn: null
                //callBack: function (iframeId) {
                //    top.frames[iframeId].$.NoticeInfoForm.submitAuditForm();
                //}
            });
        },
        btn_edit: function () {
        var keyValue = $("#gridList").jqGridRowValue().nID;
        if (keyValue == "" || keyValue == null) {
            return $.modalAlert("请选择一条记录", "warning");
        }
        $.modalOpen({
            id: "Form",
            title: "编辑公告",
            url: "/SystemManage/Notice/Form?keyValue=" + keyValue + "&type=" + "edit",
            width: "800px",
            height: "600px",
            btn: ["提交", "取消"],
            callBack: function (iframeId) {
                top.frames[iframeId].$.NoticeInfoForm.submitForm();
            }
        });
        },
        btn_delete: function () {
            var _keyValue = $("#gridList").jqGridRowValue().nID;
            if (_keyValue == "" || _keyValue == null) {
                return $.modalAlert("请选择一条记录", "warning");
            }
            $.modalConfirm("注：您确定要【删除】该项吗？", function (r) {
                if (r) {
                    $.ajax({
                        type: "Post",
                        url: "/SystemManage/Notice/DeleteNotice",
                        data: { keyValue: _keyValue },
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
        Search: function (id) {
            $.modalOpen({
                id: "NoticeInfoForm",
                title: "公告信息",
                url: "/SystemManage/Notice/SearchForm?keyValue=" + id,
                width: "800px",
                height: "550px",
                btn: [],
                callBack: null
            });
        }
    };

})(jQuery);