var controller = {};

controller.contentID = utilService.getUrlVar('contentID');

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

    $('.modal-body').height($(window).height() - 200);

    var options = {
        autoResize: true, // This will auto-update the layout when the browser window is resized.
        container: $('#photoList'), // Optional, used for some extra CSS styling
        offset: 2, // Optional, the distance between grid items
        itemWidth: 144 // Optional, the width of a grid item
    };

    // Get a reference to your grid items.
    var handler = $('#photoList li');

    // Call the layout function.
    handler.wookmark(options);
    window.setTimeout(function () {
        handler.wookmark(options);
    }, 500);
};

controller.getDateTime = function (dt) {
    return utilService.getDateTime(dt);
};

controller.main = function () {
    service.getAppointmentItemParameter(controller.contentID, function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {

            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.appointmentInfo = data.appointmentInfo;
            controller.appointmentItemInfo = data.appointmentItemInfo;
            controller.appointmentContentInfo = data.appointmentContentInfo;
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