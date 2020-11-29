var $url = '/plugins/manage';
var $urlActionsDisable = $url + '/actions/disable';
var $urlActionsDelete = $url + '/actions/delete';
var $urlActionsRestart = $url + '/actions/restart';

var data = utils.init({
  pageType: utils.getQueryString("pageType"),
  cmsVersion: null,
  allPlugins: null,
  containerized: null,
  plugins: null,
  enabledPlugins: [],
  disabledPlugins: [],
  errorPlugins: [],
  updatePlugins: [],
  updatePluginIds: []
});

var methods = {
  getIconUrl: function (plugin) {
    return cloud.getPluginIconUrl(plugin);
  },

  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.cmsVersion = res.cmsVersion;
      $this.allPlugins = res.allPlugins;
      $this.containerized = res.containerized;

      var server = response.headers['server'];
      if (!$this.containerized && (!server || server === 'Kestrel')) {
        var docsUrl = cloud.getDocsUrl('getting-started/deploy.html');
        utils.error('页面加载失败，SSCMS 插件需要在进程管理器（Nginx、Apache、IIS、Windows 服务）中运行，请参考文档 <a href="' + docsUrl + '" target="_blank">托管和部署</a>', {
          redirect: true
        });
        return;
      }

      for (var i = 0; i < $this.allPlugins.length; i++) {
        var plugin = $this.allPlugins[i];
        if (plugin.disabled) {
          $this.disabledPlugins.push(plugin);
        } else {
          if (plugin.success) {
            $this.enabledPlugins.push(plugin);
          } else {
            $this.errorPlugins.push(plugin);
          }
        }
      }

      var pluginIds = $this.enabledPlugins.map(function(x) { return x.pluginId });

      cloud.getUpdates($this.cmsVersion, pluginIds).then(function (response) {
        var res = response.data;
  
        var releases = res.releases;
        for (var i = 0; i < releases.length; i++) {
          var release = releases[i];

          var installedPlugins = $.grep($this.enabledPlugins, function (e) {
            return e.pluginId == release.userName + '.' + release.name;
          });
          if (installedPlugins.length == 1) {
            var installedPlugin = installedPlugins[0];
            installedPlugin.updatePlugin = release;

            if (installedPlugin && installedPlugin.version) {
              if (cloud.compareVersion(installedPlugin.version, release.version) == -1) {
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
      $this.btnNavSelect($this.pageType || 'enabled');
      utils.loading($this, false);
    });
  },

  apiRestart: function (callback) {
    utils.loading(this, true);
    $api.post($urlActionsRestart).then(function (response) {
      setTimeout(function() {
        if (callback) {
          callback();
        } else {
          utils.alertSuccess({
            title: '插件重新加载成功',
            text: '插件重新加载成功，系统需要重载页面',
            callback: function() {
              window.top.location.reload(true);
            }
          });
        }
      }, 30000);
    }).catch(function (error) {
      utils.error(error);
    });
  },

  apiDisable: function (plugin) {
    var $this = this;

    utils.loading(this, true);
    $api.post($urlActionsDisable, {
      pluginId: plugin.pluginId,
      disabled: !plugin.disabled
    }).then(function (response) {
      var res = response.data;
      $this.plugins.splice($this.plugins.indexOf(plugin), 1);

      var text = plugin.disabled ? '启用' : '禁用';
      $this.apiRestart(function() {
        utils.alertSuccess({
          title: '插件' + text + '成功',
          text: '插件' + text + '成功，系统需要重载页面',
          callback: function() {
            window.top.location.reload(true);
          }
        });
      });
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDelete: function(plugin) {
    var $this = this;

    utils.loading($this, true);
    $api.post($urlActionsDisable, {
      pluginId: plugin.pluginId,
      disabled: true
    }).then(function (response) {
      var res = response.data;

      $this.apiRestart(function () {
        $api.post($urlActionsDelete, {
          pluginId: plugin.pluginId
        }).then(function (response) {
          $this.apiRestart(function () {
            utils.alertSuccess({
              title: '插件卸载成功',
              text: '插件卸载成功，系统需要重载页面',
              callback: function() {
                window.top.location.reload(true);
              }
            });
          });
        }).catch(function (error) {
          utils.error(error);
        }).then(function () {
          utils.loading($this, false);
        });
      });
      
    }).catch(function (error) {
      utils.error(error);
    });
  },

  btnMenuClick: function(index) {
    var ids = index.split(':');
    var plugin = this.plugins.find(function (x) { return x.pluginId === ids[0]; });
    if (!plugin) return;

    var command = ids[1];
    var $this = this;

    if (command === 'config') {
      utils.addTab('插件配置：' + plugin.pluginId, utils.getPluginsUrl('config', {pluginId: plugin.pluginId}));
    } else if (command === 'enable') {
      utils.alertDelete({
        title: '启用插件',
        text: '此操作将会启用“' + plugin.displayName + '”，确认吗？',
        button: '确认启用',
        callback: function () {
          $this.apiDisable(plugin);
        }
      });
    } else if (command === 'disable') {
      utils.alertDelete({
        title: '禁用插件',
        text: '此操作将会禁用“' + plugin.displayName + '”，确认吗？',
        button: '确认禁用',
        callback: function () {
          $this.apiDisable(plugin);
        }
      });
    } else if (command === 'uninstall') {
      utils.alertDelete({
        title: '卸载插件',
        text: '此操作将会卸载插件“' + plugin.displayName + '”，确认吗？',
        button: '确认卸载',
        callback: function() {
          $this.apiDelete(plugin);
        }
      });
    }

    return false;
  },

  btnNavSelect: function(key) {
    this.pageType = key;
    if (this.pageType == 'enabled') {
      this.plugins = this.enabledPlugins;
    } else if (this.pageType == 'disabled') {
      this.plugins = this.disabledPlugins;
    } else if (this.pageType == 'error') {
      this.plugins = this.errorPlugins;
    } else if (this.pageType == 'update'){
      this.plugins = this.updatePlugins;
    }
  },

  getPageTitle: function() {
    if (this.pageType == 'enabled') {
      return '已启用';
    } else if (this.pageType == 'disabled') {
      return '已禁用';
    } else if (this.pageType == 'error') {
      return '运行错误';
    } else if (this.pageType == 'update') {
      return '发现新版本';
    }
    return '';
  },

  btnRestartClick: function () {
    this.apiRestart();
  },

  btnViewClick: function(displayName, userName, name) {
    utils.addTab(displayName, utils.getPluginsUrl('view', {
      userName: userName,
      name: name
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