$(function () {
    document.onkeydown = function (event) {
        var e = event || window.event || arguments.callee.caller.arguments[0];
        if (e && e.keyCode == 27) { // 按 Esc 
            //要做的事情
        }
        if (e && e.keyCode == 113) { // 按 F2 
            //要做的事情
        }
        if (e && e.keyCode == 13) { // enter 键
            //要做的事情
            //获取当前页面
            document.getElementById("btn_search").focus();
            //document.getElementById("btn_search").trigger("click");
        }
    };
});