var controller = {};
 
controller.poweredBy = '';
controller.isLoading = true;
controller.isSuccess = false;
controller.isPoweredBy = false;
  
controller.cardInfo = {};
controller.cardSNInfo = {};
 
controller.render = function () {
    utilService.render('controller', controller);
      
    $("#btnSubmit").click(function () {
        controller.Consume();
    });

    $('#loading').hide();
    $('#main').show();
}

controller.Consume = function () {

    var amount = $('#amount').val();
    var operator = $('#operator').val();
    var password = $('#password').val();
    
    if (amount.length <= 0) {
        notifyService.error('消费金额不能为空');
        $('#amount').focus();
        return;
    }
    else if (!utilService.isPrice(amount)) {
        notifyService.error('请输入正确的消费金额');
        $('#amount').focus();
        return;
    }
    if (operator==null) {
        notifyService.error('操作员不能为空');
        return;
    }
    if (password.length <= 0) {
        notifyService.error('消费密码不能为空');
        $('#password').focus();
        return;
    }
     
     
    service.consume(service.cardID, amount, operator,password, function (data) {
        controller.isSuccess = data.isSuccess;
        if (controller.isSuccess) {
            controller.render();
            if (data.errorMessage.length > 0) {
                notifyService.error(data.errorMessage);
            }
            else {
                notifyService.success('消费操作成功');
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
            controller.cardInfo.shopOperatorList = JSON.parse(controller.cardInfo.shopOperatorList);

            controller.render();
         } else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.cardInfo.title;
    });
}

controller.main();

 