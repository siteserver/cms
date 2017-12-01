var controller = {};

controller.isLoading = true;

controller.lotteryID = utilService.getUrlVar('lotteryID');
controller.wxOpenID = utilService.getUrlVar('wxOpenID');

controller.isPoweredBy = false;
controller.poweredBy = '';
controller.lotteryInfo = {};
controller.awardInfoList = [];
controller.awardInfo = null;

controller.render = function () {
    utilService.render('controller', controller);
    $('#loading').hide();
    $('#main').show();
};

controller.main = function () {

    service.getLotteryParameter(controller.lotteryID, controller.wxOpenID, function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {

            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.lotteryInfo = data.lotteryInfo;
            controller.awardInfoList = data.awardInfoList;
            controller.awardInfo = data.awardInfo;

            controller.render();

        } else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.lotteryInfo.title;
    });

};

controller.main();