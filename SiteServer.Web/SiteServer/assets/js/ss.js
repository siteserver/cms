var $ssApiUrl = "http://api.siteserver-cms.com/v1.3";
var $ssConsoleUrl = "http://console.siteserver-cms.com";

var $ssUrlStatus = "/users/status";
var $ssUrlLogin = "/users/actions/login";
var $ssUrlLogout = "/users/actions/logout";
var $ssUrlCaptchaGet = "/captcha/LOGIN-CAPTCHA";
var $ssUrlCaptchaCheck = "/captcha/LOGIN-CAPTCHA/actions/check";
var $ssUrlConnections = "/connections";
var $ssUrlUpdates = "/updates";
var $ssUrlPlugins = "/plugins";
var $ssUrlTemplates = "/templates";

var $ssUrlConnect = "/connect";

var $ssApi = axios.create({
  baseURL: $ssApiUrl,
  withCredentials: true
});

var ssUtils = {
  getTemplatesUrl: function (url) {
    return "https://templates.siteserver.cn/" + url;
  },

  getDemoUrl: function (url) {
    return "https://demo.siteserver.cn/" + url;
  },

  getTemplatePageUrl: function (templateId) {
    return "https://www.siteserver.cn/templates/template.html?id=" + templateId;
  },

  getPluginsUrl: function (url) {
    return "https://plugins.siteserver.cn/" + url;
  },

  getPluginsPageUrl: function () {
    return "https://www.siteserver.cn/plugins/";
  },

  getPluginPageUrl: function (pluginId) {
    return "https://www.siteserver.cn/plugins/plugin.html?id=" + pluginId;
  },

  getVersionPageUrl: function (major, minor) {
    return (
      "https://www.siteserver.cn/updates/v" +
      major +
      "_" +
      minor +
      "/index.html"
    );
  },

  getUserPageUrl: function (userName) {
    return "https://www.siteserver.cn/users/index.html?userName=" + userName;
  },

  getConnectUrl: function (guid) {
    return $ssConsoleUrl + "/connect/?guid=" + guid;
  }
};