var service = {};
service.baseUrl = '/api/wx_scence/';
service.publishmentSystemID = utilService.getUrlVar('publishmentSystemID');

service.getUrl = function (action, id) {
    return service.baseUrl + action + '?id=1&publishmentSystemID=1091';
};

service.getScenceParameter = function (scenceID, success) {
    $.getJSON(service.getUrl('GetScenceParameter', scenceID), {}, success);
};
