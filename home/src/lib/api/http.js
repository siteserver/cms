import * as base64 from './utils/base64';
import * as utf8 from './utils/utf8';
import * as enums from '../enums';
import * as utils from '../utils';
function base64Encode(text) {
    var bytes = utf8.encode(text);
    return base64.encode(bytes);
}
export function parse(responseText) {
    if (!responseText) {
        return {};
    }
    return JSON.parse(responseText);
}
Date.prototype.toJSON = function () { return this.toLocaleString(); };
function stringify(obj) {
    return JSON.stringify(obj);
}
export class APIRequest {
    constructor(options) {
        this.options = options;
        this.options.api = options.api;
    }
    _getURL(path, data, method, api) {
        var url = path.indexOf('//') >= 0 ? path : api + path;
        url += ((/\?/).test(url) ? '&' : '?');
        if (utils.isObject(data) && utils.indexOf(['GET', 'HEAD'], method) > -1) {
            url += '&' + utils.map(data, function (v, k) {
                return k + '=' + encodeURIComponent(v);
            }).join('&');
        }
        return url + '&' + (new Date()).getTime();
    }
    _request(method, path, data, cb) {
        const xhr = new XMLHttpRequest();
        xhr.open(method, this._getURL(path, data, method, this.options.api), true);
        xhr.withCredentials = true;
        if (cb) {
            xhr.onreadystatechange = () => {
                if (xhr.readyState === 4) {
                    if (xhr.status < 400) {
                        cb(null, parse(xhr.responseText), xhr.status);
                    }
                    else {
                        var err = parse(xhr.responseText);
                        cb({ status: xhr.status, message: err.message || enums.ERestMethodUtils.errorCode(xhr.status) }, null, xhr.status);
                    }
                }
            };
        }
        xhr.dataType = 'json';
        xhr.setRequestHeader('Accept', 'application/vnd.siteserver+json; version=1');
        xhr.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
        if (this.options.username && this.options.password) {
            var authorization = 'Basic ' + base64Encode(this.options.username + ':' + this.options.password);
            xhr.setRequestHeader('Authorization', authorization);
        }
        if (data) {
            xhr.send(stringify(data));
        }
        else {
            xhr.send();
        }
    }
    getURL(path) {
        return this._getURL(path, null, "get", this.options.api);
    }
    get(path, data, cb) {
        return this._request("GET", path, data, cb);
    }
    post(path, data, cb) {
        return this._request("POST", path, data, cb);
    }
    patch(path, data, cb) {
        return this._request("PATCH", path, data, cb);
    }
    put(path, data, cb) {
        return this._request("PUT", path, data, cb);
    }
    delete(path, data, cb) {
        return this._request("DELETE", path, data, cb);
    }
}
export class WebRequest {
    constructor() { }
    _getURL(path, data, method) {
        var url = path;
        url += ((/\?/).test(url) ? '&' : '?');
        if (utils.isObject(data) && utils.indexOf(['GET', 'HEAD'], method) > -1) {
            url += '&' + utils.map(data, function (v, k) {
                return k + '=' + v;
            }).join('&');
        }
        return url + '&' + (new Date()).getTime();
    }
    _request(method, path, authorization, data, cb) {
        var xhr = new XMLHttpRequest();
        xhr.open(method, this._getURL(path, data, method), true);
        xhr.withCredentials = true;
        if (cb) {
            xhr.onreadystatechange = function () {
                if (xhr.readyState === 4) {
                    if (xhr.status < 400) {
                        cb(null, xhr.responseText, xhr.status);
                    }
                    else {
                        var err = {
                            status: xhr.status,
                            message: enums.ERestMethodUtils.errorCode(xhr.status)
                        };
                        cb(err, null, xhr.status);
                    }
                }
            };
        }
        if (authorization) {
            xhr.setRequestHeader('Authorization', authorization);
        }
        if (data) {
            xhr.dataType = 'json';
            xhr.send(JSON.stringify(data));
        }
        else {
            xhr.send();
        }
    }
    getBasicAuthorization(username, password) {
        return 'Basic ' + base64Encode(username + ':' + password);
    }
    getBearerAuthorization(token) {
        return 'Bearer ' + token;
    }
    get(path, authorization, cb) {
        return this._request("GET", path, authorization, null, cb);
    }
    post(path, authorization, data, cb) {
        return this._request("POST", path, authorization, data, cb);
    }
    patch(path, authorization, data, cb) {
        return this._request("PATCH", path, authorization, data, cb);
    }
    put(path, authorization, data, cb) {
        return this._request("PUT", path, authorization, data, cb);
    }
    delete(path, authorization, data, cb) {
        return this._request("DELETE", path, authorization, data, cb);
    }
}
//# sourceMappingURL=http.js.map