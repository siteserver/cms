var controller = {};
 
controller.poweredBy = '';
controller.isLoading = true;
controller.isSuccess = false;
controller.isPoweredBy = false;
  
controller.cardInfo = {};
controller.cardSNInfo = {};
controller.cardEntitySNInfo = {};
 
controller.render = function () {
    utilService.render('controller', controller);
      
    $("#btnBind").click(function () {
        controller.BindCard();
    });

    $('#loading').hide();
    $('#main').show();
}

controller.BindCard = function () {

    var cardSN = $('#cardSN').val();
    var mobile = $('#mobile').val();

    if (cardSN.length <= 0) {
        notifyService.error('卡号不能为空');
        $('#cardSN').focus();
        return;
    }

    if (mobile.length <= 0) {
        notifyService.error('手机不能为空');
        $('#mobile').focus();
        return;
    }
    else if (!utilService.isMobile(mobile)) {
        notifyService.error('请输入正确的手机号码');
        $('#mobile').focus();
        return
    }
     
    service.bindCard(service.cardID, cardSN, mobile, function (data) {
        controller.isSuccess = data.isSuccess;
        if (controller.isSuccess) {
            controller.render();
            if (data.errorMessage.length > 0) {
                notifyService.error(data.errorMessage);
            }
            else {
                service.openMyCard();
            }
         } else {
            notifyService.error(data.errorMessage);
        }

    });
};
   
controller.main = function () {

    service.getCardParameter(service.cardID, function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {
            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.cardInfo = data.cardInfo;
            controller.cardSNInfo = data.cardSNInfo;
            controller.cardEntitySNInfo = data.cardEntitySNInfo;

            controller.render();
         } else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.cardInfo.title;
    });
}

controller.main();

 