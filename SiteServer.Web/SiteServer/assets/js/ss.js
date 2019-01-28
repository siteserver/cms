// var $ssHost = 'https://api.siteserver.cn/v1.2';
var $ssHost = 'http://api.siteserver-cms.com/v1.2';

var $ssUrlConnectPing = '/connect/actions/ping'
var $ssUrlStatus = '/users/status'
var $ssUrlLogin = '/users/actions/login';
var $ssUrlLogout = '/users/actions/logout';
var $ssUrlCaptchaGet = '/captcha/LOGIN-CAPTCHA';
var $ssUrlCaptchaCheck = '/captcha/LOGIN-CAPTCHA/actions/check';
var $ssUrlUpdates = '/updates';
var $ssUrlPlugins = '/plugins';
var $ssUrlTemplates = '/templates';

var $ssApi = axios.create({
  baseURL: $ssHost,
  withCredentials: true
});

var ssUtils = {
  getTemplatesUrl: function (url) {
    return 'https://templates.siteserver.cn/' + url;
  },

  getDemoUrl: function (url) {
    return 'https://demo.siteserver.cn/' + url;
  },

  getTemplatePageUrl: function (templateId) {
    return 'https://www.siteserver.cn/templates/template.html?id=' + templateId;
  },

  getPluginsUrl: function (url) {
    return 'https://plugins.siteserver.cn/' + url;
  },

  getVersionPageUrl: function (major, minor) {
    return 'https://www.siteserver.cn/updates/v' + major + '_' + minor + '/index.html';
  },
};