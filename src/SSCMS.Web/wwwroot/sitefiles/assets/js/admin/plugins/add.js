var $url = '/plugins/add';

var data = utils.init({
  version: null,
  packageIds: null,
  q: utils.getQueryString('q'),
  keyword: utils.getQueryString('q') || '',
  plugins: null
});

var methods = {
  getIconUrl: function (iconUrl) {
    return cloud.hostStorage + '/' + iconUrl;
  },

  getTagNames: function (pluginInfo) {
    var tagNames = [];
    if (pluginInfo.tags) {
      tagNames = pluginInfo.tags.split(',');
    }
    return tagNames;
  },

  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      var server = response.headers['server'];
      if (!server || server === 'Kestrel') {
        var docsUrl = cloud.getDocsUrl('getting-started/deploy.html');
        utils.error('页面加载失败，SSCMS 插件需要在进程管理器（Nginx、Apache、IIS、Windows 服务）中运行，请参考文档 <a href="' + docsUrl + '" target="_blank">托管和部署</a>', {
          redirect: true
        });
        return;
      }

      $this.version = res.version;
      $this.packageIds = res.packageIds;

      cloud.getPlugins($this.keyword).then(function (response) {
        var plugins = response.data;
  
        $this.plugins = plugins;
      }).catch(function (error) {
        utils.error(error);
      }).then(function () {
        utils.loading($this, false);
      });
    }).catch(function (error) {
      utils.error(error);
    });
  },

  getLatestVersion: function(plugin) {
    return plugin.latestStableVersion;
  },

  getLatestPublished: function(plugin) {
    return plugin.latestStablePublished;
  },

  btnSearchClick: function () {
    location.href = '?q=' + this.keyword;
  },

  btnUploadClick: function () {
    utils.openLayer({
      title: '离线安装插件',
      url: utils.getPluginsUrl('addLayerUpload'),
      width: 550,
      height: 350
    });
  },

  btnViewClick: function(plugin) {
    utils.addTab('插件：' + plugin.pluginId, utils.getPluginsUrl('view', {pluginId: plugin.pluginId}));
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