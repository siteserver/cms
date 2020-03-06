var $url = '/admin/plugins/add';

var data = utils.initData({
  isNightly: null,
  pluginVersion: null,
  packageIds: null,
  q: utils.getQueryString('q'),
  keyword: utils.getQueryString('q') || '',
  packages: null
});

var methods = {
  getIconUrl: function (url) {
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
      $this.packageIds = res.packageIds;

      $apiCloud.get('plugins', {
        params: {
          isNightly: $this.isNightly,
          pluginVersion: $this.pluginVersion,
          keyword: $this.keyword
        }
      }).then(function (response) {
        var res = response.data;

        $this.packages = res.value;
      }).catch(function (error) {
        utils.error($this, error);
      }).then(function () {
        utils.loading($this, false);
      });

    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSearchClick: function () {
    location.href = '?q=' + this.keyword;
  },

  btnUploadClick: function () {
    utils.openLayer({
      title: '离线安装插件',
      url: utils.getPluginsUrl('addLayerUpload'),
      full: true
    });
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