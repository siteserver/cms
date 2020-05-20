var $url = '/plugins/add';

var data = utils.init({
  isNightly: null,
  version: null,
  packageIds: null,
  q: utils.getQueryString('q'),
  keyword: utils.getQueryString('q') || '',
  plugins: null
});

var methods = {
  getIconUrl: function (relatedUrl) {
    if (_.startsWith(relatedUrl, 'plugins/')) {
      return 'http://sscms-public.oss-accelerate.aliyuncs.com/' + relatedUrl;
    }
    return "https://www.siteserver.cn/plugins/" + relatedUrl;
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
      $this.version = res.version;
      $this.packageIds = res.packageIds;

      $cloud.getPlugins($this.keyword, function(plugins) {
        $this.plugins = plugins;
        utils.loading($this, false);
      });
    }).catch(function (error) {
      utils.error(error);
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