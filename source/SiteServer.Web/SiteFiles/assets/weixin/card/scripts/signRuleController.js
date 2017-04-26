var controller = {};
 
controller.poweredBy = '';
controller.isLoading = true;
controller.isSuccess = false;
controller.isPoweredBy = false;
 
controller.signRuleList = [];
 
controller.render = function () {
    utilService.render('controller', controller);
     
    $('#loading').hide();
    $('#main').show();
}
 
   
controller.main = function () {

    service.getSignRule( function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {
            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.signRuleList = data.signRuleList;

            controller.render();
         } else {
            notifyService.error(data.errorMessage);
        }
    });
}

controller.main();
  