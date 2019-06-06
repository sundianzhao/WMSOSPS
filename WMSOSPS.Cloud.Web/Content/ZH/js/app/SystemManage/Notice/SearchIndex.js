(function ($) {
    "use strict";

    $(function () {
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
                url: "/SystemManage/Notice/GetGridJsonBySearch",
                height: $(window).height() - 128,
                colModel: [
                    { label: '主键', name: 'nID', hidden: true }, 
                    {
                        label: '标题', name: 'Title', width: 380, align: 'left',
                        formatter: function (cellvalue, options, rowObject) {
                            return "<a href=\"javascript:void(0);\"    onclick=\"$.Index.SearchForm('" + rowObject.nID + "')\">" + cellvalue + "</a>";
                        }
                    }, 
                    {
                        label: '是否置顶', name: 'IsTop', width: 80, align: 'left',
                        formatter: function (cellvalue, options, rowObject) {
                            if (cellvalue)
                                return "是";
                            else
                                return '否';
                        }

                    }, 
                    { label: '创建时间', name: 'JoinTime', width: 180, align: 'left' }, 
                ],
                pager: "#gridPager",
                sortname: 'JoinTime desc',
                viewrecords: true,
                beforeSelectRow: function (index) {
                  
                }
            });

        },
        SearchForm: function (keyValue) {
            $.modalOpen({
                id: "NoticeInfoForm",
                title: "公告信息",
                url: "/SystemManage/Notice/SearchForm?keyValue=" + keyValue,
                width: "800px",
                height: "550px",
                btn: [],
                callBack: null
            });
        }
        
       
        
       
    };

})(jQuery);