/// <reference path="../SCode/jquery-1.4.2.min-vsdoc.js" />
 
function PictureView(data, First) {
    $(document).ready(function () {
        $(document).bind("contextmenu", function (e) {
            return false;
        });
    });
    var src = "";
    var imgIndex = 0;
    var index = 100000000;
    if (First) {
        src = 'src="' + First + '"';
    }
    if (typeof data == "object" && First) {
        for (var i = 0; i < data.length; i++) {
            if (data[i] == First) {
                imgIndex = i;
                break;
            }
        }
    }
    var parentView = $('<div id="parent"/>').appendTo("body");
    var PictureViewMask = $('<div class="PictureViewMask"></div>').appendTo(parentView);
    var PictureViewNext = $('<div title="下一张" class="PictureViewNext"/>').appendTo(parentView);
    var PictureViewPrev = $('<div title="上一张" class="PictureViewPrev"/>').appendTo(parentView);
    var PictureViewClose = $('<div  title="关闭" class="PictureViewClose"/>').appendTo(parentView);
    var PictureViewRaised = $('<div class="PictureViewRaised"><div class="PictureViewBoxcontent"><div  title="向右旋转" class="PictureViewClockwise"/><div title="向左旋转" class="PictureViewAnticlockwise"  /></div><b class="b4b"></b><b class="b3b"></b><b class="b2b"></b><b class="b-1b"></b><b class="b1b"></b></div>').appendTo("body");
    var ViewWindow = $('<div class="ViewWindow" />').appendTo(parentView);
    var PictureViewImg = $('<img title="左右上下拖动"   class="PictureViewImg" alt="" imgIndex="' + imgIndex + '" scale="100" angle="0"/>').appendTo(ViewWindow);
    var PictureViewClockwise = PictureViewRaised.find(".PictureViewClockwise");
    var PictureViewAnticlockwise = PictureViewRaised.find(".PictureViewAnticlockwise");
    index++;
    PictureViewMask.css("z-index", index);
    index++;
    ViewWindow.css("z-index", index);
    index++;
    PictureViewNext.css("z-index", index);
    PictureViewPrev.css("z-index", index);
    PictureViewClose.css("z-index", index);
    PictureViewRaised.css("z-index", index);
    addEvent(PictureViewImg[0], PVscrollFunc);
    PictureViewImg[0].onload = function () {
        dd.call(this);
    };
    PictureViewImg[0].onreadystatechange = function () {
        if (this.readyState == "complete" || this.readyState == "loaded") {
            dd.call(this);
        }
    };
    PictureViewImg[0].src = First;
    var tt;
    function dd() {
        if (this.offsetHeight > 0) {
            var windowbl = $(window).height() / $(window).width();
            var imgbl = this.height / this.width;
            if (imgbl > windowbl) {
                var scale = parseInt($(window).height() * 0.8 / this.height * 100) || 100;
                fnRotateScale(this, 0, scale / 100, scale / 100);
                PictureViewImg.attr("scale", scale);
            } else {
                var scale = parseInt($(window).width() * 0.8 / this.width * 100) || 100;
                fnRotateScale(this, 0, scale / 100, scale / 100);
                PictureViewImg.attr("scale", scale);
            }
            PictureViewImg.css({ top: ($(window).height() - this.offsetHeight) / 2, left: ($(window).width() - this.offsetWidth) / 2 });
            clearTimeout(tt);
            return;
        }
        tt = setTimeout(dd, 1);

    }
    PictureViewNext.click(function () {
        ViewNext.call(this, data, PictureViewImg);
    });
    PictureViewPrev.click(function () {
        ViewPrev.call(this, data, PictureViewImg);
    });
    PictureViewClockwise.click(function () {
        var angle = parseInt(PictureViewImg.attr("angle")) || 0;
        var scale = parseInt(PictureViewImg.attr("scale")) || 100;
        angle += 90;
        PictureViewImg.attr("angle", angle);
        fnRotateScale(PictureViewImg[0], angle, scale / 100, scale / 100);
        PictureViewImg.css({ top: ($(window).height() - PictureViewImg.height()) / 2, left: ($(window).width() - PictureViewImg.width()) / 2 });
    });
    PictureViewAnticlockwise.click(function () {
        var angle = parseInt(PictureViewImg.attr("angle")) || 0;
        var scale = parseInt(PictureViewImg.attr("scale")) || 100;
        angle -= 90;
        PictureViewImg.attr("angle", angle);
        fnRotateScale(PictureViewImg[0], angle, scale / 100, scale / 100);
        PictureViewImg.css({ top: ($(window).height() - PictureViewImg.height()) / 2, left: ($(window).width() - PictureViewImg.width()) / 2 });
    });
    $(document).mousemove(function (e) {
        if (e.clientY <= 50)
            PictureViewRaised.slideDown("fast");
        else {
            if (PictureViewRaised.css("display") == "block")
                PictureViewRaised.slideUp("fast");
            return false;
        }
    });

    var drag = new Drag(PictureViewImg[0]);


    PictureViewClose.click(function () {
        PictureViewMask.remove();
        ViewWindow.remove();
        PictureViewNext.remove();
        PictureViewPrev.remove();
        PictureViewRaised.remove();
        $(document).unbind("mousemove");
        PictureViewClose.remove();
    });


    function addEvent(el, fn) {
        if (el.addEventListener)
            el.addEventListener("DOMMouseScroll", fn, false); //Firefox
        el.onmousewheel = fn; //IE/Opera/Chrome
    }

    function PVscrollFunc(e) {
        var delta;
        var obj = $(this);
        var scale = obj.attr('scale');
        var zoom = parseInt(scale, 10) || 100;
        e = fixEvent(e); //e || window.event; //
        if (e.wheelDelta) {//IE/Opera/Chrome
            delta = e.wheelDelta;
            zoom += delta / 12;
        } else if (e.detail) {//Firefox
            delta = e.detail;
            zoom += (-delta) / 0.3;
        }
        if (zoom > 0) {
            //var hei = scale ? (obj.height() / ((parseInt(scale, 10) / 100))) : obj.height();
            //var wid = scale ? (obj.width() / ((parseInt(scale, 10) / 100))) : obj.width();
            //obj.css("height", (hei * (zoom / 100)));
            //obj.css("width", (wid * (zoom / 100)));
            var angle = obj.attr("angle");
            fnRotateScale(this, angle, zoom / 100, zoom / 100);
            obj.attr('scale', zoom);
            //obj.css({ top: ($(window).height() - obj.height()) / 2, left: ($(window).width() - obj.width()) / 2 })
            if ((parseInt(obj.css("top").replace("px", ""), 10) + obj.height()) < 0 || (parseInt(obj.css("left").replace("px", ""), 10) + obj.width()) < 0)
                obj.css({ top: 0, left: 0 });
        }
        e.preventDefault();
    };

    function ViewNext(data, Img) {
        var index = Img.attr("imgIndex");
        if (index == (data.length - 1)) index = -1;
        index++;
        Img.attr("src", data[index]);
        Img.attr("imgIndex", index);
        Img.attr("angle", 0);
    }
    function ViewPrev(data, Img) {
        var index = Img.attr("imgIndex");
        if (index == 0) index = data.length;
        index--;
        Img.attr("src", data[index]);
        Img.attr("imgIndex", index);
        Img.attr("angle", 0);
    }

    function fnRotateScale(dom, angle, scale_x, scale_y) {
        angle = parseFloat(angle) || 0;
        scale_x = parseFloat(scale_x) || 1;
        scale_y = parseFloat(scale_y) || 1;
        if (typeof (angle) === "number") {
            //IE
            var radian = angle * (Math.PI / 180);
            var x, y;
            x = scale_x;
            y = scale_y;
            if ("filters" in document.createElement("img")) {
                dom.style.filter = "progid:DXImageTransform.Microsoft.Matrix(SizingMethod='auto expand')";
                dom.onload = null; //防止ie重复加载gif的bug
                dom.style.visibility = "visible";
                $.extend(dom.filters.item("DXImageTransform.Microsoft.Matrix"), getMatrix(radian, y, x));
                return;
            }
            var css3Transform;
            var ary = ["transform", "MozTransform", "webkitTransform", "OTransform"];
            for (var i = 0; i < ary.length; i++) {
                if (ary[i] in dom.style) {
                    css3Transform = ary[i];
                    break;
                }
            }
            if (!css3Transform) return;
            var matrix = getMatrix(radian, y, x);
            dom.style[css3Transform] = "matrix("
					+ matrix.M11.toFixed(16) + "," + matrix.M21.toFixed(16) + ","
					+ matrix.M12.toFixed(16) + "," + matrix.M22.toFixed(16) + ", 0, 0)";
        }
        //获取变换参数函数
        function getMatrix(radian, x, y) {
            var Cos = Math.cos(radian), Sin = Math.sin(radian);
            return {
                M11: Cos * x, M12: -Sin * y,
                M21: Sin * x, M22: Cos * y
            };
        }
    }

    var D = {
        getScrollTop: function (node) {
            var doc = node ? node.ownerDocument : document;
            return doc.documentElement.scrollTop || doc.body.scrollTop;
        },
        getScrollLeft: function (node) {
            var doc = node ? node.ownerDocument : document;
            return doc.documentElement.scrollLeft || doc.body.scrollLeft;
        }
    };
    function fixEvent(event) {
        if (event) return event;
        event = window.event;
        event.pageX = event.clientX + D.getScrollLeft(event.srcElement);
        event.pageY = event.clientY + D.getScrollTop(event.srcElement);
        event.target = event.srcElement;
        event.stopPropagation = stopPropagation;
        event.preventDefault = preventDefault;
        var relatedTarget = {
            "mouseout": event.toElement,
            "mouseover": event.fromElement
        }[event.type];
        if (relatedTarget) {
            event.relatedTarget = relatedTarget;
        }
        return event;
    };
    function stopPropagation() {
        this.cancelBubble = true;
    };
    function preventDefault() {
        this.returnValue = false;
    }; 
}


