var controller = {};

controller.isLoading = true;

controller.messageID = utilService.getUrlVar('messageID');
controller.wxOpenID = utilService.getUrlVar('wxOpenID');

controller.totalMessageNum = 0;
controller.isPoweredBy = false;
controller.poweredBy = '';
controller.messageInfo = {};
controller.contentInfoList = [];
controller.isMore = false;
controller.replyContentID = 0;
controller.startNum = 0;

controller.render = function () {

    utilService.render('controller', controller);

    controller.init();

    $('#loading').hide();
    $('#main').show();

    if (!controller.isMore) $('#more').hide();
};

controller.more = function () {
    $('#moreLoading').show();
    service.more(controller.messageID, controller.startNum, function (data) {
        controller.contentInfoList = data.contentInfoList;
        controller.isMore = data.isMore;
        if (controller.contentInfoList) controller.startNum += controller.contentInfoList.length;
        $('#moreLoading').hide();
        utilService.render('list', controller);
        if (!controller.isMore) $('#more').hide();
    });
};

controller.main = function () {
    service.getMessageParameter(controller.messageID, controller.wxOpenID, function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {

            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.messageInfo = data.messageInfo;
            controller.contentInfoList = data.contentInfoList;
            controller.isMore = data.isMore;
            if (controller.contentInfoList) controller.startNum = controller.contentInfoList.length;

            controller.render();
        } else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.messageInfo.title;
    });
};

controller.main();

controller.submitLike = function (e, contentID) {
    $.layer({
        area: ['auto', 'auto'],
        title: '赞',
        dialog: {
            msg: '写得好，支持作者',
            btns: 1,
            type: 6,
            btn: ['赞一个'],
            yes: function () {
                service.submitLike(controller.messageID, contentID, controller.wxOpenID, function (data) {
                    if (data.isSuccess) {
                        $(e).find('span').html(parseInt($(e).find('span').html(), 10) + 1);
                        layer.msg('提交成功，感谢您的参与！', 2, 1);
                    } else {
                        layer.msg('操作失败，不能重复操作！', 2, 5);
                    }
                });

            }
        }
    });
};

controller.openReply = function (contentID) {
    controller.replyContentID = contentID;
    $('#replyDisplayName').val('');
    $('#replyContent').val('');

    $('#myModal').modal('show');
};

controller.submitReply = function () {
    var displayName = $('#replyDisplayName').val();
    var content = $('#replyContent').val();

    if (!displayName) {
        layer.msg('操作失败，请填写您的称呼', 2, 5);
        $('#replyDisplayName').focus();
        return;
    }
    if (!content) {
        layer.msg('操作失败，请填写留言内容', 2, 5);
        $('#replyContent').focus();
        return;
    }

    $('#myModal').modal('hide');

    $('#loading').show();
    $('#main').hide();

    service.submitReply(controller.messageID, controller.replyContentID, controller.wxOpenID, displayName, content, function (data) {
        if (data.isSuccess) {

            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.messageInfo = data.messageInfo;
            controller.contentInfoList = data.contentInfoList;
            controller.isMore = data.isMore;
            if (controller.contentInfoList) controller.startNum = controller.contentInfoList.length;

            layer.msg('评论成功，感谢您的参与', 2, 1);
        } else {
            notifyService.error(data.errorMessage);
        }
        controller.render();
    });
};

controller.submitContent = function () {

    var displayName = $('#displayName').val();
    var color = $('.color_hight').attr('color');
    var content = $('#content').val();

    if (!displayName) {
        layer.msg('操作失败，请填写您的称呼', 2, 5);
        $('#displayName').focus();
        return;
    }
    if (!content) {
        layer.msg('操作失败，请填写留言内容', 2, 5);
        $('#content').focus();
        return;
    }

    $('#loading').show();
    $('#main').hide();

    service.submitContent(controller.messageID, controller.wxOpenID, displayName, color, content, function (data) {
        if (data.isSuccess) {

            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.messageInfo = data.messageInfo;
            controller.contentInfoList = data.contentInfoList;
            controller.isMore = data.isMore;
            if (controller.contentInfoList) controller.startNum = controller.contentInfoList.length;

            layer.msg('留言成功，感谢您的参与', 2, 1);
        } else {
            notifyService.error(data.errorMessage);
        }
        controller.render();
    });
};

controller.init = function () {

    $(".color").click(function () {
        $(this).addClass('color_hight').siblings().removeClass('color_hight').end();
    });

    $('.iconEmotion').click(function () {
        $('.emotions').toggle();
    });
    $('.eItem').hover(function () {
        $('.emotionsGif img').attr('src', $(this).attr('data-gifurl'));
    });
    $('.eItem').click(function () {
        $('#content').val($('#content').val() + '/' + $(this).attr('data-title'));
        $('.emotions').hide();
    });
};

controller.getDateTime = function (d) {
    return utilService.getDateTime(d);
};