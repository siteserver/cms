var service = {};
service.baseUrl = '/api/wx_coupon/';
service.publishmentSystemID = utilService.getUrlVar('publishmentSystemID');

service.getUrl = function (action, id) {
    if (id) {
        return service.baseUrl + action + '/' + id + '?publishmentSystemID=' + service.publishmentSystemID;
    }
    return service.baseUrl + action + '?publishmentSystemID=' + service.publishmentSystemID;
};

service.getCouponParameter = function (snID, wxOpenID, success) {
    $.getJSON(service.getUrl('GetCouponParameter', snID), { 'wxOpenID': wxOpenID }, success);
};

service.getCouponParameterInfo = function (actID, wxOpenID, success) {
    $.getJSON(service.getUrl('GetCouponParameterInfo', actID), { 'wxOpenID': wxOpenID }, success);
};

service.submitApplication = function (actID, realName, mobile, email, address, success, flag) {
    $.getJSON(service.getUrl('submitApplication', actID), { 'realName': realName, 'mobile': mobile, 'email': email, 'address': address }, success);
};

service.submitAwardCode = function (CouponSnID, success) {
    $.getJSON(service.getUrl('SubmitAwardCode', CouponSnID), {}, success);
};