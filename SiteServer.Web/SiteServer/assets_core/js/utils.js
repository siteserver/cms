Object.defineProperty(Object.prototype,"getValue",{value:function(t){var e;for(e in this)if(e.toLowerCase()==t.toLowerCase())return this[e]}});

if (window.swal && swal.mixin) {
  var alert = swal.mixin({
    confirmButtonClass: 'btn btn-primary',
    cancelButtonClass: 'btn btn-default ml-3',
    buttonsStyling: false,
  });
}

if (window.Vue && window.VeeValidate) {
  VeeValidate.Validator.localize('zh_CN');
  Vue.use(VeeValidate);
  VeeValidate.Validator.localize({
    zh_CN: {
      messages: {
        required: function (name) {
          return name + '不能为空'
        },
      }
    }
  });
  VeeValidate.Validator.extend('mobile', {
    getMessage: function () {
      return " 请输入正确的手机号码"
    },
    validate: function (value, args) {
      return value.length == 11 && /^((13|14|15|16|17|18|19)[0-9]{1}\d{8})$/.test(value)
    }
  });
}

var $api = axios.create({
  baseURL: window.apiUrl || '../api',
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

  getQueryString: function (name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (!result || result.length < 1) {
      return "";
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

  getQueryInt: function (name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (!result || result.length < 1) {
      return 0;
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

  getCountName(attributeName) {
    return _.camelCase(attributeName + '_Count');
  },

  getExtendName(attributeName, n) {
    return _.camelCase(n ? attributeName + '_' + n : attributeName);
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

  error: function (app, error) {
    var message = error.message;
    if (error.response && error.response.data) {
      if (error.response.data.exceptionMessage) {
        message = error.response.data.exceptionMessage;
      } else if (error.response.data.message) {
        message = error.response.data.message;
      }
    }

    app.$message({
      type: 'error',
      message: message
    });
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
  }
};