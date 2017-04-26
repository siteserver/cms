var controller = {};

controller.isLoading = true;

controller.conferenceID = utilService.getUrlVar('conferenceID');
controller.wxOpenID = utilService.getUrlVar('wxOpenID');

controller.isPoweredBy = false;
controller.poweredBy = '';
controller.conferenceInfo = {};
controller.isApplied = false;

controller.render = function () {

    $('body').css('background-image', 'url(' + controller.conferenceInfo.backgroundImageUrl + ')');
    $('title').html(controller.conferenceInfo.conferenceName);
    utilService.render('controller', controller);
    var mapWD = $("#mapWD").html();
    getMapLocation(mapWD);
    $('#loading').hide();
    $('#loading').hide();
    $('#main').show();
};


function getMapLocation(mapWD) {

    var map = new BMap.Map("placecontainer");
    var point = new BMap.Point(116.331398, 39.897445);
    map.centerAndZoom(point, 12);
    map.enableScrollWheelZoom(true);
    map.enableContinuousZoom();
    var myGeo = new BMap.Geocoder();
    myGeo.getPoint("" + mapWD + "", function (point) {
        if (point) {
            map.centerAndZoom(point, 16);
            map.addOverlay(new BMap.Marker(point));
        }
    }, $("#mapPlace").html());
}

controller.main = function () {
    service.getConferenceParameter(controller.conferenceID, controller.wxOpenID, function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {

            if (data.isEnd) {
                controller.openEnd();
            } else {
                controller.isPoweredBy = data.isPoweredBy;
                controller.poweredBy = data.poweredBy;
                controller.isApplied = data.isApplied;
                controller.conferenceInfo = data.conferenceInfo;
                controller.conferenceInfo.agendaList = JSON.parse(controller.conferenceInfo.agendaList);
                controller.conferenceInfo.guestList = JSON.parse(controller.conferenceInfo.guestList);
                for (var i = 0; i < controller.conferenceInfo.guestList.length; i++) {
                    var guest = controller.conferenceInfo.guestList[i];
                };

                controller.render();
            }

        } else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.conferenceInfo.title;
    });
};

controller.main();

controller.openApplication = function () {
    location.href = 'application.html?publishmentSystemID=' + service.publishmentSystemID + '&conferenceID=' + controller.conferenceID + '&wxOpenID=' + controller.wxOpenID;
};

controller.openEnd = function () {
    location.href = 'end.html?publishmentSystemID=' + service.publishmentSystemID + '&conferenceID=' + controller.conferenceID + '&wxOpenID=' + controller.wxOpenID;
};

controller.getDateTime = function (d) {
    return utilService.getDateTime(d);
};