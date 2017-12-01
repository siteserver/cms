$(function () {
    var count = $("#top_menu").children().length;
    for (i = 0; i < count; i++) {
        if ($("#top_menu").children().eq(i).find("ul li").length == 0) {
            $("#top_menu").children().eq(i).find("ul").remove();
        }
    }
})
function displayit(n) {
    var count = document.getElementById("top_menu").getElementsByTagName("ul").length;
    for (i = 0; i < count; i++) {
        if (i == n) {
            if (document.getElementById("top_menu").getElementsByTagName("ul").item(n).style.display == 'none') {
                document.getElementById("top_menu").getElementsByTagName("ul").item(n).style.display = '';
                document.getElementById("plug-wrap").style.display = '';
            } else {
                document.getElementById("top_menu").getElementsByTagName("ul").item(n).style.display = 'none';
                document.getElementById("plug-wrap").style.display = 'none';
            }
        } else {
            document.getElementById("top_menu").getElementsByTagName("ul").item(i).style.display = 'none';
        }
    }
}

function closeall() {
    var count = document.getElementById("top_menu").getElementsByTagName("ul").length;
    for (i = 0; i < count; i++) {
        document.getElementById("top_menu").getElementsByTagName("ul").item(i).style.display = 'none';
    }
    document.getElementById("plug-wrap").style.display = 'none';
}

document.addEventListener('WeixinJSBridgeReady', function onBridgeReady() {
    WeixinJSBridge.call('hideToolbar');
});