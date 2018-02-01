var controller = {};

controller.isLoading = true;

controller.actID = utilService.getUrlVar('actID');
controller.wxOpenID = utilService.getUrlVar('wxOpenID');
controller.snID = 0;
controller.uniqueID = utilService.getUrlVar('uniqueID');

controller.render = function () {
    utilService.render('controller', controller);

    $('#loading').hide();
    $('#main').show();
};

controller.openCheck = function () {
    location.href = 'index.html?snID=' + controller.snID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID;
};

controller.Auth = function () {
    var loginType = "WeixinMob";
    var returnUrl = location.href;
    var locationUrl = "http://gexia.com/home/authlogin.html?isWeixin=true&loginType=" + loginType + "&returnUrl=" + returnUrl;
    location.href = locationUrl;

};


function isPc() {
    var userAgentInfo = navigator.userAgent;
    var Agents = ["Android", "iPhone", "SymbianOS", "Windows Phone", "iPad", "iPod"];
    var flag = true;
    for (var v = 0; v < Agents.length; v++) {
        if (userAgentInfo.indexOf(Agents[v]) > 0) {
            flag = false;
            break;
        }
    }
    return flag;
}

controller.isSuccess = false;
controller.isPoweredBy = false;
controller.poweredBy = '';
controller.conponActInfo = {};
controller.couponSNInfo = null;

controller.main = function () {
    if (controller.uniqueID == "" && !isPc()) {
        controller.Auth();
        return;
    }
    else {
        controller.wxOpenID = controller.uniqueID;
    }

    service.getCouponParameterInfo(controller.actID, controller.wxOpenID, function (data) {
        controller.isSuccess = data.isSuccess;
        controller.isPoweredBy = data.isPoweredBy;
        controller.poweredBy = data.poweredBy;
        controller.conponActInfo = data.conponActInfo;

        if (!controller.conponActInfo.isFormRealName && !controller.conponActInfo.isFormMobile && !controller.conponActInfo.isFormEmail && !controller.conponActInfo.isFormAddress) {
            controller.submitApplication(0);
        }
        if (data.couponSNInfo) {
            controller.snID = data.couponSNInfo.id;
            controller.openCheck();
        }
        else {
            controller.render();
        }

        document.title = data.conponActInfo.title;
    });
};

controller.submitApplication = function (flag) {
    var realName = "";
    var mobile = "";
    var email = "";
    var address = "";
    if (flag == 1) {
        var realName = $('#realName').val();
        var mobile = $('#mobile').val();
        var email = $('#email').val();
        var address = $('#address').val();

        $('#myForm').validator('validate');
        if ($("#myForm").children().hasClass("has-error")) {
            return false;
        }
    }

    $('#loading').show();
    $('#main').hide();

    service.submitApplication(controller.actID, realName, mobile, email, address, function (data) {
        if (data.isSuccess) {
            controller.snID = data.snid;
            controller.openCheck();
        }
        else {
            controller.render();
            $("#alertMessage").html(data.errorMessage);
            $("#alertTitle").html("提示信息");
            $("#divBtn").hide();
            notifyService.error(data.errorMessage);

        }
    });
};

controller.main();