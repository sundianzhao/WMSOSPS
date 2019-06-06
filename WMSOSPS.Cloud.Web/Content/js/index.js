var storage, fail, uid; try { uid = new Date; (storage = window.localStorage).setItem(uid, uid); fail = storage.getItem(uid) != uid; storage.removeItem(uid); fail && (storage = false); } catch (e) { }
if (storage) {
    var usedSkin = localStorage.getItem('config-skin');
    if (usedSkin != '' && usedSkin != null) {
        document.body.className = usedSkin;
    }
    else {
        document.body.className = 'theme-blue-gradient';
        localStorage.setItem('config-skin', "theme-blue-gradient");
    }
}
else {
    document.body.className = 'theme-blue';
}
$(function () {
    if (storage) {
        try {
            var usedSkin = localStorage.getItem('config-skin');
            if (usedSkin != '') {
                $('#skin-colors .skin-changer').removeClass('active'); $('#skin-colors .skin-changer[data-skin="' + usedSkin + '"]').addClass('active');
            }
        }
        catch (e) { console.log(e); }
    }
    //==================start 设置左侧菜单栏显示自动滑动的效果=====================
    var sidenav = function () {
        //console.log($(window).height(), $('#header-navbar').height(), $('#user-left-box').height());
        // $('#user-left-box')的高度还要减去padding
        $('.nav-small #sidebar-nav').height($(window).height() - $('#header-navbar').height()).css('overflow', 'auto !important');
        $('#sidebar-nav ul')
            .height($(window).height() - $('#header-navbar').height() - $('#user-left-box').height()-30)
            .css('overflow', 'auto')
            ;
    }
    $(window).on('resize', sidenav);
    sidenav();
    //==================end 设置左侧菜单栏显示自动滑动的效果=====================






})
$.fn.removeClassPrefix = function (prefix) {
    this.each(function (i, el) {
        var classes = el.className.split(" ").filter(function (c) {
            return c.lastIndexOf(prefix, 0) !== 0;
        });
        el.className = classes.join(" ");
    });
    return this;
};
$(function ($) {
    $('#config-tool-cog').on('click', function () { $('#config-tool').toggleClass('closed'); }); $('#config-fixed-header').on('change', function () {
        var fixedHeader = '';
        if ($(this).is(':checked')) {
            $('body').addClass('fixed-header'); fixedHeader = 'fixed-header';
        }
        else {
            $('body').removeClass('fixed-header');
            if ($('#config-fixed-sidebar').is(':checked')) {
                $('#config-fixed-sidebar').prop('checked', false);
                $('#config-fixed-sidebar').trigger('change'); location.reload();
            }
        }
    });
    $('#skin-colors .skin-changer').on('click', function () {
        $('body').removeClassPrefix('theme-');
        $('body').addClass($(this).data('skin'));
        $('#skin-colors .skin-changer').removeClass('active');
        $(this).addClass('active');
        writeStorage(storage, 'config-skin', $(this).data('skin'));
    });
    function writeStorage(storage, key, value) {
        if (storage) {
            try {
                localStorage.setItem(key, value);
            }
            catch (e) { console.log(e); }
        }
    }
});
$(function ($) {
    $("#content-wrapper").find('.mainContent').height($(window).height() - 100);
    $(window).resize(function (e) {
        $("#content-wrapper").find('.mainContent').height($(window).height() - 100);
    });
    $('#sidebar-nav,#nav-col-submenu').on('click', '.dropdown-toggle', function (e) {
        e.preventDefault();
        var $item = $(this).parent();
        if (!$item.hasClass('open')) {
            $item.parent().find('.open .submenu').slideUp('fast');
            $item.parent().find('.open').toggleClass('open');
        }
        $item.toggleClass('open');
        if ($item.hasClass('open')) {
            $item.children('.submenu').slideDown('fast', function () {
                $item.find('ul.submenu').css({
                    height: 'auto'
                })
            });
        }
        else {
            $item.children('.submenu').slideUp('fast');
        }
    });
    $(window).resize(function (e) {
        var openSubmenu = $('#sidebar-nav').find('.open').find('.submenu');
        openSubmenu.css('height', 'auto');
    });
    GetLoadNav();
    $('body').on('mouseenter', '#page-wrapper.nav-small #sidebar-nav .dropdown-toggle', function (e) {
        if ($(document).width() >= 600) {
            var $item = $(this).parent();
            if ($('body').hasClass('fixed-leftmenu')) {
                var topPosition = $item.position().top;

                if ((topPosition + 4 * $(this).outerHeight()) >= $(window).height()) {
                    topPosition -= 6 * $(this).outerHeight();
                }
                $('#nav-col-submenu').html($item.children('.submenu').clone());
                $('#nav-col-submenu > .submenu').css({ 'top': topPosition });
            }

            $item.addClass('open').siblings('li').removeClass('open');
            $item.children('.submenu').slideDown('fast');
            $item.siblings('li').children('.submenu').slideUp('fast');
        }
    });
   // $('body').on('mouseleave', '#page-wrapper.nav-small #sidebar-nav > .nav-pills > li', function (e) {
        $('body').on('mouseleave', '#page-wrapper.nav-small #sidebar-nav ', function (e) {
        if ($(document).width() >= 600) {
            var $item = $(this).children('ul').children('li');
            if ($item.hasClass('open')) {
                $item.find('.open .submenu').slideUp('fast');
                $item.find('.open').removeClass('open');
                $item.children('.submenu').slideUp('fast');
            }
            $item.removeClass('open');
        }
    });
    $('body').on('mouseenter', '#page-wrapper.nav-small #sidebar-nav a:not(.dropdown-toggle)', function (e) {
        if ($('body').hasClass('fixed-leftmenu')) {
            $('#nav-col-submenu').html('');
        }
    });
    $('body').on('mouseleave', '#page-wrapper.nav-small #nav-col', function (e) {
        if ($('body').hasClass('fixed-leftmenu')) {
            $('#nav-col-submenu').html('');
        }
    });
    //切换按钮
    $('body').find('#make-small-nav').click(function (e) {
        $('#page-wrapper').toggleClass('nav-small');
        $('.nav-small #sidebar-nav').height($(window).height() - $('#header-navbar').height()).css('overflow', 'auto');
    });
    $('body').find('.mobile-search').click(function (e) {
        e.preventDefault();
        $('.mobile-search').addClass('active');
        $('.mobile-search form input.form-control').focus();
    });
    $(document).mouseup(function (e) {
        var container = $('.mobile-search');
        if (!container.is(e.target) && container.has(e.target).length === 0) // ... nor a descendant of the container
        {
            container.removeClass('active');
        }
    });
    $(window).load(function () {
        window.setTimeout(function () {
            $('#ajax-loader').fadeOut();
        }, 300);
    });
});
function GetLoadNav() {
    var data = top.clients.authorizeMenu;
    var _html = "";
    $.each(data, function (i) {
        var row = data[i];
        if (row.F_ParentId == "0") {
            _html += '<li>';
            _html += '<a data-id="' + row.F_Id + '" href="#" class="dropdown-toggle"><i class="' + row.F_Icon + '"></i><span>' + row.F_FullName + '</span><i class="fa fa-angle-right drop-icon"></i></a>';
            var childNodes = row.ChildNodes;
            if (childNodes.length > 0) {
                _html += '<ul class="submenu">';
                $.each(childNodes, function (i) {
                    var subrow = childNodes[i];
                    _html += '<li>';
                    if (subrow.F_Target=="blank") {
                        _html += '<a class="menuItem1" data-id="' + subrow.F_Id + '" href="' + subrow.F_UrlAddress + '" data-index="' + subrow.F_SortCode + '" target="' + subrow.F_Target + '">' + subrow.F_FullName + '</a>';
                    }
                    else {
                        _html += '<a class="menuItem" data-id="' + subrow.F_Id + '" href="' + subrow.F_UrlAddress + '" data-index="' + subrow.F_SortCode + '" target="' + subrow.F_Target + '">' + subrow.F_FullName + '</a>';
                    } 
                    _html += '</li>';
                });
                _html += '</ul>';
            }
            _html += '</li>';
        }
    });
    $("#sidebar-nav ul").prepend(_html);
}



