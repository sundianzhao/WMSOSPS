var chat;
$(function() {
    chat = $.connection.noticeHub;
    $.initWebSocket();
    chat.client.receiveNotice = function (msg) {
        $.modalMsgF(msg,'success');
    };

    chat.client.receiveMessage = function (message) {
        $.modalMsgF(message, 'success');
    }
});
$.initWebSocket = function () {
    var deferred = $.Deferred();

    if ($.connection.hub.state === 1) {
        deferred.resolve();
        return deferred.promise();
    }

    $.connection.hub.start().done(function () {
        if ($.connection.hub.state === 1) {
            var user = top.clients.user;
            chat.server.join(user.UserCode, user.UserName);
        }
        deferred.resolve();
    }).fail(function () {
        deferred.reject();
    });

    return deferred.promise();
}