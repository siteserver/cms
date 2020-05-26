Object.defineProperty(Object.prototype, "getEntityValue", {
  value: function (t) {
    var e;
    for (e in this) if (e.toLowerCase() == t.toLowerCase()) return this[e];
  },
});

if (window.swal && swal.mixin) {
  var alert = swal.mixin({
    confirmButtonClass: "el-button el-button--primary",
    cancelButtonClass: "el-button el-button--default",
    buttonsStyling: false,
  });
}

var PER_PAGE = 30;
var ADMIN_ACCESS_TOKEN_NAME = "ss_admin_access_token";
var USER_ACCESS_TOKEN_NAME = "ss_user_access_token";

var $type = "admin";

try {
  var scripts = document.getElementsByTagName("script");
  var dataValue = scripts[scripts.length - 1].getAttribute("data-type");
  if (dataValue) $type = dataValue;
} catch (e) {}

var $apiUrl = "/api/admin";
var $rootUrl = "/ss-admin";
var $token =
  sessionStorage.getItem(ADMIN_ACCESS_TOKEN_NAME) ||
  localStorage.getItem(ADMIN_ACCESS_TOKEN_NAME);
if ($type === "user") {
  $apiUrl = "/api/home";
  $rootUrl = "/home";
  $token =
    sessionStorage.getItem(USER_ACCESS_TOKEN_NAME) ||
    localStorage.getItem(USER_ACCESS_TOKEN_NAME);
}

var $api = axios.create({
  baseURL: $apiUrl,
  headers: {
    Authorization: "Bearer " + $token,
  },
});

var $urlCloud = 'https://sscms.com';
var $urlCloudDl = 'https://dl.sscms.com';
var $urlCloudDocs = 'https://sscms.com/docs/v7';

