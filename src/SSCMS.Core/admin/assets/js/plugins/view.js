var $url = '/admin/plugins/view';

var data = utils.initData({
  pluginId: utils.getQueryString('pluginId'),
  returnUrl: utils.getQueryString('returnUrl'),
  isNightly: null,
  pluginVersion: null,
  installed: null,
  installedVersion: null,
  package: null,
  isShouldUpdate: false,
  pluginInfo: {},
  releaseInfo: {},
  userInfo: {}
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
      $this.pluginVersion = res.pluginVersion;
      $this.installed = res.installed;
      $this.installedVersion = res.installedVersion;
      $this.package = res.package || {};

      $apiCloud.get('plugins/' + this.pluginId, {
        params: {
          isNightly: $this.isNightly,
          pluginVersion: $this.pluginVersion
        }
      }).then(function (response) {
        var res = response.data;

        $this.pluginInfo = res.value.pluginInfo;
        $this.releaseInfo = res.value.releaseInfo;
        $this.userInfo = res.value.userInfo;

        $this.isShouldUpdate = utils.compareVersion($this.installedVersion, $this.releaseInfo.version) == -1;
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

  getTagNames: function (pluginInfo) {
    var tagNames = [];
    if (pluginInfo.tags) {
      tagNames = pluginInfo.tags.split(',');
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