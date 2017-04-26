import * as base64 from './utils/base64'
import * as utf8 from './utils/utf8'
import * as models from '../models'
import * as enums from '../enums'
import * as utils from '../utils'

declare var XMLHttpRequest

function base64Encode(text: string): string {
  var bytes = utf8.encode(text)
  return base64.encode(bytes)
}

export function parse(responseText) {
  if (!responseText) {
    return {}
  }
  return JSON.parse(responseText)
}

Date.prototype.toJSON = function () { return this.toLocaleString(); }

function stringify(obj: Object) {
  return JSON.stringify(obj)
}

export class APIRequest {
  constructor(public options: models.Options) {
    this.options.api = options.api
  }

  private _getURL(path: string, data: any, method: string, api: string) {
    var url = path.indexOf('//') >= 0 ? path : api + path
    url += ((/\?/).test(url) ? '&' : '?')
    // Fix #195 about XMLHttpRequest.send method and GET/HEAD request
    if (utils.isObject(data) && utils.indexOf(['GET', 'HEAD'], method) > -1) {
      url += '&' + utils.map(data, function(v, k) {
        return k + '=' + encodeURIComponent(v)
      }).join('&')
    }
    return url + '&' + (new Date()).getTime()
  }

  private _request(method: string, path: string, data: Object, cb?: (err: models.Error, res: Object, status?: number) => void) {
    const xhr = new XMLHttpRequest()
    xhr.open(method, this._getURL(path, data, method, this.options.api), true)
    xhr.withCredentials = true
    if (cb) {
      xhr.onreadystatechange = () => {
        if (xhr.readyState === 4) {
          if (xhr.status < 400) {
            cb(null, parse(xhr.responseText), xhr.status)
          } else {
            var err = parse(xhr.responseText)
            cb({ status: xhr.status, message: err.message || enums.ERestMethodUtils.errorCode(xhr.status) }, null, xhr.status)
          }
        }
      }
    }

    xhr.dataType = 'json'

    xhr.setRequestHeader('Accept', 'application/vnd.siteserver+json; version=1')
    xhr.setRequestHeader('Content-Type', 'application/json;charset=UTF-8')
    if (this.options.username && this.options.password) {
      var authorization = 'Basic ' + base64Encode(this.options.username + ':' + this.options.password)
      xhr.setRequestHeader('Authorization', authorization)
    }
    if (data) {
      xhr.send(stringify(data))
    } else {
      xhr.send()
    }
  }

  public getURL(path: string): string {
    return this._getURL(path, null, "get", this.options.api)
  }

  public get(path: string, data: Object, cb?: (err: models.Error, data: Object, status?: number) => void) {
    return this._request("GET", path, data, cb)
  }

  public post(path: string, data: Object, cb?: (err: models.Error, data: Object, status?: number) => void) {
    return this._request("POST", path, data, cb)
  }

  public patch(path: string, data: Object, cb?: (err: models.Error, data: Object, status?: number) => void) {
    return this._request("PATCH", path, data, cb)
  }

  public put(path: string, data: Object, cb?: (err: models.Error, data: Object, status?: number) => void) {
    return this._request("PUT", path, data, cb)
  }

  public delete(path: string, data: Object, cb?: (err: models.Error, data: Object, status?: number) => void) {
    return this._request("DELETE", path, data, cb)
  }
}

export class WebRequest {
  constructor() { }

  private _getURL(path: string, data: any, method: string) {
    var url = path
    url += ((/\?/).test(url) ? '&' : '?')
    // Fix #195 about XMLHttpRequest.send method and GET/HEAD request
    if (utils.isObject(data) && utils.indexOf(['GET', 'HEAD'], method) > -1) {
      url += '&' + utils.map(data, function(v, k) {
        return k + '=' + v
      }).join('&')
    }
    return url + '&' + (new Date()).getTime()
  }

  private _request(method: string, path: string, authorization: string, data: Object, cb?: (err: models.Error, res: string, status?: number) => void) {
    var xhr = new XMLHttpRequest()
    xhr.open(method, this._getURL(path, data, method), true)
    xhr.withCredentials = true
    if (cb) {
      xhr.onreadystatechange = function() {
        if (xhr.readyState === 4) {
          if (xhr.status < 400) {
            cb(null, xhr.responseText, xhr.status)
          } else {
            var err: models.Error = {
              status: xhr.status,
              message: enums.ERestMethodUtils.errorCode(xhr.status)
            }
            cb(err, null, xhr.status)
          }
        }
      }
    }

    if (authorization) {
      xhr.setRequestHeader('Authorization', authorization)
    }

    if (data) {
      xhr.dataType = 'json'
      xhr.send(JSON.stringify(data))
    } else {
      xhr.send()
    }
  }

  public getBasicAuthorization(username: string, password: string): string {
    return 'Basic ' + base64Encode(username + ':' + password)
  }

  public getBearerAuthorization(token: string): string {
    return 'Bearer ' + token
  }

  public get(path: string, authorization: string, cb: (err: models.Error, data: Object, status?: number) => void) {
    return this._request("GET", path, authorization, null, cb)
  }

  public post(path: string, authorization: string, data: Object, cb: (err: models.Error, data: Object, status?: number) => void) {
    return this._request("POST", path, authorization, data, cb)
  }

  public patch(path: string, authorization: string, data: Object, cb: (err: models.Error, data: Object, status?: number) => void) {
    return this._request("PATCH", path, authorization, data, cb)
  }

  public put(path: string, authorization: string, data: Object, cb: (err: models.Error, data: Object, status?: number) => void) {
    return this._request("PUT", path, authorization, data, cb)
  }

  public delete(path: string, authorization: string, data: Object, cb: (err: models.Error, data: Object, status?: number) => void) {
    return this._request("DELETE", path, authorization, data, cb)
  }
}
