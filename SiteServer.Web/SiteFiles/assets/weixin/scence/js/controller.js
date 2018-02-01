var controller = {};

controller.isLoading = true;

controller.scenceID = utilService.getUrlVar('scenceID');
controller.wxOpenID = utilService.getUrlVar('wxOpenID');
controller.keyword = utilService.getUrlVar('keyword');

controller.data = {};
controller.isPoweredBy = false;
controller.poweredBy = '';
controller.clickNum = '';

controller.render = function () {

    utilService.render('controller', controller);

    $('#loading').hide();
    $('#main').show();
};

controller.main = function () {

    service.getScenceParameter(controller.scenceID, function (data) {
        controller.isLoading = false;
        controller.clickNum = data;
    });
};

controller.main();
