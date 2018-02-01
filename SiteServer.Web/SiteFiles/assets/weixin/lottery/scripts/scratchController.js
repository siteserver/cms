var controller = {};

controller.isLoading = true;

controller.lotteryID = utilService.getUrlVar('lotteryID');
controller.wxOpenID = utilService.getUrlVar('wxOpenID');
controller.uniqueID = utilService.getUrlVar('uniqueID');
controller.from = utilService.getUrlVar('from');

controller.openLottery = function () {
    location.href = 'scratch.html?lotteryID=' + controller.lotteryID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID;
};
controller.openApplication = function () {
    location.href = 'application.html?lotteryID=' + controller.lotteryID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID + '&lotteryType=scratch';
};

controller.openCheck = function () {
    location.href = 'check.html?lotteryID=' + controller.lotteryID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID + '&lotteryType=scratch';
};

controller.openEnd = function () {
    location.href = 'end.html?lotteryID=' + controller.lotteryID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID;
};

controller.isSuccess = false;
controller.errorMessage = '';
controller.isPoweredBy = false;
controller.poweredBy = '';
controller.lotteryInfo = {};
controller.awardInfoList = [];
controller.awardInfo = null;
controller.winnerInfo = null;

controller.render = function () {
    utilService.render('controller', controller);
    //alert(controller.winnerInfo);
    if (controller.isSuccess) {
        $('.ggk').wScratchPad({
            bg: controller.winnerInfo ? 'imgScratch/bg-success.png' : 'imgScratch/bg-failure.png',
            fg: 'imgScratch/fg.png',
            'cursor': 'url("imgScratch/coin.png") 5 5, default',
            scratchMove: function (e, percent) {
                if (controller.winnerInfo && percent > 20.0) {
                    if (controller.awardInfo != null && controller.winnerInfo != null) {
                        $('.ggk').wScratchPad('clear');
                        $.layer({
                            area: ['auto', 'auto'],
                            title: '中奖了',
                            dialog: {
                                msg: '恭喜你，中奖了！<br />您抽中了' + controller.awardInfo.awardName + '：<br />' + controller.awardInfo.title,
                                btns: 1,
                                type: 6,
                                btn: ['确定'],
                                yes: function () {
                                    controller.openApplication();
                                }
                            }
                        });
                    }
                }
            }
        });
    } else {
        $('.ggk').hide();
        layer.msg(controller.errorMessage, 2, 5);
    }

    $('#loading').hide();
    $('#main').show();
};

function isPc() {
    var userAgentInfo = navigator.userAgent;
    var Agents = ["Android", "iPhone", "SymbianOS", "Windows Phone", "iPad", "iPod"];
    var flag = true;
    for (var v = 0; v < Agents.length; v++) {
        if (userAgentInfo.indexOf(Agents[v]) > 0) {
            flag = false;
            break;
        }
    }
    return flag;
}

controller.main = function () {
    if (controller.from != "") {
        controller.openLottery();
    }
    //if (controller.uniqueID == "" && !isPc()) {
    //    controller.Auth();
    //    return;
    //}
    //else {
    //    controller.wxOpenID = controller.uniqueID;
    //}
    if (!controller.uniqueID) {
        controller.wxOpenID = controller.uniqueID;
    }
    service.getLotteryParameter(controller.lotteryID, controller.wxOpenID, function (data) {
        controller.isLoading = false;

        if (data.isEnd) {
            controller.openEnd();
        }
        else {

            controller.isSuccess = data.isSuccess;
            controller.errorMessage = data.errorMessage;
            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.lotteryInfo = data.lotteryInfo;
            controller.awardInfoList = data.awardInfoList;
            controller.awardInfo = data.awardInfo;
            controller.winnerInfo = data.winnerInfo;

            if (controller.winnerInfo != null) {
                if (controller.winnerInfo.status == 'Applied' || controller.winnerInfo.status == 'Cashed') {
                    controller.openCheck();
                    return;
                }
            }

            controller.render();

        }
        document.title = data.lotteryInfo.title;

    });
};

controller.main();