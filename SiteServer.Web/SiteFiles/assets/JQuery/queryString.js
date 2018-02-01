$.request = (function() {
    var apiMap = {};

    function request(queryStr) {
        var api = {};
        if (apiMap[queryStr]) {
            return apiMap[queryStr];
        }
        api.queryString = (function() {
            var urlParams = {};
            var e,
                d = function(s) {
                    return decodeURIComponent(s.replace(/\+/g, " "));
                },
                q = queryStr.substring(queryStr.indexOf('?') + 1),
                r = /([^&=]+)=?([^&]*)/g;
            while (e = r.exec(q))
                urlParams[d(e[1])] = d(e[2]);

            return urlParams;
        })();
        api.getUrl = function() {
            var url = queryStr.substring(0, queryStr.indexOf('?') + 1);
            for (var p in api.queryString) {
                url += p + '=' + api.queryString[p] + "&";
            }
            if (url.lastIndexOf('&') == url.length - 1) {
                return url.substring(0, url.lastIndexOf('&'));
            }
            return url;
        }
        apiMap[queryStr] = api;
        return api;
    }
    $.extend(request, request(window.location.href));
    return request;
})();
