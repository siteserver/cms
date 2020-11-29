var $url = '/plugins/add';

var data = utils.init({
  cmsVersion: null,
  packageIds: null,
  containerized: null,
  q: utils.getQueryString('q'),
  keyword: utils.getQueryString('q') || '',
  extensionWithReleases: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.cmsVersion = res.cmsVersion;
      $this.packageIds = res.packageIds;
      $this.containerized = res.containerized;

      var server = response.headers['server'];
      if (!$this.containerized && (!server || server === 'Kestrel')) {
        var docsUrl = cloud.getDocsUrl('getting-started/deploy.html');
        utils.error('页面加载失败，SSCMS 插件需要在进程管理器（Nginx、Apache、IIS、Windows 服务）中运行，请参考文档 <a href="' + docsUrl + '" target="_blank">托管和部署</a>', {
          redirect: true
        });
        return;
      }

      cloud.getExtensions($this.cmsVersion, $this.keyword).then(function (response) {
        var res = response.data;
  
        $this.extensionWithReleases = res.extensionWithReleases;
      }).catch(function (error) {
        utils.error(error);
      }).then(function () {
        utils.loading($this, false);
      });
    }).catch(function (error) {
      utils.error(error);
    });
  },

  getIconUrl: function (iconUrl) {
    return cloud.hostStorage + '/' + _.trim(iconUrl, '/');
  },

  isInstalled: function(extension) {
    return this.packageIds.indexOf(extension.userName + '.' + extension.name) !== -1;
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

  btnViewClick: function(item) {
    utils.addTab(item.extension.displayName, utils.getPluginsUrl('view', {
      userName: item.extension.userName,
      name: item.extension.name
    }));
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