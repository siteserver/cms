var controller = {};

controller.isLoading = true;

controller.searchID = utilService.getUrlVar('searchID');

controller.isPoweredBy = false;
controller.poweredBy = '';
controller.searchInfo = {};
controller.searchNavigationInfoList = [];
controller.imageContentInfoList = [];
controller.textContentInfoList = [];

controller.imageIDCollection = '';
controller.textIDCollection = '';

controller.render = function () {
    utilService.render('controller', controller);
    $(".btn-toolbar .btn").click(function () {
        $("#searchType").val($(this).attr("id"));
        $(this).addClass("active").siblings().removeClass("active");
    });
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

controller.main = function () {
    service.getSearchParameter(controller.searchID, function (data) {
        controller.isLoading = false;

        if (data.isSuccess) {
            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.searchInfo = data.searchInfo;
            controller.searchNavigationInfoList = data.searchNavigationInfoList;
            controller.imageContentInfoList = data.imageContentInfoList;
            controller.textContentInfoList = data.textContentInfoList;

            controller.render();

        } else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.searchInfo.title;
    });
};

controller.loadImageContentMore = function () {
    for (var i = 0; i < controller.imageContentInfoList.length; i++) {
        if (controller.imageIDCollection) {
            controller.imageIDCollection += ',' + controller.imageContentInfoList[i].id;
        } else {
            controller.imageIDCollection = '' + controller.imageContentInfoList[i].id;
        }
    }
    $('#loading').show();
    $('#main').hide();
    service.getRefreshParameter(controller.searchID, controller.imageIDCollection, '', function (data) {
        if (data.isSuccess) {
            controller.imageContentInfoList = data.imageContentInfoList;
            controller.render();
        }
    });
}

controller.loadTextContentMore = function () {
    for (var i = 0; i < controller.textContentInfoList.length; i++) {
        if (controller.textIDCollection) {
            controller.textIDCollection += ',' + controller.textContentInfoList[i].id;
        } else {
            controller.textIDCollection = '' + controller.textContentInfoList[i].id;
        }
    }
    $('#loading').show();
    $('#main').hide();
    service.getRefreshParameter(controller.searchID, '', controller.textIDCollection, function (data) {
        if (data.isSuccess) {
            controller.textContentInfoList = data.textContentInfoList;
            controller.render();
        }
    });
}

controller.main();