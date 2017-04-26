var controller = {};

controller.isLoading = true;

controller.isPoweredBy = false;
controller.poweredBy = '';
controller.appointmentInfo = {};
controller.appointmentItemInfoList = [];
controller.appointmentContentInfoList = [];

controller.render = function () {

    utilService.render('controller', controller);

    $('#loading').hide();
    $('#main').show();

};

controller.main = function () {
    service.getAppointmentContentParameter(function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {

            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.appointmentInfo = data.appointmentInfo;
            controller.appointmentItemInfoList = data.appointmentItemInfoList;
            controller.appointmentContentInfoList = data.appointmentContentInfoList;
            controller.render();
        } else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.appointmentInfo.title;
    });
};

controller.main();

controller.showMessage = function (status, message) {
    $.layer({
        area: ['260', '260'],
        title: '预约信息',
        dialog: {
            msg: message,
            btns: 0,
            type: -1,
            btn: ['关闭'],
            yes: function () {

            }
        }
    });
};

controller.getDateTime = function (d) {
    return utilService.getDateTime(d);
};

