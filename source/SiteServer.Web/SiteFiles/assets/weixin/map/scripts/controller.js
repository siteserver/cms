var controller = {};

controller.isLoading = true;
controller.isPoweredBy = true;
controller.poweredBy = '';

controller.render = function () {

    $('#loading').hide();
    $('#main').show();
    $('#allmap').height($(window).height() - 85);
    var mapWD = request("mapWD");
    mapWD = decodeURIComponent(mapWD);

    getMapLocation(mapWD);

    $("#mapTitle").html(mapWD.substring(0, 14));

    utilService.render('controller', controller);
};

controller.render();


function getMapLocation(mapWD) {

    var map = new BMap.Map("allmap");
    var point = new BMap.Point(116.331398, 39.897445);
    map.centerAndZoom(point, 12);
    map.enableScrollWheelZoom(true);
    map.enableContinuousZoom();
    var myGeo = new BMap.Geocoder();
    myGeo.getPoint("" + mapWD + "", function (point) {
        if (point) {
            map.centerAndZoom(point, 16);
            map.addOverlay(new BMap.Marker(point));
        }
    }, "北京市");
}

/*
* 获取网页传递的参数 调用方法 request("id")
*/
function request(paras) {
    var url = location.href;
    var paraString = url.substring(url.indexOf("?") + 1, url.length).split("&");
    var paraObj = {}
    for (i = 0; j = paraString[i]; i++) {
        paraObj[j.substring(0, j.indexOf("=")).toLowerCase()] = j.substring(j.indexOf("=") + 1, j.length);
    }
    var returnValue = paraObj[paras.toLowerCase()];
    if (typeof (returnValue) == "undefined") {
        return "";
    } else {
        return returnValue;
    }
}