var controller = {};
 
controller.poweredBy = '';
controller.isLoading = true;
controller.isSuccess = false;
controller.isPoweredBy = false;
controller.exchangeProportion = '';
  
controller.userInfo = {};
controller.cardInfo = {};
controller.cardSNInfo = {};
 
controller.render = function () {
    utilService.render('controller', controller);
      
    $("#credits").blur(function () {
        if (!(/^(\+)?\d+$/.test($(this).val()))) {//isNaN($.trim($(this).val()))
            notifyService.error("请输入正确的兑换积分");
            $(this).val("");
        }
        else {
            if ($.trim($(this).val()).length > 0)
            {
                if($(this).val()=="0")
                {
                    notifyService.error("兑换积分大于0");
                    $(this).val("");
                }
                else
                {
                    var credits = parseInt($.trim($(this).val()));
                    if (controller.userInfo.credits < credits) {
                        notifyService.error("兑换积分大于可用积分");
                        $(this).val("");
                    }
                    else {
                        var amount = credits / parseFloat(controller.exchangeProportion);
                        $("#amount").val(amount);
                    }
                }
             }
         }
    });

    $("#btnSubmit").click(function () {
        controller.Exchange();
    });

    $('#loading').hide();
    $('#main').show();
}

controller.Exchange = function () {

    var credits = $('#credits').val();
    
    if (credits.length <= 0) {
        notifyService.error("兑换积分不能为空");
        $('#credits').focus();
        return;
    }
    else if (isNaN(credits)) {
        notifyService.error("请输入正确的兑换积分");
        $('#credits').focus();
        return;
    }
    
    service.exchange(service.cardID, credits, function (data) {
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
            controller.userInfo = data.userInfo;
            controller.cardInfo = data.cardInfo;
            controller.cardSNInfo = data.cardSNInfo;
            controller.exchangeProportion = data.exchangeProportion;

            controller.render();
         } else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.cardInfo.title;
    });
}

controller.main();

 