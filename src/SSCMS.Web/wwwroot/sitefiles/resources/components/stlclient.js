function StlClient() {}

StlClient.parse = function(responseText) {
    return responseText ? JSON.parse(responseText) : {};
};

StlClient.getQueryString = function() {
    var i, len, params = '',
        hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    if (hashes.length === 0) return params;
    for (i = 0, len = hashes.length, len; i < len; i += 1) {
        hash = hashes[i].split('=');
        if (hash && hash.length == 2) {
            value = encodeURIComponent(decodeURIComponent(decodeURIComponent(hash[1])));
            params += (hash[0] + '=' + value + '&');
        }
    }
    if (params && params.length > 0) params = params.substring(0, params.length - 1);
    return params;
};

StlClient.errorCode = function(status) {
    switch (status) {
        case 400:
            return 'Bad Request';
        case 401:
            return 'Unauthorized';
        case 402:
            return 'Payment Required';
        case 403:
            return 'Forbidden';
        case 404:
            return 'Not Found';
        case 405:
            return 'Method Not Allowed';
        case 406:
            return 'Not Acceptable';
        case 407:
            return 'Proxy Authentication Required';
        case 408:
            return 'Request Timeout';
        case 409:
            return 'Conflict';
        case 410:
            return 'Gone';
        case 411:
            return 'Length Required';
        case 500:
            return 'Internal Server Error';
    }
    return 'Unknown Error';
};

StlClient.prototype = {
    _getURL: function(url, data, method) {
        url += ((/\?/).test(url) ? '&' : '?');
        // Fix #195 about XMLHttpRequest.send method and GET/HEAD request
        if (_.isObject(data) && _.indexOf(['GET', 'HEAD'], method) > -1) {
            url += '&' + _.map(data, function(v, k) {
                return k + '=' + encodeURIComponent(v);
            }).join('&');
        }
        return url + '&' + (new Date()).getTime();
    },

    _request: function(method, path, data, cb) {
        var xhr = new XMLHttpRequest();
        xhr.open(method, this._getURL(path, data, method), true)
        xhr.withCredentials = true;
        if (cb) {
            xhr.onreadystatechange = function() {
                if (xhr.readyState === 4) {
                    if (xhr.status < 400) {
                        cb(null, StlClient.parse(xhr.responseText), xhr.status);
                    } else {
                        var err = StlClient.parse(xhr.responseText);
                        cb({
                            status: xhr.status,
                            message: err.message || StlClient.errorCode(xhr.status)
                        }, null, xhr.status);
                    }
                }
            };
        }

        xhr.dataType = 'json';
        xhr.setRequestHeader('Accept', 'application/vnd.siteserver+json; version=1');
        xhr.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
        if (data) {
            xhr.send(JSON.stringify(data));
        } else {
            xhr.send();
        }
    },

    getURL: function(path) {
        return this._getURL(path, null, "get");
    },

    get: function(path, data, cb) {
        return this._request("GET", path, data, cb);
    },

    post: function(path, data, cb) {
        return this._request("POST", path, data, cb);
    },

    patch: function(path, data, cb) {
        return this._request("PATCH", path, data, cb);
    },

    put: function(path, data, cb) {
        return this._request("PUT", path, data, cb);
    },

    delete: function(path, data, cb) {
        return this._request("DELETE", path, data, cb);
    }
};

var stlClient = new StlClient();