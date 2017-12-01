var controller = {};
 
controller.poweredBy = '';
controller.isLoading = true;
controller.isSuccess = false;
controller.isPoweredBy = false;

controller.userInfo = {};
controller.cardInfo = {};
controller.cardSNInfo = {};
controller.cardSignLogInfoList = [];
  
controller.render = function () {
    utilService.render('controller', controller);    
    $("#btnSubmit").click(function () {
        controller.Sign();
    });

    $('#loading').hide();
    $('#main').show();

    getCalendar();
     
}

controller.openSignRecord = function () {
    location.href = 'signRecord.html?publishmentSystemID=' + service.publishmentSystemID + '&cardID=' + service.cardID;
};

controller.openSignRule = function () {
    location.href = 'signRule.html?publishmentSystemID=' + service.publishmentSystemID + '&cardID=' + service.cardID;
};

controller.Sign = function () {
    service.sign(function (data) {
        controller.isSuccess = data.isSuccess;
        if (controller.isSuccess) {
            controller.render();
            if (data.errorMessage.length > 0) {
                notifyService.error(data.errorMessage);
            }
            else {
                //notifyService.success("签到成功");
                location.href = 'sign.html?publishmentSystemID=' + service.publishmentSystemID + '&cardID=' + service.cardID;
            }
            
        } else {
            notifyService.error(data.errorMessage);
        }

    });
};
   
controller.main = function () {   
    service.getSignRecord(function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {
            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.userInfo = data.userInfo;
            controller.cardSignLogInfoList = data.cardSignLogInfoList;
            
            controller.render();
         } else {
            notifyService.error(data.errorMessage);
        }
    });
 
}

controller.main();   
 


