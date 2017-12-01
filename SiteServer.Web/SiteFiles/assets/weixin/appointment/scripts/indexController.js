var controller = {};
controller.appointmentID = utilService.getUrlVar('appointmentID');
controller.isLoading = true;

controller.isPoweredBy = false;
controller.poweredBy = '';
controller.appointmentInfo = {};
controller.appointmentItemInfo = {};
controller.appointmentContentInfo = {};
controller.appointmentItemInfoList = [];
controller.appointmentContentInfoList = [];
controller.isEnd = false;

controller.render = function () {
    utilService.render('controller', controller);

    $('#loading').hide();
    $('#main').show();
};

controller.getItemUrl = function (itemID) {
    return 'item.html?publishmentSystemID=' + service.publishmentSystemID + '&appointmentID=' + service.appointmentID + '&itemID=' + itemID + '&wxOpenID=' + service.wxOpenID;
};

controller.main = function () {
    service.getAppointmentParameter(controller.appointmentID, function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {

            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.appointmentInfo = data.appointmentInfo;
            controller.appointmentItemInfoList = data.appointmentItemInfoList;
            controller.isEnd = data.isEnd;

            if (controller.isEnd) {
                service.openEnd();
            }
            else {
                controller.render();
            }

        } else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.appointmentInfo.title;
    });
};

controller.main();