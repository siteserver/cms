var controller = {};

controller.login = {};
controller.userName = '';
controller.poweredBy = '';
controller.isLoading = true;
controller.isSuccess = false;
controller.isPoweredBy = false;

   
controller.render = function () {
    utilService.render('controller', controller);
     
    $('#btnLogin').click(function () {
        controller.submit();
    });

    $(".forget_box label").click(function () {
        if ($(this).hasClass("on")) {
            $(this).removeClass("on");
            $(".forget_box input").val("0")
        } else {
            $(this).addClass("on");
            $(".forget_box input").val("1")
        }
   })

    $('#loading').hide();
    $('#main').show();
    $('#userName').focus();
}
  
controller.submit = function () {
     
    controller.login.userName = $('#userName').val();
    controller.login.password = $("#password").val();
    controller.login.isPersistent = $('#isPersistent').val() == "0" ? false : true;
    
    var isValid = true;
    if (!controller.login.userName && !controller.login.password) {
        notifyService.error("请输入登录账号及密码！");
        $('#userName').focus();
        isValid = false;
    }
    else if (!controller.login.userName) {
        notifyService.error("请输入登录账号！");
        $('#userName').focus();
        isValid = false;
    }
    else if (!controller.login.password) {
        notifyService.error("请输入登录密码！");
        $('#password').focus();
        isValid = false;
    }
    if (!isValid) return false;
    
    service.login(service.cardID,controller.login.userName, controller.login.password, controller.login.isPersistent, function (data) {
        controller.isSuccess = data.isSuccess;
        if (controller.isSuccess) {
            service.openMyCard();
        } else {
            controller.render();
            //notifyService.error(data.errorMessage);
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
          
            controller.render();
         } else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.cardInfo.title;
    });
}

controller.main();

 