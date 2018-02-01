var controller = {};

controller.isLoading = true;

controller.lotteryID = utilService.getUrlVar('lotteryID');
controller.wxOpenID = utilService.getUrlVar('wxOpenID');

controller.uniqueID = utilService.getUrlVar('uniqueID');
controller.from = utilService.getUrlVar('from');

controller.openLottery = function () {
    location.href = 'yaoyao.html?lotteryID=' + controller.lotteryID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID;
};
controller.openApplication = function () {
    location.href = 'application.html?lotteryID=' + controller.lotteryID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID + '&lotteryType=yaoYao';
};

controller.openCheck = function () {
    location.href = 'check.html?lotteryID=' + controller.lotteryID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID + '&lotteryType=yaoYao';
};

controller.openEnd = function () {
    location.href = 'end.html?lotteryID=' + controller.lotteryID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID;
};

controller.isEnd = false;
controller.isSuccess = false;
controller.errorMessage = '';
controller.isPoweredBy = false;
controller.poweredBy = '';
controller.lotteryInfo = {};
controller.awardInfoList = [];
controller.awardInfo = null;
controller.winnerInfo = null;

controller.success = function () {
    $.layer({
        area: ['auto', 'auto'],
        title: '中奖了',
        dialog: {
            msg: '恭喜你，中奖了！<br />您摇中了' + controller.awardInfo.awardName + '：<br />' + controller.awardInfo.title,
            btns: 1,
            type: 6,
            btn: ['确定'],
            yes: function () {
                controller.openApplication();
            }
        }
    });
};


controller.failure = function () {
    layer.msg('很遗憾，您没有中奖哦！<br/>再来一次！', 2, 5);
};

controller.render = function () {
    utilService.render('controller', controller);

    $(".yao_box").click(function () {
        var _this = $(this);
        $(this).addClass("yao_box_on");
        setTimeout(function () {
            _this.removeClass("yao_box_on");
            controller.submitLottery();
        }, 2000)

    })

    if (window.DeviceMotionEvent) {
        var speed = 25;
        var x = y = z = lastX = lastY = lastZ = 0;
        window.addEventListener('devicemotion', function () {
            var acceleration = event.accelerationIncludingGravity;
            x = acceleration.x;
            y = acceleration.y;
            if (Math.abs(x - lastX) > speed || Math.abs(y - lastY) > speed) {
                $(".yao_box").addClass("yao_box_on");
                setTimeout(function () {
                    $(".yao_box").removeClass("yao_box_on");
                    controller.submitLottery();
                }, 2000)
            }
            lastX = x;
            lastY = y;
        }, false);
    }

    $('#loading').hide();
    $('#main').show();

};

controller.submitLottery = function () {
    service.submitLottery(controller.lotteryID, controller.wxOpenID, function (data) {

        controller.isSuccess = data.isSuccess;
        controller.errorMessage = data.errorMessage;
        controller.awardInfo = data.awardInfo;
        controller.winnerInfo = data.winnerInfo;

        if (controller.isSuccess) {
            if (controller.awardInfo != null && controller.winnerInfo != null) {
                controller.success();
            } else {
                controller.failure();
            }
        }
        else {
            layer.msg(controller.errorMessage, 2, 5);
        }
    })
}

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
    if (!controller.uniqueID) {
        controller.wxOpenID = controller.uniqueID;
    }
    service.getLotteryParameter(controller.lotteryID, controller.wxOpenID, function (data) {
        controller.isLoading = false;

        controller.isEnd = data.isEnd;
        controller.isSuccess = data.isSuccess;
        controller.errorMessage = data.errorMessage;
        controller.isPoweredBy = data.isPoweredBy;
        controller.poweredBy = data.poweredBy;
        controller.lotteryInfo = data.lotteryInfo;
        controller.awardInfoList = data.awardInfoList;
        controller.awardInfo = data.awardInfo;
        controller.winnerInfo = data.winnerInfo;

        if (controller.winnerInfo != null) {
            if (controller.winnerInfo.status == 'Won' && controller.winnerInfo.cashSN.length <= 0) {
                controller.openApplication();
                return;
            }
            if (controller.winnerInfo.status == 'Applied' || controller.winnerInfo.status == 'Cashed') {
                controller.openCheck();
                return;
            }
        }

        if (controller.isEnd == true) {
            location.href = 'end.html?lotteryID=' + controller.lotteryID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID;
        }
        else {
            controller.render();
        }
        document.title = data.lotteryInfo.title;
    });
};


controller.main();
