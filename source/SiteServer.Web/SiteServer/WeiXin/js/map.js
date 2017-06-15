var map;
var marker;
var myGeo;
var map1;
var marker1;
var myGeo1;
function baidu_map() {
    // 百度地图API功能
    map = new BMap.Map("container");
    myGeo = new BMap.Geocoder();
    map.enableScrollWheelZoom(true);
    map.enableContinuousZoom();
    map.addControl(new BMap.NavigationControl());
    map.addControl(new BMap.ScaleControl());
    map.addControl(new BMap.OverviewMapControl());

    var newLat = $("#txtLatitude").val();
    var newLng = $("#txtLongitude").val();
    if (newLat == "" || newLng == "") {
        newLng = 116.407284;
        newLat = 39.912936;
        $("#txtLongitude").val(newLng);
        $("#txtLatitude").val(newLat);
    }

    map.centerAndZoom(new BMap.Point(newLng, newLat), 14);
    marker = new BMap.Marker(new BMap.Point(newLng, newLat));  // 创建标注
    map.addOverlay(marker);              // 将标注添加到地图中
    //创建信息窗口
    var infoWindow1 = new BMap.InfoWindow("拖拽标点选择门店的地理位置");
    marker.addEventListener("click", function () { this.openInfoWindow(infoWindow1); });
    marker.enableDragging();
    marker.addEventListener("dragend", function (e) {
        $("#txtLatitude").val(e.point.lat);
        $("#txtLongitude").val(e.point.lng);
    });
}

function loadmap() {
    map1 = new BMap.Map("container");
    var city = $("#suggestId").val();
    // 将结果显示在地图上，并调整地图视野      
    // 将地址解析结果显示在地图上,并调整地图视野
    map1.enableScrollWheelZoom(true);
    map1.enableContinuousZoom();
    map1.addControl(new BMap.NavigationControl());
    map1.addControl(new BMap.ScaleControl());
    map1.addControl(new BMap.OverviewMapControl());
    myGeo.getPoint(city, function (point) {
        if (point) {
            map1.centerAndZoom(point, 14);
            marker1 = new BMap.Marker(point);  // 创建标注
            map1.addOverlay(marker1);
            $("#txtLatitude").val(point.lat);
            $("#txtLongitude").val(point.lng);
            marker1.enableDragging();
            marker1.addEventListener("dragend", function (e) {
                $("#txtLatitude").val(e.point.lat);
                $("#txtLongitude").val(e.point.lng);
            });
        }
    }, "北京市");


}