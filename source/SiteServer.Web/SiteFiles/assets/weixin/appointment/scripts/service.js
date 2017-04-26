var service = {};
service.baseUrl = '/api/wx_appointment/';
service.publishmentSystemID = utilService.getUrlVar('publishmentSystemID');

service.getUrl = function (action, id) {
    if (id) {
        return service.baseUrl + action + '/' + id + '?publishmentSystemID=' + service.publishmentSystemID;
    }
    return service.baseUrl + action + '?publishmentSystemID=' + service.publishmentSystemID;
};

service.getAppointmentParameter = function (appointmentID, success) {

    $.getJSON(service.getUrl('GetAppointmentParameter', service.appointmentID), { 'wxOpenID': service.wxOpenID }, success);
};

service.getAppointmentItemParameter = function (contentID, success) {

    $.getJSON(service.getUrl('GetAppointmentItemParameter', service.appointmentID), { 'wxOpenID': service.wxOpenID, "itemID": service.itemID, "contentID": contentID }, success);
};

service.getAppointmentContentParameter = function (success) {
    $.getJSON(service.getUrl('GetAppointmentContentParameter', service.appointmentID), { 'wxOpenID': service.wxOpenID }, success);
};

service.submitApplication = function (realName, mobile, email, settingsXml, success) {
    $.getJSON(service.getUrl('SubmitApplication', service.appointmentID), { 'itemID': service.itemID, 'wxOpenID': service.wxOpenID, 'realName': realName, 'mobile': mobile, 'email': email, 'settingsXml': settingsXml }, success);
};

service.appointmentID = utilService.getUrlVar('appointmentID');
service.itemID = utilService.getUrlVar('itemID');
service.wxOpenID = utilService.getUrlVar('wxOpenID');

service.openIndex = function () {
    location.href = 'index.html?publishmentSystemID=' + service.publishmentSystemID + '&appointmentID=' + service.appointmentID + '&wxOpenID=' + service.wxOpenID;
};

service.openItem = function (itemID, contentID) {
    if (!itemID) {
        itemID = service.itemID;
    }
    location.href = 'item.html?publishmentSystemID=' + service.publishmentSystemID + '&appointmentID=' + service.appointmentID + '&itemID=' + itemID + '&contentID=' + contentID + '&wxOpenID=' + service.wxOpenID;
};

service.openResult = function () {
    location.href = 'result.html?publishmentSystemID=' + service.publishmentSystemID + '&appointmentID=' + service.appointmentID + '&itemID=' + service.itemID + '&wxOpenID=' + service.wxOpenID;
};

service.openApplication = function (isSingle) {
    location.href = 'application.html?publishmentSystemID=' + service.publishmentSystemID + '&appointmentID=' + service.appointmentID + '&itemID=' + service.itemID + '&wxOpenID=' + service.wxOpenID + '&isSingle=' + isSingle;
};

service.openEnd = function () {
    location.href = 'end.html?publishmentSystemID=' + service.publishmentSystemID + '&appointmentID=' + service.appointmentID + '&wxOpenID=' + service.wxOpenID;
};