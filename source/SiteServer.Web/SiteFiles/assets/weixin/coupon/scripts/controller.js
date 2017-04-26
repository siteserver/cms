var controller = {};

controller.isLoading = true;
controller.snID = utilService.getUrlVar('snID');
controller.wxOpenID = utilService.getUrlVar('wxOpenID');
controller.keyword = utilService.getUrlVar('keyword');
controller.from = utilService.getUrlVar('from');

controller.data = {};
controller.isPoweredBy = false;
controller.poweredBy = '';

controller.render = function () {
    utilService.render('controller', controller);
    $('#loading').hide();
    $('#main').show();
};

controller.openCheck = function (actID) {
    location.href = 'application.html?actID=' + actID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=';
};
 

controller.main = function () {
    service.getCouponParameter(controller.snID, controller.wxOpenID, function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {
            if (controller.from != "") {
                controller.openCheck(data.conponActInfo.id);
            }
            controller.data = data;
            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;

            controller.render();
        }
        else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.title;
    });

};
controller.main();



function submitAwardCode() {

    var awardCode = $('#awardCode').val();
    var hideErrorMessage = $("#hideErrorMessage").val();

    if (controller.data.awardCode != awardCode) {

        notifyService.error(decodeURIComponent(hideErrorMessage));
        $('#awardCode').val('');
        $('#awardCode').focus();
        return false;
    }

    service.submitAwardCode(controller.snID, function (data) {

        if (data.isSuccess) {
            controller.data.status = 'Cash';
            location.href = location.href;
        } else {
            notifyService.error(data.errorMessage);
        }
    });
};

