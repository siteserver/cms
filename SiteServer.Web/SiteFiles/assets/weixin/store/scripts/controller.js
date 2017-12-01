var controller = {};

controller.isLoading = true;
controller.storeID = utilService.getUrlVar('storeID');
controller.parentID = utilService.getUrlVar('parentID');
controller.categoryID = utilService.getUrlVar('categoryID');
controller.storeItemID = utilService.getUrlVar('storeItemID');
controller.wxOpenID = utilService.getUrlVar('wxOpenID');

controller.isPoweredBy = false;
controller.poweredBy = '';
controller.title = '';
controller.contentInfoList = [];

controller.render = function () {
    utilService.render('controller', controller);

    $('#loading').hide();
    $('#main').show();
};


//获取门店属性信息
controller.main = function () {
    service.getStoreCategory(
        controller.storeID,
        controller.wxOpenID,
        controller.parentID,
        function (data) {
            controller.isLoading = false;
            if (data.isSuccess) {
                controller.isPoweredBy = data.isPoweredBy;
                controller.poweredBy = data.poweredBy;
                controller.title = data.title;
                controller.contentInfoList = data.contentInfoList;
                if (data.contentInfoList.length == 0) {
                    location.href = 'store.html?publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID + '&storeID=' + controller.storeID + '&categoryID=0';
                }

                controller.render();
            }
            else {
                notifyService.error(data.errorMessage);
            }
            document.title = data.title;
        });
};


//根据属性ID获取店面信息
controller.store = function () {
    service.getStoreItemList(
        controller.storeID,
        controller.categoryID,
        controller.wxOpenID,
        function (data) {
            controller.isLoading = false;
            if (data.isSuccess) {
                controller.isPoweredBy = data.isPoweredBy;
                controller.poweredBy = data.poweredBy;
                controller.title = data.title;
                controller.contentInfoList = data.contentInfoList;                
                controller.render();
            }
            else {
                notifyService.error(data.errorMessage);
            }
        });
};

//根据店面ID获取店面详情
controller.storeItem = function () {
    service.getStoreItemInfo(
        controller.storeID,
        controller.storeItemID,
        controller.wxOpenID,
        function (data) {
            controller.isLoading = false;
            if (data.isSuccess) {
                controller.isPoweredBy = data.isPoweredBy;
                controller.poweredBy = data.poweredBy;
                controller.title = data.title;
                controller.contentInfo = data.contentInfo;
                controller.render();
                controller.setMap(data.contentInfo.longitude, data.contentInfo.latitude);
            }
            else {
                notifyService.error(data.errorMessage);
            }
        });
};

controller.setMap = function (hiddenLng, hiddenLat) {
    map = new BMap.Map("mapContent");
    map.enableScrollWheelZoom(true);
    map.enableContinuousZoom();
    map.centerAndZoom(new BMap.Point(hiddenLng, hiddenLat), 14);
    marker = new BMap.Marker(new BMap.Point(hiddenLng, hiddenLat));  // 创建标注
    map.addOverlay(marker);              // 将标注添加到地图中     
};

controller.getStoreListUrl = function (categoryID, childCount, parentID) {
    if (childCount == 0) {
        return 'store.html?publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID + '&storeID=' + controller.storeID + '&categoryID=' + categoryID;
    }
    else {
        return 'index.html?publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID + '&parentID=' + categoryID + '&storeID=' + controller.storeID;
    }
};

controller.getStoreItemUrl = function (storeItemID) {
    return 'content.html?publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID + '&storeID=' + controller.storeID + '&storeItemID=' + storeItemID;
};

controller.getStoreListReturnUrl = function (storeID) {
    if (storeID > 0) {
        controller.storeID = storeID;
    }
    return 'index.html?publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID + '&parentID=0&storeID=' + controller.storeID;
};

if (controller.categoryID) {
    controller.store();
}
else if (controller.storeItemID) {
    controller.storeItem();
}
else if (controller.storeID) {
    controller.main();
}