//#region
var isIE = (document.all) ? true : false;

var $$ = function (id) {
    return "string" == typeof id ? document.getElementById(id) : id;
};

var Class = {
    create: function () {
        return function () { this.initialize.apply(this, arguments); }
    }
};

var Extend = function (destination, source) {
    for (var property in source) {
        destination[property] = source[property];
    }
};

var Bind = function (object, fun) {
    return function () {
        return fun.apply(object, arguments);
    }
};

var BindAsEventListener = function (object, fun) {
    return function (event) {
        return fun.call(object, (event || window.event));
    }
};

var CurrentStyle = function (element) {
    return element.currentStyle || document.defaultView.getComputedStyle(element, null);
};

function addEventHandler(oTarget, sEventType, fnHandler) {
    if (oTarget.addEventListener) {
        oTarget.addEventListener(sEventType, fnHandler, false);
    } else if (oTarget.attachEvent) {
        oTarget.attachEvent("on" + sEventType, fnHandler);
    } else {
        oTarget["on" + sEventType] = fnHandler;
    }
};

function removeEventHandler(oTarget, sEventType, fnHandler) {
    if (oTarget.removeEventListener) {
        oTarget.removeEventListener(sEventType, fnHandler, false);
    } else if (oTarget.detachEvent) {
        oTarget.detachEvent("on" + sEventType, fnHandler);
    } else {
        oTarget["on" + sEventType] = null;
    }
};

