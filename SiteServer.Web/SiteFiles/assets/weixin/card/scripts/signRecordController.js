var controller = {};
 
controller.poweredBy = '';
controller.isLoading = true;
controller.isSuccess = false;
controller.isPoweredBy = false;
 
controller.userCreditsLogInfoList = [];
 
controller.render = function () {
    utilService.render('controller', controller);
      
    $('#loading').hide();
    $('#main').show();
}
 
controller.main = function () {

    service.getSignRecord(function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {
            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.userCreditsLogInfoList = data.userCreditsLogInfoList;

            controller.render();
         } else {
            notifyService.error(data.errorMessage);
        }
    });
}

controller.main();

controller.getDateTime = function (d) {
    return d.substr(0, d.indexOf('T'))
};