var controller = {};

controller.isLoading = true;
controller.lotteryType = utilService.getUrlVar('lotteryType');
controller.lotteryID = utilService.getUrlVar('lotteryID');
controller.wxOpenID = utilService.getUrlVar('wxOpenID');

controller.openLottery = function (lotteryType) {
    location.href = lotteryType + '.html?lotteryID=' + controller.lotteryID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID + '&lotteryType=' + lotteryType;
};

controller.openCheck = function (lotteryType) {
    location.href = 'check.html?lotteryID=' + controller.lotteryID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID + '&lotteryType=' + lotteryType;
};

controller.render = function () {
    utilService.render('controller', controller);

    $('#loading').hide();
    $('#main').show();
};

controller.isSuccess = false;
controller.errorMessage = '';
controller.isPoweredBy = false;
controller.poweredBy = '';
controller.lotteryInfo = {};
controller.awardInfoList = [];
controller.awardInfo = null;
controller.winnerInfo = null;

controller.submitApplication = function () {

    var realName = $('#realName').val();
    var mobile = $('#mobile').val();
    var email = $('#email').val();
    var address = $('#address').val();

    $('#myForm').validator('validate');

    if ($("#myForm").children().hasClass("has-error")) {
        return false;
    }

    $('#loading').show();
    $('#main').hide();

    service.submitApplication(controller.lotteryID, controller.winnerInfo.id, realName, mobile, email, address, function (data) {
        if (data.isSuccess) {
            controller.openCheck(controller.lotteryType);
        }
        else {
            notifyService.error(data.errorMessage);
        }
    });
};

controller.main = function () {
    service.getLotteryParameter(controller.lotteryID, controller.wxOpenID, function (data) {

        controller.isSuccess = data.isSuccess;
        controller.errorMessage = data.errorMessage;
        controller.isPoweredBy = data.isPoweredBy;
        controller.poweredBy = data.poweredBy;
        controller.lotteryInfo = data.lotteryInfo;
        controller.awardInfoList = data.awardInfoList;
        controller.awardInfo = data.awardInfo;
        controller.winnerInfo = data.winnerInfo;
        if (!controller.awardInfo) {
            controller.openLottery(controller.lotteryInfo.lotteryType);
        } else if (!controller.lotteryInfo.isFormRealName && !controller.lotteryInfo.isFormMobile && !controller.lotteryInfo.isFormEmail && !controller.lotteryInfo.isFormAddress) {
            controller.openCheck(controller.lotteryInfo.lotteryType);
        } else {
            controller.render();
        }
        document.title = data.lotteryInfo.title;
    });
};

controller.main();