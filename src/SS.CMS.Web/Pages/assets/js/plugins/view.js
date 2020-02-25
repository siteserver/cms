var pluginId = utils.getQueryString('pluginId');
var returnUrl = utils.getQueryString('returnUrl');
var $url = '/admin/plugins/view/' + pluginId;

var data = utils.initData({
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

        $this.pluginInfo = res.value.pluginInfo;
        $this.releaseInfo = res.value.releaseInfo;
        $this.userInfo = res.value.userInfo;

        $this.isShouldUpdate = compareversion($this.installedVersion, $this.releaseInfo.version) == -1;
      }).catch(function (error) {
        utils.error($this, {
          message: '系统在线获取插件信息失败，请检查网络环境是否能够访问外网'
        });
      }).then(function () {
        utils.loading($this, false);
      });

    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
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