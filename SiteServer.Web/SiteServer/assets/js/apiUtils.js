var apiUtils = {
    Api: function(apiUrl) {
        this.getQueryStringByName = function(name) {
            var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
            if (!result || result.length < 1) {
                return "";
            }
            return decodeURIComponent(result[1]);
        };

        this.apiUrl = apiUrl || 'https://api.siteserver.cn/v1.1';
        // this.apiUrl = 'http://localhost:90/v1.1';

        this._getURL = function(url, data, method) {
            url += ((/\?/).test(url) ? '&' : '?');
            if ((typeof data === 'object') && method === 'GET') {
                var pairs = [];
                for (var prop in data) {
                    if (data.hasOwnProperty(prop)) {
                        var k = encodeURIComponent(prop),
                            v = encodeURIComponent(data[prop]);
                        pairs.push( k + "=" + v);
                    }
                }
                url += "&" + pairs.join("&");
            }
            return (url + '&' + (new Date()).getTime()).replace('?&', '?');
        };

        this.request = function(method, path, data, cb) {
            var xhr = new XMLHttpRequest();
            xhr.open(method, this._getURL(path, data, method), true);
            xhr.withCredentials = true;
            if (cb) {
                xhr.onreadystatechange = function() {
                    if (xhr.readyState === 4) {
                        if (xhr.status < 400) {
                            cb(null, apiUtils.parse(xhr.responseText), xhr.status);
                        } else {
                            var err = apiUtils.parse(xhr.responseText);
                            cb({
                                status: xhr.status,
                                message: err.message || apiUtils.errorCode(xhr.status)
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
        };
    

        this.get = function(data, cb, name, id) {
            var url = this.apiUrl;
            if (name) {
                url += '/' + name;
                if (id) {
                    url += '/' + id;
                }
            }
            return this.request("GET", url, data, cb);
        };

        this.post = function(data, cb, name, id) {
            var url = this.apiUrl;
            if (name) {
                url += '/' + name;
                if (id) {
                    url += '/' + id;
                }
            }
            return this.request("POST", url, data, cb);
        };

        this.put = function(data, cb, name, id) {
            var url = this.apiUrl;
            if (name) {
                url += '/' + name;
                if (id) {
                    url += '/' + id;
                }
            }
            return this.request("PUT", url, data, cb);
        };

        this.delete = function(data, cb, name, id) {
            var url = this.apiUrl;
            if (name) {
                url += '/' + name;
                if (id) {
                    url += '/' + id;
                }
            }
            return this.request("DELETE", url, data, cb);
        };

        this.patch = function(data, cb, name, id) {
            var url = this.apiUrl;
            if (name) {
                url += '/' + name;
                if (id) {
                    url += '/' + id;
                }
            }
            return this.request("PATCH", url, data, cb);
        };
    },

    parse: function(responseText) {
        return responseText ? JSON.parse(responseText) : {};
    },

    errorCode: function(status) {
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
    }
};