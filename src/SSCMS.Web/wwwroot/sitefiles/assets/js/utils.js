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

  loadEditors: function (styles, form) {
    setTimeout(function () {
      for (var i = 0; i < styles.length; i++) {
        var style = styles[i];
        if (style.inputType === 'TextEditor') {
          UE.delEditor(style.attributeName);
          var editor = utils.getEditor(style.attributeName);
          editor.attributeName = style.attributeName;
          editor.ready(function () {
            this.addListener("contentChange", function () {
              form[this.attributeName] = this.getContent();
            });
          });
        }
      }
    }, 100);
  },

  getEditor: function (attributeName) {
    return UE.getEditor(attributeName, {
      allowDivTransToP: false,
      maximumWords: 99999999,
      initialFrameWidth:null ,
      autoHeightEnabled: false,
      autoFloatEnabled: false
    });
  },

  toCamelCase: function (s) {
    if (!s || s[0] !== s[0].toUpperCase()) {
      return s;
    }
    var chars = s.split('');
    var values = s.split('');
    for (var i = 0; i < chars.length; i++) {
      if (i == 1 && chars[i] !== chars[i].toUpperCase()) {
        return values.join('');
      }
      var hasNext = (i + 1) < chars.length;
      if (i > 0 && hasNext && chars[i + 1] !== chars[i + 1].toUpperCase()) {
        return values.join('');
      }
      values[i] = _.toLower(chars[i]);
    }
    return values.join('');
  },

  toInt: function (val) {
    if (!val) return 0;
    if (typeof val === 'number') return val;
    return parseInt(val, 10) || 0;
  },

  formatDate: function(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) 
        month = '0' + month;
    if (day.length < 2) 
        day = '0' + day;

    return [year, month, day].join('-');
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

  getWxUrl: function (name, query) {
    return utils.getPageUrl("wx", name, query);
  },

  getPluginsUrl: function (name, query) {
    return utils.getPageUrl("plugins", name, query);
  },

  getSettingsUrl: function (name, query) {
    return utils.getPageUrl("settings", name, query);
  },

  getCommonUrl: function (name, query) {
    return utils.getPageUrl("common", name, query);
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

  getCountName: function(attributeName) {
    return _.camelCase(attributeName + "Count");
  },

  getExtendName: function(attributeName, n) {
    return _.camelCase(n ? attributeName + n : attributeName);
  },

  pad: function(num) {
    var s = num+"";
    while (s.length < 2) s = "0" + s;
    return s;
  },

  getUrl: function(siteUrl, url) {
    if (url && (url.startsWith('/') || url.indexOf('://') != -1)) return url;
    siteUrl = _.trimEnd(siteUrl, '/');
    return siteUrl + '/' + _.trimStart(_.trimStart(_.trimStart(url, '~'), '@'), '/');
  },

  getFriendlyDate: function(date) {
    if (Object.prototype.toString.call(date) !== '[object Date]') {
      date = new Date(date);
    }
    var delta = Math.round((new Date() - date) / 1000);
    var minute = 60, hour = minute * 60, day = hour * 24;
    if (delta < day) {
      return utils.pad(date.getHours()) + ':' + utils.pad(date.getMinutes());
    }
    if (delta < day * 2) {
      return '昨天 ' + utils.pad(date.getHours()) + ':' + utils.pad(date.getMinutes());
    }
    return utils.pad(date.getMonth() + 1) + '月' + utils.pad(date.getDate()) + '日';
  },

  getRootVue: function() {
    return top.$vue || window.$vue;
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
    var index = $this.tabs.findIndex(function(tab) {
      return tab.name == name;
    });
    if (index !== -1) {
      $this.tabName = name;
    }
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
      iframe.location.href = url;
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

  notifySuccess: function (message, position) {
    utils.getRootVue().$notify.success({
      title: '成功',
      message: message,
      position: position || 'top-right'
    });
  },

  notifyWarning: function (message, position) {
    utils.getRootVue().$notify.warning({
      title: '警告',
      message: message,
      position: position || 'top-right'
    });
  },

  notifyInfo: function (message, position) {
    utils.getRootVue().$notify.info({
      title: '提示',
      message: message,
      position: position || 'top-right'
    });
  },

  notifyError: function (error, position) {
    if (!error) return;

    var message = '';
    if (error.response) {
      message = utils.getErrorMessage(error);
    } else if (typeof error === 'string') {
      message = error;
    } else {
      message = error + '';
    }

    utils.getRootVue().$notify.error({
      title: '错误',
      message: message,
      position: position || 'top-right'
    });
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

        if (typeof message === 'string') {
          sessionStorage.setItem(uuid, JSON.stringify({
            message: message
          }));
        } else {
          sessionStorage.setItem(uuid, message);
        }
  
        if (options && options.redirect) {
          location.href = utils.getRootUrl("error", { uuid: uuid })
          return;
        }
  
        top.utils.openLayer({
          url: utils.getRootUrl("error", { uuid: uuid }),
        });
        return;
      } else if (error.response && (error.response.status === 401 || error.response.status === 403)) {
        var location = _.trimEnd(window.location.href, '/');
        if (_.endsWith(location, '/ss-admin') || _.endsWith(location, '/home')) {
          top.location.href = utils.getRootUrl('login');
        } else {
          top.location.href = utils.getRootUrl('login', {status: 401});
        }
      }

      utils.getRootVue().$message({
        type: "error",
        message: message,
        showIcon: true
      });
    } else if (typeof error === 'string') {
      if (options && options.redirect) {
        var uuid = utils.uuid();
        sessionStorage.setItem(uuid, JSON.stringify({
          message: error
        }));
  
        location.href = utils.getRootUrl("error", { uuid: uuid });
      } else {
        utils.getRootVue().$message({
          type: "error",
          message: error,
          showIcon: true
        });
      }
    } else if (typeof error === 'object') {
      utils.getRootVue().$message({
        type: "error",
        message: error + '',
        showIcon: true
      });
    }
  },

  loading: function (app, isLoading) {
    if (isLoading) {
      if (app.pageLoad) {
        app.loading = app.$loading({ text: '页面加载中' });
      }
    } else {
      app.loading ? app.loading.close() : (app.pageLoad = true);
    }
  },

  scrollTop: function () {
    document.documentElement.scrollTop = document.body.scrollTop = 0;
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
      config.width = ($(window).width() - 50) + 'px';
    } else {
      var width = config.width + '';
      if (width.indexOf('%') == -1 && width.indexOf('px') == -1) {
        config.width = width + 'px';
      }
    }
    if (!config.height) {
      config.height = ($(window).height() - 50) + 'px';
    } else {
      var height = config.height + '';
      if (height.indexOf('%') == -1 && height.indexOf('px') == -1) {
        config.height = height + 'px';
      }
    }

    var index = layer.open({
      type: 2,
      btn: null,
      title: config.title,
      area: [config.width, config.height],
      maxmin: !config.max,
      resize: !config.max,
      shadeClose: true,
      content: config.url,
      success: config.success
    });

    if (config.max) {
      layer.full(index);
    }

    return false;
  },

  contains: function (str, val) {
    return str && val && str.indexOf(val) !== -1;
  },

  validateMobile: function (rule, value, callback) {
    if (!value) {
      callback();
    } else if (!/^1[3|4|5|7|8][0-9]\d{8}$/.test(value)) {
      callback(new Error(rule.message || '字段必须是有效的手机号码'));
    } else {
      callback()
    }
  },

  validateInt: function (rule, value, callback) {
    if (!value) {
      callback();
    } else if (!/^[-]?\d+$/.test(value)) {
      callback(new Error(rule.message || '字段必须是有效的数字值'));
    } else {
      callback()
    }
  },

  getForm: function(styles, value) {
    var form =  _.assign({}, value);
    for (var i = 0; i < styles.length; i++) {
      var style = styles[i];
      var name = utils.toCamelCase(style.attributeName);
      if (style.inputType === 'TextEditor') {
        setTimeout(function () {
          var editor = utils.getEditor(style.attributeName);
          editor.attributeName = style.attributeName;
          editor.ready(function () {
            this.addListener("contentChange", function () {
              $this.form[this.attributeName] = this.getContent();
            });
          });
        }, 100);
      } else if (style.inputType === 'CheckBox' || style.inputType === 'SelectMultiple') {
        if (!form[name] || !Array.isArray(form[name])) {
          form[name] = [];
        }
      }
    }
    return form;
  },

  getRules: function (rules) {
    var options = [
      { required: "字段为必填项" },
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
        } else if (ruleType === "email") {
          array.push({ 
            type: "email", 
            message: rule.message || options.email 
          });
        } else if (ruleType === "mobile") {
          array.push({
            validator: utils.validateMobile,
            message: rule.message || options.mobile
          });
        } else if (ruleType === "url") {
          array.push({ 
            type: "url", 
            message: rule.message || options.url
          });
        } else if (ruleType === "alpha") {
          array.push({ 
            type: "alpha", 
            message: rule.message || options.alpha
          });
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
          array.push({ 
            type: "max", 
            message: rule.message || options.max
          });
        } else if (ruleType === "maxValue") {
          array.push({
            type: "maxValue",
            message: rule.message || options.maxValue,
          });
        } else if (ruleType === "min") {
          array.push({ 
            type: "min", 
            message: rule.message || options.min 
          });
        } else if (ruleType === "minValue") {
          array.push({
            type: "minValue",
            message: rule.message || options.minValue,
          });
        } else if (ruleType === "regex" && rule.value) {
          var re = new RegExp(rule.value, "ig");
          var message = rule.message || options.regex;
          array.push({ 
            validator: function (rule, value, callback) {
              if (!value){
                callback();
              } else if (!re.test(value)){
                callback(new Error(message));
              } else {
                callback()
              }
            },
            message: message
          });
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
          array.push({ 
            type: "zip", 
            message: rule.message || options.zip
          });
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
  }
};

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
var DEFAULT_AVATAR_URL = '/sitefiles/assets/images/default_avatar.png';

var $token = sessionStorage.getItem(ACCESS_TOKEN_NAME) || localStorage.getItem(ACCESS_TOKEN_NAME) || utils.getQueryString('accessToken');
var $api = axios.create({
  baseURL: $apiUrl,
  headers: {
    Authorization: "Bearer " + $token,
  },
});