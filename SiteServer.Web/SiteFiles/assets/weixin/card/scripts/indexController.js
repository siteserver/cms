var controller = {};


controller.poweredBy = '';
controller.isLoading = true;
controller.isSuccess = false;
controller.isPoweredBy = false;
controller.isAnonymous = false;
 
controller.cardInfo = {};
controller.cardSNInfo = {};

 
controller.render = function () {
    utilService.render('controller', controller);
     

    $('#loading').hide();
    $('#main').show();
}
    
controller.main = function () {

    service.getCardParameter(service.cardID, function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {
            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.cardInfo = data.cardInfo;
            controller.cardSNInfo = data.cardSNInfo;
            controller.isAnonymous = data.isAnonymous;
            if (!controller.isAnonymous) {
                service.openMyCard();
            }
            else {
                controller.render();
            }
           
         } else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.cardInfo.title;
    });
}

controller.main();

 