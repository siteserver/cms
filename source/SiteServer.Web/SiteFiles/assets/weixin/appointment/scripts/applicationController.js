var controller = {};

controller.isSingle = utilService.getUrlVar('isSingle');
controller.appointmentID = utilService.getUrlVar('appointmentID');

controller.render = function () {
    utilService.render('controller', controller);
    $('#loading').hide();
    $('#main').show();
};

controller.appointmentInfo = {};
controller.configExtendInfoList = [];

controller.submitApplication = function () {

    var realName = $('#realName').val();
    var mobile = $('#mobile').val();
    var email = $('#email').val();

    var settingsXml = "";
    $("#SettingsXML").children(".form-group").each(function () {
        var iptVal = '"' + $(this).children("input").val() + '"';
        var lblVal = '"' + $(this).children("label").html().replace("：", "") + '"';
        var itemVal = lblVal + ":" + iptVal + ",";
        settingsXml += itemVal;
    });
    var strlength = settingsXml.length;
    settingsXml = "{" + settingsXml.substring(0, strlength - 1) + "}";

    $('#myForm').validator('validate');

    if ($("#myForm").children().hasClass("has-error")) {
        return false;
    }


    $('#loading').show();
    $('#main').hide();

    service.submitApplication(realName, mobile, email, settingsXml, function (data) {
        if (data.isSuccess) {

            layer.msg('预约提交成功', 2, 1);

            setTimeout(function () {
                if (controller.isSingle == 'true') {
                    service.openItem();
                } else {
                    service.openResult();
                }
            }, 1000);

        } else {
            notifyService.error(data.errorMessage);
        }
    });
}

controller.main = function () {
    service.getAppointmentParameter(controller.appointmentID, function (data) {
        controller.appointmentInfo = data.appointmentInfo;
        controller.configExtendInfoList = data.configExtendInfoList;
        controller.render();
        document.title = data.appointmentInfo.title;
    });
};

controller.main();
