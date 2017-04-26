var controller = {};

controller.isLoading = true;

controller.isPoweredBy = false;
controller.poweredBy = '';
controller.appointmentInfo = {};
controller.appointmentItemInfo = {};
  
controller.render = function () {
    utilService.render('controller', controller);
    $('#loading').hide();
    $('#main').show();
};

controller.main = function () {
    service.getAppointmentParameter(function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {

            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.appointmentInfo = data.appointmentInfo;
            controller.appointmentItemInfo = data.appointmentItemInfo;
           
            controller.render();

        } else {
            notifyService.error(data.errorMessage);
        }

        document.title = data.appointmentInfo.title;
    });
};

controller.main();