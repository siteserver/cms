var controller = {};

controller.isLoading = true;

controller.searchID = utilService.getUrlVar('searchID');
controller.keywords = utilService.getUrlVar('keywords');

controller.isPoweredBy = false;
controller.poweredBy = '';
controller.searchInfo = {};
controller.searchNavigationInfoList = [];
controller.contentInfoList = [];

controller.idCollection = '';

controller.render = function () {
    utilService.render('controller', controller);
    $(".btn-toolbar .btn").click(function () {
        $("#searchType").val($(this).attr("id"));
        $(this).addClass("active").siblings().removeClass("active");
    });
    $("#keywords").val(controller.keywords);
    $('#loading').hide();
    $('#main').show();
};

controller.openSearch = function () {
    var keywords = $("#keywords").val();
    if (!keywords) {
        notifyService.error('请输入搜索关键字');
        return;
    }
    $('#loading').show();
    $('#main').hide();
    var searchType = $("#searchType").val();
    if (searchType == "site") {
        location.href = 'search.html?publishmentSystemID=' + service.publishmentSystemID + '&searchID=' + controller.searchID + '&keywords=' + keywords;
    }
    else if (searchType == "baidu") {
        location.href = 'http://www.baidu.com/fro=844b/s?word=' + keywords + '&st=11104i&ts=1706443&sa=is_1&ms=1&SS=10&rq=JA';

    }
    else if (searchType == "sogou") {
        location.href = 'http://weixin.sogou.com/weixinwap?ie=utf8&query=' + keywords + '&type=1';
    }
};

controller.more = function () {
    for (var i = 0; i < controller.contentInfoList.length; i++) {
        if (controller.idCollection) {
            controller.idCollection += ',' + controller.contentInfoList[i].id;
        } else {
            controller.idCollection = '' + controller.contentInfoList[i].id;
        }
    }
    $('#loading').show();
    $('#main').hide();
    service.getMoreResultParameter(controller.searchID, controller.keywords, controller.idCollection, function (data) {

        if (data.isSuccess) {
            controller.contentInfoList = data.contentInfoList;
            for (var i = 0; i < controller.contentInfoList.length; i++) {
                var contentInfo = controller.contentInfoList[i];
                contentInfo.title = contentInfo.title.replace(new RegExp(controller.keywords, 'g'), '<span class="red">' + controller.keywords + '</span>');
                contentInfo.summary = contentInfo.summary.replace(new RegExp(controller.keywords, 'g'), '<span class="red">' + controller.keywords + '</span>');
            }
            controller.render();

        } else {
            notifyService.error(data.errorMessage);
        }
    });
};

controller.main = function () {
    service.getSearchResultParameter(controller.searchID, controller.keywords, function (data) {
        controller.isLoading = false;

        if (data.isSuccess) {
            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.searchInfo = data.searchInfo;
            controller.searchNavigationInfoList = data.searchNavigationInfoList;
            controller.contentInfoList = data.contentInfoList;
            for (var i = 0; i < controller.contentInfoList.length; i++) {
                var contentInfo = controller.contentInfoList[i];
                contentInfo.title = contentInfo.title.replace(new RegExp(controller.keywords, 'g'), '<span class="red">' + controller.keywords + '</span>');
                contentInfo.summary = contentInfo.summary.replace(new RegExp(controller.keywords, 'g'), '<span class="red">' + controller.keywords + '</span>');
            }
            controller.render();

        } else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.searchInfo.title;
    });
};

controller.main();

controller.getDate = function (d) {
    return utilService.getDate(d);
};