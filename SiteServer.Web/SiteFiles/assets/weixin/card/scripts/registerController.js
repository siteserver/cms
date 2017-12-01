var controller = {};

controller.login = {};
controller.userName = '';
controller.poweredBy = '';
controller.errorMessage = '';
controller.isLoading = true;
controller.isSuccess = false;
controller.isPoweredBy = false;
 
 
controller.render = function () {
    utilService.render('controller', controller);
    $('#btRegister').click(function () {
        controller.register();
    });

    $('#loading').hide();
    $('#main').show();
}
 
controller.register = function () {
     
    controller.login.userName = $('#userName').val();
    controller.login.email = $('#email').val();
    controller.login.mobile = $('#mobile').val();
    controller.login.password = $("#password").val();
    controller.login.confirmPassword = $("#confirmPassword").val();
    
    var isValid = true;
    if (!controller.login.userName) {
        notifyService.error('请输入登录账号');
        $('#loginName').focus();
        isValid = false;
    }
    else if (!controller.login.email) {
        notifyService.error('请输入邮箱');
        $('#email').focus();
        isValid = false;
    }
    else if (!utilService.isEmail(controller.login.email)) {
        notifyService.error('请输入正确的邮箱地址');
        $('#email').focus();
        isValid = false;
    }
    else if (!controller.login.mobile) {
        notifyService.error('请输入手机号码');
        $('#mobile').focus();
        isValid = false;
    }
    else if (!utilService.isMobile(controller.login.mobile)) {
        notifyService.error('请输入正确的手机号码');
        $('#mobile').focus();
        isValid = false;
    }
    else if (!controller.login.password) {
        notifyService.error('请输入登录密码');
        $('#password').focus();
        isValid = false;
    }
    else if (controller.login.password.length < 6 || controller.login.password.length > 20) {
        notifyService.error('密码长度只能在6-20位字符之间');
        $('#password').focus();
        isValid = false;
    }
    else if ($("#confirmPassword").length > 0 && controller.login.password != controller.login.confirmPassword) {
        notifyService.error('确认密码与密码不一致，请重新输入');
        $('#confirmPassword').focus();
        isValid = false;
    }
    if (!isValid) return false;
    
    service.register(service.cardID, controller.login.userName,controller.login.email,controller.login.mobile,controller.login.password, function (data) {
        controller.isSuccess = data.isSuccess;
        if (controller.isSuccess) {
            service.openLogin();

        } else {
            notifyService.error(data.errorMessage);
        }
         
    });
};
   
controller.main = function () {
     
    service.getCardParameter(service.cardID,function (data) {
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

 