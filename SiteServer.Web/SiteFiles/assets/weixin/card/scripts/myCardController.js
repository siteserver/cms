var controller = {};


controller.poweredBy = '';
controller.isLoading = true;
controller.isSuccess = false;
controller.isPoweredBy = false;
controller.isBinding = false;
controller.isExchange = false;
controller.isSign = false;

controller.cardInfo = {};
controller.cardSNInfo = {};
controller.userInfo = {};
 
controller.render = function () {
    utilService.render('controller', controller);

    $(".my_card_box .vip_card").click(function () {
        if ($(this).data("id") == 1) {
            $(this).find(".front").addClass("front_on");
            $(this).find(".back").addClass("back_on");
            $(this).data("id", 2)
        } else {
            $(this).find(".front").removeClass("front_on");
            $(this).find(".back").removeClass("back_on");
            $(this).data("id", 1)
        }
    })

    $('#loading').hide();
    $('#main').show();
}
  
controller.openUser = function () {
    location.href = 'user.html?publishmentSystemID=' + service.publishmentSystemID + '&cardID=' + service.cardID ;
};

controller.openBindCard = function () {
    location.href = 'bindCard.html?publishmentSystemID=' + service.publishmentSystemID + '&cardID=' + service.cardID;
};

controller.openConsume = function () {
    location.href = 'consume.html?publishmentSystemID=' + service.publishmentSystemID + '&cardID=' + service.cardID;
};

controller.openExchange = function () {
    location.href = 'exchange.html?publishmentSystemID=' + service.publishmentSystemID + '&cardID=' + service.cardID;
};

controller.openMyBill = function () {
    location.href = 'myBill.html?publishmentSystemID=' + service.publishmentSystemID + '&cardID=' + service.cardID;
};


controller.openSign = function () {
    location.href = 'sign.html?publishmentSystemID=' + service.publishmentSystemID + '&cardID=' + service.cardID;
};
 
controller.main = function () { 

    service.getCardParameter(service.cardID, function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {
            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.isBinding = data.isBinding;
            controller.isExchange = data.isExchange;
            controller.isSign = data.isSign;
            controller.cardInfo = data.cardInfo;
            controller.cardSNInfo = data.cardSNInfo;
            controller.userInfo = data.userInfo;

            controller.render();
         } else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.cardInfo.title;
    });
}

controller.main();

controller.getAmount = function (amount) {
    amount = amount + "";
    if (amount.indexOf(".") == -1) {
        amount = amount + ".00";
    }
    else {
        if (amount.substr(amount.indexOf(".")+1).length == 1) {
            amount = amount + "0";
        }
    }
    return amount;
};