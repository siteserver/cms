var controller = {};

controller.isLoading = true;

controller.conferenceID = utilService.getUrlVar('conferenceID');
controller.wxOpenID = utilService.getUrlVar('wxOpenID');

controller.isPoweredBy = false;
controller.poweredBy = '';
controller.conferenceInfo = {};

controller.render = function () {
    utilService.render('controller', controller);
    $('#loading').hide();
    $('#main').show();
};

controller.main = function () {

    service.getConferenceParameter(controller.conferenceID, controller.wxOpenID, function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {

            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.conferenceInfo = data.conferenceInfo;

            controller.render();

        } else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.conferenceInfo.title;
    });

};

controller.main();