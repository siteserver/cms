var config = {
  apiUrl: '../api'
};

var alert = swal.mixin({
  confirmButtonClass: 'btn btn-primary',
  cancelButtonClass: 'btn btn-default ml-3',
  buttonsStyling: false,
});

VeeValidate.Validator.localize('zh_CN');
Vue.use(VeeValidate);
VeeValidate.Validator.localize({
  zh_CN: {
    messages: {
      required: function (name) {
        return name + '不能为空';
      }
    }
  }
});
VeeValidate.Validator.extend('mobile', {
  getMessage: function () {
    return ' 请输入正确的手机号码';
  },
  validate: function (value, args) {
    return (
      value.length == 11 &&
      /^((13|14|15|16|17|18|19)[0-9]{1}\d{8})$/.test(value)
    );
  }
});

var utils = {
  Api: function (path, isRoot) {
    this.apiUrl = utils.getApiUrl(path, isRoot);

    this._getURL = function (url, data, method) {
      url += /\?/.test(url) ? '&' : '?';
      if (typeof data === 'object' && method === 'GET') {
        var pairs = [];
        for (var prop in data) {
          if (data.hasOwnProperty(prop)) {
            var k = encodeURIComponent(prop),
              v = encodeURIComponent(data[prop]);
            pairs.push(k + '=' + v);
          }
        }
        url += '&' + pairs.join('&');
      }
      return (url + '&' + new Date().getTime()).replace('?&', '?');
    };

    this.request = function (method, path, data, cb) {
      var xhr = new XMLHttpRequest();
      xhr.open(method, this._getURL(path, data, method), true);
      xhr.withCredentials = true;
      if (cb) {
        xhr.onreadystatechange = function () {
          if (xhr.readyState === 4) {
            if (xhr.status < 400) {
              cb(null, utils.parse(xhr.responseText), xhr.status);
            } else {
              var err = utils.parse(xhr.responseText);
              cb({
                  status: xhr.status,
                  message: err.message || utils.errorCode(xhr.status)
                },
                null,
                xhr.status
              );
            }
          }
        };
      }

      xhr.dataType = 'json';
      xhr.setRequestHeader(
        'Accept',
        'application/vnd.siteserver+json; version=1'
      );
      xhr.setRequestHeader('Content-Type', 'application/json;charset=UTF-8');
      if (data) {
        xhr.send(JSON.stringify(data));
      } else {
        xhr.send();
      }
    };

    this.get = function (data, cb, path) {
      var url = this.apiUrl;
      if (path) {
        url += '/' + path;
      }
      return this.request('GET', url, data, cb);
    };

    this.post = function (data, cb, path) {
      var url = this.apiUrl;
      if (path) {
        url += '/' + path;
      }
      return this.request('POST', url, data, cb);
    };

    this.put = function (data, cb, path) {
      var url = this.apiUrl;
      if (path) {
        url += '/' + path;
      }
      return this.request('PUT', url, data, cb);
    };

    this.delete = function (data, cb, path) {
      var url = this.apiUrl;
      if (path) {
        url += '/' + path;
      }
      return this.request('DELETE', url, data, cb);
    };

    this.patch = function (data, cb, path) {
      var url = this.apiUrl;
      if (path) {
        url += '/' + path;
      }
      return this.request('PATCH', url, data, cb);
    };
  },

  getApiUrl: function (path, isRoot) {
    var apiUrl = _.trimEnd(config.apiUrl, '/');
    if (!isRoot && apiUrl.indexOf('..') !== -1) {
      apiUrl = '../' + apiUrl;
    }
    apiUrl += '/' + _.trimStart(path, '/');
    return apiUrl;
  },

  parse: function (responseText) {
    try {
      return responseText ? JSON.parse(responseText) : {};
    } catch (e) {
      return {};
    }
  },

  errorCode: function (status) {
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
  },

  getQueryString: function (name) {
    var result = location.search.match(
      new RegExp('[?&]' + name + '=([^&]+)', 'i')
    );
    if (!result || result.length < 1) {
      return '';
    }
    return decodeURIComponent(result[1]);
  },

  getToken: function () {
    return Cookies.get('SS-USER-TOKEN-CLIENT');
  },

  setToken: function (accessToken, expiresAt) {
    Cookies.set('SS-USER-TOKEN-CLIENT', accessToken, {
      expires: new Date(expiresAt)
    });
  },

  removeToken: function () {
    Cookies.remove('SS-USER-TOKEN-CLIENT');
  },

  redirectLogin: function () {
    if (location.hash) {
      location.href = 'pages/login.html';
    } else {
      top.location.hash = 'pages/login.html';
    }
  },

  loading: function (isLoading) {
    if (isLoading) {
      return layer.load(1, {
        shade: [0.2, '#000']
      });
    } else {
      layer.close(layer.index);
    }
  },

  scrollToTop: function () {
    document.documentElement.scrollTop = document.body.scrollTop = 0;
  },

  closeLayer: function () {
    parent.layer.closeAll();
    return false;
  },

  openLayer: function (config) {
    if (!config || !config.url) return false;

    if (!config.width) {
      config.width = $(window).width() - 50;
    }
    if (!config.height) {
      config.height = $(window).height() - 50;
    }

    if (config.full) {
      config.width = $(window).width() - 50;
      config.height = $(window).height() - 50;
    }

    layer.open({
      type: 2,
      btn: null,
      title: config.title,
      area: [config.width + 'px', config.height + 'px'],
      maxmin: true,
      resize: true,
      shadeClose: true,
      content: config.url
    });

    return false;
  },

  openImagesLayer: function (imageUrls) {
    var data = [];
    for (var i = 0; i < imageUrls.length; i++) {
      var imageUrl = imageUrls[i];
      data.push({
        src: imageUrl, //原图地址
        thumb: imageUrl //缩略图地址
      });
    }
    layer.photos({
      photos: {
        data: data
      },
      anim: 5
    });
  },

  getConfig: function (params, callback, isRoot) {
    var api = new utils.Api('/home', isRoot);
    if (typeof params === 'string') {
      params = {
        pageName: params
      };
    }
    api.get(params, function (err, res) {
      if (err) {
        api.get(params, function (err, res) {
          if (err) return utils.alertError(err);
          if (res.config.isHomeClosed) {
            alert({
              title: '用户中心已关闭！',
              type: 'error',
              showConfirmButton: false,
              allowOutsideClick: false,
              allowEscapeKey: false
            });
          }
          callback(res);
        });
      }
      if (res.config.isHomeClosed) {
        alert({
          title: '用户中心已关闭！',
          text: ' ',
          type: 'error',
          showConfirmButton: false,
          allowOutsideClick: false,
          allowEscapeKey: false
        });
      }
      callback(res);
    });
  },

  alertError: function (err) {
    alert({
      title: '系统错误！',
      text: '请联系管理员协助解决',
      type: 'error',
      showConfirmButton: false,
      allowOutsideClick: false,
      allowEscapeKey: false
    });
  },

  alertDelete: function (config) {
    if (!config) return false;

    alert({
      title: config.title,
      text: config.text,
      type: 'question',
      confirmButtonText: '确认删除',
      confirmButtonClass: 'btn btn-danger',
      showCancelButton: true,
      cancelButtonText: '取 消'
    }).then(function (result) {
      if (result.value) {
        config.callback();
      }
    });

    return false;
  }
};