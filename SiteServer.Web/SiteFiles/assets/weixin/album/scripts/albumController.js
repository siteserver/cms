var controller = {};

controller.isLoading = true;

controller.albumID = utilService.getUrlVar('albumID');
controller.wxOpenID = utilService.getUrlVar('wxOpenID');

controller.isPoweredBy = false;
controller.poweredBy = '';
controller.title = '';
controller.contentInfoList = [];

controller.render = function () {
    utilService.render('controller', controller);

    $('#loading').hide();
    $('#main').show();
};

controller.main = function () {
    service.getAlbumParameter(controller.albumID, controller.wxOpenID, function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {

            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.title = data.title;
            controller.contentInfoList = data.contentInfoList;

            controller.render();
        } else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.title;
    });
};

controller.getPhotoUrl = function (parentID) {
    return 'photo.html?publishmentSystemID=' + service.publishmentSystemID + '&albumID=' + controller.albumID + '&wxOpenID=' + controller.wxOpenID + '&parentID=' + parentID;
};

controller.main();