
function weixinAuth(returnUrl) {

    $.ajax({
        type: "get",
        async: false,
        url: "http://api.gexia.com/weixinauth/WeiXinAuthorize",
        data: { returnUrl: returnUrl },
        dataType: "jsonp",
        success: function (json) {
     
            if (json.length > 0) {
                location.href = json;
            }
        }
    });
}

