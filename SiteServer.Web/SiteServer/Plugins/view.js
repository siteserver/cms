var pluginId = utils.getQueryString('pluginId');
var returnUrl = utils.getQueryString('returnUrl');
var $url = '/pages/plugins/view/' + pluginId;

var data = {
  pageLoad: false,
  pageAlert: null,
  isNightly: null,
  pluginVersion: null,
  installed: null,
  installedVersion: null,
  package: null,
  isShouldUpdate: false,
  plugin: {},
  release: {},
  user: {}
};

var methods = {
  getIconUrl: function (url) {
    if (url && url.indexOf('://') !== -1) return url;
    return $urlCloudDl + '/plugins/' + url;
  },

  getTagNames: function (plugin) {
    var tagNames = [];
    if (plugin.tags) {
      tagNames = plugin.tags.split(',');
    }
    return tagNames;
  },

  load: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.isNightly = res.isNightly;
      $this.pluginVersion = res.pluginVersion;
      $this.installed = res.installed;
      $this.installedVersion = res.installedVersion;
      $this.package = res.package || {};

      $apiCloud.get('plugins/' + pluginId, {
        params: {
          isNightly: $this.isNightly,
          pluginVersion: $this.pluginVersion
        }
      }).then(function (response) {
        var res = response.data;

        $this.plugin = res.plugin;
        $this.release = res.release;
        $this.user = res.user;

        $this.isShouldUpdate = compareversion($this.installedVersion, $this.release.version) == -1;
      }).catch(function (error) {
        $this.pageAlert = utils.getPageAlert({
          message: '系统在线获取插件信息失败，请检查网络环境是否能够访问外网'
        });
      }).then(function () {
        $this.pageLoad = true;
      });

    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  btnReturn: function () {
    location.href = returnUrl;
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.load();
  }
});