//拖放程序
var Drag = Class.create();
Drag.prototype = {
    //拖放对象
    initialize: function (drag, options) {
        this.Drag = typeof drag == "string" ? $$(drag) : drag; //拖放对象
        this._x = this._y = 0; //记录鼠标相对拖放对象的位置
        this._marginLeft = this._marginTop = 0; //记录margin
        //事件对象(用于绑定移除事件)
        this._fM = BindAsEventListener(this, this.Move);
        this._fS = Bind(this, this.Stop);

        this.SetOptions(options);

        this.Limit = !!this.options.Limit;
        this.mxLeft = parseInt(this.options.mxLeft);
        this.mxRight = parseInt(this.options.mxRight);
        this.mxTop = parseInt(this.options.mxTop);
        this.mxBottom = parseInt(this.options.mxBottom);

        this.LockX = !!this.options.LockX;
        this.LockY = !!this.options.LockY;
        this.Lock = !!this.options.Lock;

        this.onStart = this.options.onStart;
        this.onMove = this.options.onMove;
        this.onStop = this.options.onStop;

        this._Handle = $$(this.options.Handle) || this.Drag;
        this._mxContainer = $$(this.options.mxContainer) || null;

        this.Drag.style.position = "absolute";
        //透明
        if (isIE && !!this.options.Transparent) {
            //填充拖放对象
            with (this._Handle.appendChild(document.createElement("div")).style) {
                width = height = "100%"; backgroundColor = "#fff"; filter = "alpha(opacity:0)"; fontSize = 0;
            }
        }
        //修正范围
        this.Repair();
        addEventHandler(this._Handle, "mousedown", BindAsEventListener(this, this.Start));
    },
    //设置默认属性
    SetOptions: function (options) {
        this.options = {//默认值
            Handle: "", //设置触发对象（不设置则使用拖放对象）
            Limit: false, //是否设置范围限制(为true时下面参数有用,可以是负数)
            mxLeft: 0, //左边限制
            mxRight: 9999, //右边限制
            mxTop: 0, //上边限制
            mxBottom: 9999, //下边限制
            mxContainer: "", //指定限制在容器内
            LockX: false, //是否锁定水平方向拖放
            LockY: false, //是否锁定垂直方向拖放
            Lock: false, //是否锁定
            Transparent: false, //是否透明
            onStart: function () { }, //开始移动时执行
            onMove: function () { }, //移动时执行
            onStop: function () { } //结束移动时执行
        };
        Extend(this.options, options || {});
    },
    //准备拖动
    Start: function (oEvent) {
        if (this.Lock) { return; }
        this.Repair();
        //记录鼠标相对拖放对象的位置
        this._x = oEvent.clientX - this.Drag.offsetLeft;
        this._y = oEvent.clientY - this.Drag.offsetTop;
        //记录margin
        this._marginLeft = parseInt(CurrentStyle(this.Drag).marginLeft) || 0;
        this._marginTop = parseInt(CurrentStyle(this.Drag).marginTop) || 0;
        //mousemove时移动 mouseup时停止
        addEventHandler(document, "mousemove", this._fM);
        addEventHandler(document, "mouseup", this._fS);
        if (isIE) {
            //焦点丢失
            addEventHandler(this._Handle, "losecapture", this._fS);
            //设置鼠标捕获
            this._Handle.setCapture();
        } else {
            //焦点丢失
            addEventHandler(window, "blur", this._fS);
            //阻止默认动作
            oEvent.preventDefault();
        };
        //附加程序
        this.onStart();
    },
    //修正范围
    Repair: function () {
        if (this.Limit) {
            //修正错误范围参数
            this.mxRight = Math.max(this.mxRight, this.mxLeft + this.Drag.offsetWidth);
            this.mxBottom = Math.max(this.mxBottom, this.mxTop + this.Drag.offsetHeight);
            //如果有容器必须设置position为relative或absolute来相对或绝对定位，并在获取offset之前设置
            !this._mxContainer || CurrentStyle(this._mxContainer).position == "relative" || CurrentStyle(this._mxContainer).position == "absolute" || (this._mxContainer.style.position = "relative");
        }
    },
    //拖动
    Move: function (oEvent) {
        //判断是否锁定
        if (this.Lock) { this.Stop(); return; };
        //清除选择
        window.getSelection ? window.getSelection().removeAllRanges() : document.selection.empty();
        //设置移动参数
        var iLeft = oEvent.clientX - this._x, iTop = oEvent.clientY - this._y;
        //设置范围限制
        if (this.Limit) {
            //设置范围参数
            var mxLeft = this.mxLeft, mxRight = this.mxRight, mxTop = this.mxTop, mxBottom = this.mxBottom;
            //如果设置了容器，再修正范围参数
            if (!!this._mxContainer) {
                mxLeft = Math.max(mxLeft, 0);
                mxTop = Math.max(mxTop, 0);
                mxRight = Math.min(mxRight, this._mxContainer.clientWidth);
                mxBottom = Math.min(mxBottom, this._mxContainer.clientHeight);
            };
            //修正移动参数
            iLeft = Math.max(Math.min(iLeft, mxRight - this.Drag.offsetWidth), mxLeft);
            iTop = Math.max(Math.min(iTop, mxBottom - this.Drag.offsetHeight), mxTop);
        }
        //设置位置，并修正margin
        if (!this.LockX) { this.Drag.style.left = iLeft - this._marginLeft + "px"; }
        if (!this.LockY) { this.Drag.style.top = iTop - this._marginTop + "px"; }
        //附加程序
        this.onMove();
    },
    //停止拖动
    Stop: function () {
        //移除事件
        removeEventHandler(document, "mousemove", this._fM);
        removeEventHandler(document, "mouseup", this._fS);
        if (isIE) {
            removeEventHandler(this._Handle, "losecapture", this._fS);
            this._Handle.releaseCapture();
        } else {
            removeEventHandler(window, "blur", this._fS);
        };
        //附加程序
        this.onStop();
    }
};
//#endregion