var utils = {
  init: function (data) {
    return _.assign(
      {
        pageLoad: false,
        loading: null,
      },
      data
    );
  },

  getQueryString: function (name, defaultValue) {
    var result = location.search.match(
      new RegExp("[?&]" + name + "=([^&]+)", "i")
    );
    if (!result || result.length < 1) {
      return defaultValue || "";
    }
    return decodeURIComponent(result[1]);
  },

  getQueryStringList: function (name) {
    var value = utils.getQueryString(name);
    if (value) {
      return value.split(",");
    }
    return [];
  },

  getQueryBoolean: function (name) {
    var result = location.search.match(
      new RegExp("[?&]" + name + "=([^&]+)", "i")
    );
    if (!result || result.length < 1) {
      return false;
    }
    return result[1] === "true" || result[1] === "True";
  },

  getQueryInt: function (name, defaultValue) {
    var result = location.search.match(
      new RegExp("[?&]" + name + "=([^&]+)", "i")
    );
    if (!result || result.length < 1) {
      return defaultValue || 0;
    }
    return utils.toInt(result[1]);
  },

  getQueryIntList: function (name) {
    var result = location.search.match(
      new RegExp("[?&]" + name + "=([^&]+)", "i")
    );
    if (!result || result.length < 1) {
      return [];
    }
    return _.map(result[1].split(","), function (x) {
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
      return _.map(value.split(","), function (item) {
        return parseInt(item, 10);
      });
    }
    return [];
  },

  getIndexUrl: function (query) {
    var url = $rootUrl + "/";
    if (query) {
      url += "?";
      _.forOwn(query, function (value, key) {
        url += key + "=" + encodeURIComponent(value) + "&";
      });
      url = url.substr(0, url.length - 1);
    }
    return url;
  },

  getRootUrl: function (name, query) {
    return utils.getPageUrl(null, name, query);
  },

  getAssetsUrl: function (url) {
    return "/sitefiles/assets/" + url;
  },

  getCmsUrl: function (name, query) {
    return utils.getPageUrl("cms", name, query);
  },

  getPluginsUrl: function (name, query) {
    return utils.getPageUrl("plugins", name, query);
  },

  getSettingsUrl: function (name, query) {
    return utils.getPageUrl("settings", name, query);
  },

  getSharedUrl: function (name, query) {
    return utils.getPageUrl("shared", name, query);
  },

  getPageUrl: function (prefix, name, query) {
    var url = $rootUrl + "/";
    if (prefix) {
      url += prefix + "/" + name + "/";
    } else {
      url += name + "/";
    }
    if (query) {
      url += "?";
      _.forOwn(query, function (value, key) {
        url += key + "=" + encodeURIComponent(value) + "&";
      });
      url = url.substr(0, url.length - 1);
    }
    return url;
  },

  getCountName(attributeName) {
    return _.camelCase(attributeName + "Count");
  },

  getExtendName(attributeName, n) {
    return _.camelCase(n ? attributeName + n : attributeName);
  },

  getRootVue: function() {
    return top.$vue;
  },

  getTabVue: function(name) {
    var $this = utils.getRootVue();
    var tab = $this.tabs.find(function(tab) {
      return tab.name == name;
    });
    if (tab) {
      var iframe = top.document.getElementById('frm-' + tab.name).contentWindow;
      return iframe.$vue;
    }
    return null;
  },

  getTabName: function() {
    var $this = utils.getRootVue();
    return $this.tabName;
  },

  openTab: function(name) {
    var $this = utils.getRootVue();
    $this.tabName = name;
  },

  addTab: function(title, url) {
    var $this = utils.getRootVue();
    var index = $this.tabs.findIndex(function(tab) {
      return tab.url == url;
    });
    
    var tab = null;
    if (index === -1) {
      tab = {
        title: title,
        name: utils.uuid(),
        url: url,
      };
      $this.tabs.push(tab);
    } else {
      tab = $this.tabs[index];
      var iframe = top.document.getElementById('frm-' + tab.name).contentWindow;
      iframe.location.reload();
    }
    $this.tabName = tab.name;
  },

  removeTab: function(name) {
    var $this = utils.getRootVue();
    if (!name) {
      name = $this.tabName;
    }
    
    if ($this.tabName === name) {
      $this.activeChildMenu = null;
      $this.tabs.forEach(function(tab, index) {
        if (tab.name === name) {
          var nextTab = $this.tabs[index + 1] || $this.tabs[index - 1];
          if (nextTab) {
            $this.tabName = nextTab.name;
          }
        }
      });
    }
    
    $this.tabs = $this.tabs.filter(function(tab) {
      return tab.name !== name;
    });
  },

  addQuery: function (url, query) {
    if (!url) return '';
    url += (url.indexOf('?') === -1 ? '?' : '&');
    _.forOwn(query, function (value, key) {
      url += key + "=" + encodeURIComponent(value) + "&";
    });
    return url.substr(0, url.length - 1);
  },

  alertDelete: function (config) {
    if (!config) return false;

    alert({
      title: config.title,
      text: config.text,
      type: "warning",
      confirmButtonText: config.button || "删 除",
      confirmButtonClass: "el-button el-button--danger",
      cancelButtonClass: "el-button el-button--default",
      showCancelButton: true,
      cancelButtonText: "取 消",
    }).then(function (result) {
      if (result.value) {
        config.callback();
      }
    });

    return false;
  },

  alertSuccess: function (config) {
    if (!config) return false;

    alert({
      title: config.title,
      text: config.text,
      type: "success",
      confirmButtonText: config.button || "确 定",
      confirmButtonClass: "el-button el-button--primary",
      showCancelButton: false
    }).then(function (result) {
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
      type: "question",
      confirmButtonText: config.button || "确 认",
      confirmButtonClass: "el-button el-button--primary",
      cancelButtonClass: "el-button el-button--default",
      showCancelButton: true,
      cancelButtonText: "取 消",
    }).then(function (result) {
      if (result.value) {
        config.callback();
      }
    });

    return false;
  },

  getErrorMessage: function (error) {
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

  uuid: function() {
    return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(
      /[xy]/g,
      function (c) {
        var r = (Math.random() * 16) | 0,
          v = c == "x" ? r : (r & 0x3) | 0x8;
        return v.toString(16);
      }
    );
  },

  success: function (message) {
    utils.getRootVue().$message({
      type: "success",
      message: message,
      showIcon: true
    });
  },

  error: function (error, options) {
    if (!error) return;

    if (error.response) {
      var message = utils.getErrorMessage(error);

      if (error.response && error.response.status === 500 || options && options.redirect) {
        var uuid = utils.uuid();
        sessionStorage.setItem(uuid, message);
  
        if (options && options.redirect) {
          location.href = utils.getRootUrl("error", { uuid: uuid })
          return;
        }
  
        top.utils.openLayer({
          url: utils.getRootUrl("error", { uuid: uuid }),
        });
        return;
      }

      utils.getRootVue().$message({
        type: "error",
        message: message,
        showIcon: true
      });
    } else if (typeof error === 'string') {
      utils.getRootVue().$message({
        type: "error",
        message: error,
        showIcon: true
      });
    }
  },

  loading: function (app, isLoading) {
    if (isLoading) {
      if (app.pageLoad) {
        app.loading = app.$loading({ text: "页面加载中" });
      }
    } else {
      app.loading ? app.loading.close() : (app.pageLoad = true);
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
      area: [config.width + "px", config.height + "px"],
      maxmin: !config.max,
      resize: !config.max,
      shadeClose: true,
      content: config.url,
    });

    if (config.max) {
      layer.full(index);
    }

    return false;
  },

  contains: function (str, val) {
    return str && val && str.indexOf(val) !== -1;
  },

  validateMobile: function (rule, value,callback) {
    if (!value){
      callback();
    } else if (!/^1[3|4|5|7|8][0-9]\d{8}$/.test(value)){
      callback(new Error(rule.message));
    } else {
      callback()
    }
  },

  getRules: function (rules) {
    var options = [
      { required: "字段为必填项" },
      { numeric: "字段必须仅包含数字" },
      { email: "字段必须是有效的电子邮件" },
      { mobile: "字段必须是有效的手机号码" },
      { url: "字段必须是有效的url" },
      { alpha: "字段只能包含英文字母" },
      { alphaDash: "字段只能包含英文字母、数字、破折号或下划线" },
      { alphaNum: "字段只能包含英文字母或数字" },
      { alphaSpaces: "字段只能包含英文字母或空格" },
      { creditCard: "字段必须是有效的信用卡" },
      { between: "字段必须有一个以最小值和最大值为界的数值" },
      { decimal: "字段必须是数字，并且可能包含指定数量的小数点" },
      { digits: "字段必须是整数，并且具有指定的位数" },
      { included: "字段必须具有指定列表中的值" },
      { excluded: "字段不能具有指定列表中的值" },
      { max: "字段不能超过指定的长度" },
      { maxValue: "字段必须是数值，并且不能大于指定的值" },
      { min: "字段不能低于指定的长度" },
      { minValue: "字段必须是数值，并且不能小于指定的值" },
      { regex: "字段必须匹配指定的正则表达式" },
      { chinese: "字段必须是中文" },
      { currency: "字段必须是货币格式" },
      { zip: "字段必须是邮政编码" },
      { idCard: "字段必须是身份证号码" },
    ];

    if (rules) {
      var array = [];
      for (var i = 0; i < rules.length; i++) {
        var rule = rules[i];
        var ruleType = _.camelCase(rule.type);

        if (ruleType === "required") {
          array.push({
            required: true,
            message: rule.message || options.required,
          });
        } else if (ruleType === "numeric") {
          array.push({
            type: "numeric",
            message: rule.message || options.numeric,
          });
        } else if (ruleType === "email") {
          array.push({ type: "email", message: rule.message || options.email });
        } else if (ruleType === "mobile") {
          array.push({
            validator: utils.validateMobile,
            message: rule.message || options.mobile
          });
        } else if (ruleType === "url") {
          array.push({ type: "url", message: rule.message || options.url });
        } else if (ruleType === "alpha") {
          array.push({ type: "alpha", message: rule.message || options.alpha });
        } else if (ruleType === "alphaDash") {
          array.push({
            type: "alphaDash",
            message: rule.message || options.alphaDash,
          });
        } else if (ruleType === "alphaNum") {
          array.push({
            type: "alphaNum",
            message: rule.message || options.alphaNum,
          });
        } else if (ruleType === "alphaSpaces") {
          array.push({
            type: "alphaSpaces",
            message: rule.message || options.alphaSpaces,
          });
        } else if (ruleType === "creditCard") {
          array.push({
            type: "creditCard",
            message: rule.message || options.creditCard,
          });
        } else if (ruleType === "between") {
          array.push({
            type: "between",
            message: rule.message || options.between,
          });
        } else if (ruleType === "decimal") {
          array.push({
            type: "decimal",
            message: rule.message || options.decimal,
          });
        } else if (ruleType === "digits") {
          array.push({
            type: "digits",
            message: rule.message || options.digits,
          });
        } else if (ruleType === "included") {
          array.push({
            type: "included",
            message: rule.message || options.included,
          });
        } else if (ruleType === "excluded") {
          array.push({
            type: "excluded",
            message: rule.message || options.excluded,
          });
        } else if (ruleType === "max") {
          array.push({ type: "max", message: rule.message || options.max });
        } else if (ruleType === "maxValue") {
          array.push({
            type: "maxValue",
            message: rule.message || options.maxValue,
          });
        } else if (ruleType === "min") {
          array.push({ type: "min", message: rule.message || options.min });
        } else if (ruleType === "minValue") {
          array.push({
            type: "minValue",
            message: rule.message || options.minValue,
          });
        } else if (ruleType === "regex") {
          array.push({ type: "regex", message: rule.message || options.regex });
        } else if (ruleType === "chinese") {
          array.push({
            type: "chinese",
            message: rule.message || options.chinese,
          });
        } else if (ruleType === "currency") {
          array.push({
            type: "currency",
            message: rule.message || options.currency,
          });
        } else if (ruleType === "zip") {
          array.push({ type: "zip", message: rule.message || options.zip });
        } else if (ruleType === "idCard") {
          array.push({
            type: "idCard",
            message: rule.message || options.idCard,
          });
        }
      }
      return array;
    }
    return null;
  },

  compareVersion: function (currentVersion, newVersion, options) {
    // var v1 = (currentVersion || "").split("-")[0];
    // var v2 = (newVersion || "").split("-")[0];
    var v1 = (currentVersion || '').replace(/[^0-9\.]+/g, ".");
    var v2 = (newVersion || '').replace(/[^0-9\.]+/g, ".");

    var lexicographical = options && options.lexicographical,
      zeroExtend = options && options.zeroExtend,
      v1parts = v1.split("."),
      v2parts = v2.split(".");

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

    //1 >, -1 <, 0 ==
    return 0;
  },
};
