Object.defineProperty(Object.prototype,"getEntityValue",{value:function(t){var e;for(e in this)if(e.toLowerCase()==t.toLowerCase())return this[e]}});

if (window.swal && swal.mixin) {
  var alert = swal.mixin({
    confirmButtonClass: 'btn btn-primary',
    cancelButtonClass: 'btn btn-default ml-3',
    buttonsStyling: false,
  });
}

var $api = axios.create({
  baseURL: $apiUrl || '/api',
  withCredentials: true
});

var $urlCloud = 'https://api.siteserver.cn';
var $apiCloud = axios.create({
  baseURL: $urlCloud + '/v1.2',
  withCredentials: true
});

var utils = {
  PER_PAGE: 30,

  initData: function(data) {
    return _.assign({
      pageLoad: false,
      loading: null
    }, data);
  },

  getQueryString: function (name, defaultValue) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (!result || result.length < 1) {
      return defaultValue || '';
    }
    return decodeURIComponent(result[1]);
  },

  getQueryStringList: function (name) {
    var value = utils.getQueryString(name);
    if (value) {
      return value.split(',');
    }
    return [];
  },

  getQueryBoolean: function (name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (!result || result.length < 1) {
      return false;
    }
    return result[1] === 'true' || result[1] === 'True';
  },

  getQueryInt: function (name, defaultValue) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (!result || result.length < 1) {
      return defaultValue || 0;
    }
    return utils.toInt(result[1]);
  },

  getQueryIntList: function (name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (!result || result.length < 1) {
      return [];
    }
    return _.map(result[1].split(','), function (x) {
      return utils.toInt(x);
    });
  },

  toInt: function (val) {
    if (!val) return 0;
    return parseInt(val, 10) || 0;
  },

  getQueryIntList: function (name) {
    var value = utils.getQueryString(name);
    if (value) {
      return _.map(value.split(','), function(item) {
        return parseInt(item, 10);
      });
    }
    return [];
  },

  getIndexUrl: function (query) {
    var url = $adminUrl + '/';
    if (query) {
      url += '?';
      _.forOwn(query, function(value, key) {
        url+= key + '=' + encodeURIComponent(value) + '&'
      });
      url += '_r=' + Math.random();
    }
    return url;
  },

  getRootUrl: function(name, query) {
    return utils.getPageUrl(null, name, query);
  },

  getAssetsUrl: function(url) {
    return $adminUrl + '/assets/' + url;
  },

  getCmsUrl: function(name, query) {
    return utils.getPageUrl('cms', name, query);
  },

  getPluginsUrl: function(name, query) {
    return utils.getPageUrl('plugins', name, query);
  },

  getSettingsUrl: function(name, query) {
    return utils.getPageUrl('settings', name, query);
  },

  getSharedUrl: function(name, query) {
    return utils.getPageUrl('shared', name, query);
  },

  getPageUrl: function (prefix, name, query) {
    var url = $adminUrl + '/'
    if (prefix) {
      url += prefix + '/' + name + '/';
    } else {
      url += name + '/';
    }
    if (query) {
      url += '?';
      _.forOwn(query, function(value, key) {
        url+= key + '=' + encodeURIComponent(value) + '&'
      });
      url += '_r=' + Math.random();
    }
    return url;
  },

  getCountName(attributeName) {
    return _.camelCase(attributeName + 'Count');
  },

  getExtendName(attributeName, n) {
    return _.camelCase(n ? attributeName + n : attributeName);
  },

  alertDelete: function (config) {
    if (!config) return false;

    alert({
      title: config.title,
      text: config.text,
      type: 'warning',
      confirmButtonText: config.button || '删 除',
      confirmButtonClass: 'el-button el-button--danger',
      cancelButtonClass: 'el-button el-button--default',
      showCancelButton: true,
      cancelButtonText: '取 消'
    })
    .then(function (result) {
      if (result.value) {
        config.callback();
      }
    });

    return false;
  },

  alertWarning: function (config) {
    if (!config) return false;

    alert({
      title: config.title,
      text: config.text,
      type: 'question',
      confirmButtonText: config.button || '确 认',
      confirmButtonClass: 'el-button el-button--primary',
      cancelButtonClass: 'el-button el-button--default',
      showCancelButton: true,
      cancelButtonText: '取 消'
    })
    .then(function (result) {
      if (result.value) {
        config.callback();
      }
    });

    return false;
  },

  getErrorMessage: function(error) {
    if (error.response && error.response.status === 500) {
      return JSON.stringify(error.response.data);
    }

    var message = error.message;
    if (error.response && error.response.data) {
      if (error.response.data.exceptionMessage) {
        message = error.response.data.exceptionMessage;
      } else if (error.response.data.message) {
        message = error.response.data.message;
      }
    }

    return message;
  },

  error: function (app, error, options) {
    var message = utils.getErrorMessage(error);

    if (options && options.redirect) {
      location.href = './error/?message=' + encodeURIComponent(message);
      return;
    }

    if (error.response && error.response.status === 500) {
      var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
      });
      sessionStorage.setItem(uuid, message);
      top.utils.openLayer({
        url: utils.getRootUrl('error', {uuid: uuid})
      })
      return;
    }

    app.$message(_.assign({
      type: 'error',
      message: message,
      showIcon: true
    }, options || {}));
  },

  loading: function (app, isLoading) {
    if (isLoading) {
      if (app.pageLoad) {
        app.loading = app.$loading({text: '页面加载中'});
      }
    } else {
      app.loading ? app.loading.close() : app.pageLoad = true;
    }
  },

  closeLayer: function (reload) {
    if (reload) {
      parent.location.reload();
    } else {
      parent.layer.closeAll();
    }
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

    var index = layer.open({
      type: 2,
      btn: null,
      title: config.title,
      area: [config.width + 'px', config.height + 'px'],
      maxmin: !config.max,
      resize: !config.max,
      shadeClose: true,
      content: config.url
    });

    if (config.max) {
      layer.full(index);
    }

    return false;
  },

  contains: function(str, val) {
    return str && val && str.indexOf(val) !== -1;
  },

  getRules: function(rules) {
    if (rules) {
      var array = [];
      for (var i = 0; i < rules.length; i++) {
        var rule = rules[i];
        if (rule.type === 'Required') {
          array.push({ required: true, message: rule.message });
        } else if (rule.type === '') {
          
        }
      }
      return array;
    }
    return null;
  },

  compareVersion: function(currentVersion, newVersion, options) {
    var v1 = (currentVersion || '').split('-')[0];
    var v2 = (newVersion || '').split('-')[0];
  
    var lexicographical = options && options.lexicographical,
      zeroExtend = options && options.zeroExtend,
      v1parts = v1.split('.'),
      v2parts = v2.split('.');
  
    function isValidPart(x) {
      return (lexicographical ? /^\d+[A-Za-z]*$/ : /^\d+$/).test(x);
    }
  
    if (!v1parts.every(isValidPart) || !v2parts.every(isValidPart)) {
      return NaN;
    }
  
    if (zeroExtend) {
      while (v1parts.length < v2parts.length) v1parts.push("0");
      while (v2parts.length < v1parts.length) v2parts.push("0");
    }
  
    if (!lexicographical) {
      v1parts = v1parts.map(Number);
      v2parts = v2parts.map(Number);
    }
  
    for (var i = 0; i < v1parts.length; ++i) {
      if (v2parts.length == i) {
        return 1;
      }
  
      if (v1parts[i] == v2parts[i]) {
        continue;
      } else if (v1parts[i] > v2parts[i]) {
        return 1;
      } else {
        return -1;
      }
    }
  
    if (v1parts.length != v2parts.length) {
      return -1;
    }
  
    return 0;
  }
};