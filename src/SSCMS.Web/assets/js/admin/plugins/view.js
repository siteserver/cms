var $url = '/plugins/view';

var data = utils.initData({
  pluginId: utils.getQueryString('pluginId'),
  returnUrl: utils.getQueryString('returnUrl'),
  isNightly: null,
  version: null,
  installed: null,
  installedVersion: null,
  package: null,
  isShouldUpdate: false,
  plugin: {},
  release: {},
  user: {}
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url, {
      params: {
        pluginId: this.pluginId
      }
    }).then(function (response) {
      var res = response.data;

      $this.isNightly = res.isNightly;
      $this.version = res.version;
      $this.installed = res.installed;
      $this.installedVersion = res.installedVersion;
      $this.package = res.package || {};

      $apiCloud.get('plugins/' + this.pluginId, {
        params: {
          isNightly: $this.isNightly,
          version: $this.version
        }
      }).then(function (response) {
        var res = response.data;

        $this.plugin = res.value.plugin;
        $this.release = res.value.release;
        $this.user = res.value.user;

        $this.isShouldUpdate = utils.compareVersion($this.installedVersion, $this.release.version) == -1;
      }).then(function () {
        utils.loading($this, false);
      });

    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  getIconUrl: function (url) {
    if (url && url.indexOf('://') !== -1) return url;
    return 'https://www.siteserver.cn/plugins/' + url;
  },

  getTagNames: function (plugin) {
    var tagNames = [];
    if (plugin.tags) {
      tagNames = plugin.tags.split(',');
    }
    return tagNames;
  },

  btnReturn: function () {
    location.href = this.returnUrl;
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});