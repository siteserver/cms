var service = {};
service.baseUrl = '/api/wx_store/';
service.publishmentSystemID = utilService.getUrlVar('publishmentSystemID');

service.getUrl = function (action, id) {
    if (id) {
        return service.baseUrl + action + '/' + id + '?publishmentSystemID=' + service.publishmentSystemID;
    }
    return service.baseUrl + action + '?publishmentSystemID=' + service.publishmentSystemID;
};

//获取门店属性信息
service.getStoreCategory = function (storeID, wxOpenID, parentID, success) {
    $.getJSON(service.getUrl('GetStoreCategory', storeID), { 'wxOpenID': wxOpenID, 'parentID': parentID }, success);
};

//根据属性ID获取店面信息
service.getStoreItemList = function (storeID, categoryID, wxOpenID, success) {
    $.getJSON(service.getUrl('GetStoreItemList', storeID), { 'categoryID': categoryID, 'wxOpenID': wxOpenID }, success);
};

//根据属性ID获取店面信息
service.getStoreItemInfo = function (storeID, storeItemID, wxOpenID, success) {
    $.getJSON(service.getUrl('GetStoreItemInfo', storeID), { 'storeItemID': storeItemID, 'wxOpenID': wxOpenID }, success);
};