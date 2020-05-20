var $url = '/plugins/manage';

var data = utils.init({
  pageType: utils.getQueryString("pageType", "enabled"),
  isNightly: null,
  version: null,
  allPlugins: null,
  enabledPlugins: [],
  disabledPlugins: [],
  updatePlugins: [],
  updatePluginIds: [],
  referencePluginIds: []
});

var methods = {
  getIconUrl: function (url) {
    return 'https://www.siteserver.cn/plugins/' + url;
  },

  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.isNightly = res.isNightly;
      $this.version = res.version;
      $this.allPlugins = res.allPlugins;

      for (var i = 0; i < $this.allPlugins.length; i++) {
        var plugin = $this.allPlugins[i];
        if (plugin.disabled) {
          $this.disabledPlugins.push(plugin);
        } else {
          $this.enabledPlugins.push(plugin);
        }
      }

      var pluginIds = $this.enabledPlugins.map(function(x) { return x.pluginId });

      $cloud.getReleases($this.isNightly, $this.version, pluginIds, function(cms, plugins) {
        for (var i = 0; i < plugins.length; i++) {
          var releaseInfo = plugins[i];

          var installedPlugins = $.grep($this.enabledPlugins, function (e) {
            return e.pluginId == releaseInfo.pluginId;
          });
          if (installedPlugins.length == 1) {
            var installedPlugin = installedPlugins[0];
            installedPlugin.updatePlugin = releaseInfo;

            if (installedPlugin && installedPlugin.version) {
              if (utils.compareVersion(installedPlugin.version, releaseInfo.version) == -1) {
                $this.updatePlugins.push(installedPlugin);
                $this.updatePluginIds.push(installedPlugin.pluginId);
              }
            } else {
              $this.updatePlugins.push(installedPlugin);
              $this.updatePluginIds.push(installedPlugin.pluginId);
            }
          }
        }
      });
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnEnablePlugin: function (pkg) {
    var $this = this;
    
    var text = pkg.isDisabled ? '启用' : '禁用';
    var isReference = this.referencePluginIds.indexOf(pkg.pluginId) !== -1;
    if (isReference) {
      utils.error("无法" + text, "存在其他插件依赖此插件，需要删除依赖插件后才能进行" + text + "操作");
    }
    utils.alertDelete({
      title: text + '插件',
      text: '此操作将会禁用“' + pkg.name + '”，确认吗？',
      type: 'question',
      showCancelButton: true,
      cancelButtonText: '取 消',
      confirmButtonText: pkg.isDisabled ? '启 用' : '禁 用'
    }).then(function (result) {
      if (result.value) {
        utils.loading($this, true);
        $api.post($url + '/' + pkg.pluginId + '/actions/enable').then(function () {
          utils.loading($this, false);
          utils.alertSuccess({
            title: '插件' + text + '成功',
            text: '插件' + text + '成功，系统需要重载页面'
          }).then(function () {
            window.top.location.reload(true);
          });
        });
      }
    });
  },

  btnDeletePlugin: function (pkg) {
    var $this = this;

    var isReference = this.referencePluginIds.indexOf(pkg.pluginId) !== -1;
    if (isReference) {
      return alert("无法删除", "存在其他插件依赖此插件，需要删除依赖插件后才能进行删除操作", "error");
    }
    utils.alertDelete({
        title: '删除插件',
        text: '此操作将会删除插件“' + pkg.name + '”，确认吗？',
        type: 'question',
        showCancelButton: true,
        cancelButtonText: '取 消',
        confirmButtonText: '确认删除'
      })
      .then(function (result) {
        if (result.value) {
          utils.loading($this, true);
          $api.delete($url + '/' + pkg.pluginId).then(function () {
            utils.loading($this, false);
            alert({
                type: 'success',
                title: '插件删除成功',
                text: '插件删除成功，系统需要重载页面',
                confirmButtonText: '重新载入'
              })
              .then(function () {
                window.top.location.reload(true);
              });
          });
        }
      });
  },

  btnNavSelect: function(key) {
    this.pageType = key;
  },

  getPageTitle: function() {
    if (this.pageType == 'enabled') {
      return '已启用';
    } else if (this.pageType == 'disabled') {
      return '已禁用';
    } else if (this.pageType == 'update') {
      return '发现新版本';
    }
    return '';
  },

  btnReloadClick: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/reload').then(function () {
      utils.loading($this, false);
      utils.alertSuccess({
        title: '插件重新加载成功',
        text: '插件重新加载成功，系统需要重载页面',
        callback: function() {
          window.top.location.reload(true);
        }
      });
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