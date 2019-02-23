var $url = '/pages/plugins/add';

var $data = {
  pageLoad: false,
  pageAlert: null,
  isNightly: null,
  pluginVersion: null,
  packageIds: null,
  q: utils.getQueryString('q'),
  keyword: utils.getQueryString('q') || '',
  packages: null
};

var $methods = {
  getIconUrl: function (url) {
    return ssUtils.getPluginsUrl(url);
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

      $ssApi.get($ssUrlPlugins, {
        params: {
          isNightly: $this.isNightly,
          pluginVersion: $this.pluginVersion,
          keyword: $this.keyword
        }
      }).then(function (response) {
        var res = response.data;

        $this.packages = res.value;
      }).catch(function (error) {
        $this.pageAlert = utils.getPageAlert(error);
      }).then(function () {
        $this.pageLoad = true;
      });

    }).catch(function (error) {
      $this.pageLoad = true;
      $this.pageAlert = utils.getPageAlert(error);
    });
  },

  btnSearchClick: function () {
    location.href = '?q=' + this.keyword;
  },

  btnUploadClick: function () {
    utils.openLayer({
      title: '离线安装插件',
      url: 'addLayerUpload.cshtml',
      full: true
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  created: function () {
    this.load();
  }
});