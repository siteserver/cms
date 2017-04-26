var controller = {};

controller.isLoading = true;

controller.lotteryID = utilService.getUrlVar('lotteryID');
controller.wxOpenID = utilService.getUrlVar('wxOpenID');
controller.lotteryType = utilService.getUrlVar('lotteryType');
controller.from = utilService.getUrlVar('from');

controller.isSuccess = false;
controller.errorMessage = '';
controller.isPoweredBy = false;
controller.poweredBy = '';
controller.lotteryInfo = {};
controller.awardInfoList = [];
controller.awardInfo = null;
controller.winnerInfo = null;

controller.openLottery = function () { 
    location.href = controller.lotteryType + '.html?lotteryID=' + controller.lotteryID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID;
};


controller.render = function () {
    utilService.render('controller', controller);

    $('#qrCode').qrcode({ width: 90, height: 90, text: controller.winnerInfo.cashSN });

    $('#loading').hide();
    $('#main').show();
};

controller.main = function () {
    service.getLotteryParameter(controller.lotteryID, controller.wxOpenID, function (data) {
        if (controller.from != "") {
            location.href = location.href;
        }
        controller.isLoading = false;
        if (data.isSuccess) {

            controller.isSuccess = data.isSuccess;
            controller.errorMessage = data.errorMessage;
            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.lotteryInfo = data.lotteryInfo;
            controller.awardInfoList = data.awardInfoList;
            controller.awardInfo = data.awardInfo;
            controller.winnerInfo = data.winnerInfo;

            controller.render();
        }
        document.title = data.lotteryInfo.title;
    });
};

controller.main();

controller.submitAwardCode = function () {

    var awardCode = $('#awardCode').val();

    if (controller.lotteryInfo.awardCode != awardCode) {
        notifyService.error('对不起，您输入的兑奖密码不正确，请重新输入！');
        $('#awardCode').val('');
        $('#awardCode').focus();
    } else {

        $('#loading').show();
        $('#main').hide();

        service.submitAwardCode(controller.lotteryID, controller.winnerInfo.id, function (data) {
            if (data.isSuccess) {

                controller.winnerInfo.status = 'Cashed';

                controller.render();

            } else {
                notifyService.error(data.errorMessage);
            }
        });

    }

};