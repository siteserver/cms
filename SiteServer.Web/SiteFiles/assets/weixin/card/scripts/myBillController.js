var controller = {};
 
controller.poweredBy = '';
controller.isLoading = true;
controller.isSuccess = false;
controller.isPoweredBy = false;
controller.exchangeProportion = '';
  
controller.userInfo = {};
controller.cardInfo = {};
controller.cardSNInfo = {};
controller.cardCashYearCountInfoList = [];
 
controller.render = function () {
    utilService.render('controller', controller);

    $(".bill_nav a").click(function () {
        var index = $(".bill_nav a").index($(this));
        $(this).addClass("on").siblings().removeClass("on");
        $(".bill_content").hide().eq(index).show();
    })

    $('#loading').hide();
    $('#main').show();
}
 
controller.main = function () {

    service.getCardCashLog(service.cardID, function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {
            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.userInfo = data.userInfo;
            controller.cardInfo = data.cardInfo;
            controller.cardSNInfo = data.cardSNInfo;
            controller.cardCashYearCountInfoList = data.cardCashYearCountInfoList;

            controller.render();
         } else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.cardInfo.title;
    });
}

controller.main();

controller.getDateTime = function (d) {
    return utilService.getDateTime(d);
};

controller.getAmount = function (amount) {
    amount = amount + "";
    if (amount.indexOf(".") == -1) {
        amount = amount + ".00";
    }
    else {
        if (amount.substr(amount.indexOf(".") + 1).length == 1) {
            amount = amount + "0";
        }
    }
    return amount;
};