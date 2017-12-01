var controller = {};

controller.isLoading = true;

controller.lotteryID = utilService.getUrlVar('lotteryID');
controller.wxOpenID = utilService.getUrlVar('wxOpenID');
controller.uniqueID = utilService.getUrlVar('uniqueID');
controller.from = utilService.getUrlVar('from');

controller.openLottery = function () {
    location.href = 'goldEgg.html?lotteryID=' + controller.lotteryID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID;
};

controller.openApplication = function () {
    location.href = 'application.html?lotteryID=' + controller.lotteryID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID + '&lotteryType=goldEgg';
};

controller.openCheck = function () {
    location.href = 'check.html?lotteryID=' + controller.lotteryID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID + '&lotteryType=goldEgg';
};

controller.openEnd = function () {
    location.href = 'end.html?lotteryID=' + controller.lotteryID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID;
};

controller.isEnd = false;
controller.isSuccess = false;
controller.isPoweredBy = false;
controller.poweredBy = '';
controller.errorMessage = '';
controller.lotteryInfo = {};
controller.awardInfoList = [];
controller.awardInfo = null;
controller.winnerInfo = null;

controller.success = function () {
    $.layer({
        area: ['auto', 'auto'],
        title: '中奖了',
        dialog: {
            msg: '恭喜你，中奖了！<br />您砸中了' + controller.awardInfo.awardName + '：<br />' + controller.awardInfo.title,
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
    layer.msg('很遗憾，您没有中奖哦！<br/>再来一次！！', 2, 5);
};

controller.render = function () {
    utilService.render('controller', controller);

    $('.egg_btn').click(function (e) {
        var btn = $(this).attr("id");
        $(".cuizi").css("left", e.pageX - 5);
        $(".cuizi").css("top", e.pageY - $(".cuizi").height() * 1.5);
        $(".cuizi").show().addClass("cuizi_on");
        setTimeout(function () {
            $(".egg_box_inner img").hide();
            if (btn == "egg1_btn") {
                $(".egg_box_inner .img1").fadeIn(500);
            }
            else if (btn == "egg2_btn") {
                $(".egg_box_inner .img2").fadeIn(500);
            }
            else if (btn == "egg3_btn") {
                $(".egg_box_inner .img3").fadeIn(500);
            }
            $(".cuizi").show().removeClass("cuizi_on");

            controller.submitLottery();
        }, 1000)

    })

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
//controller.Auth = function () {
//    var loginType = "WeixinMob";
//    var returnUrl = location.href;
//    var locationUrl = "http://gexia.com/home/authlogin.html?isWeixin=true&loginType=" + loginType + "&returnUrl=" + returnUrl;
//    location.href = locationUrl;
//};

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
