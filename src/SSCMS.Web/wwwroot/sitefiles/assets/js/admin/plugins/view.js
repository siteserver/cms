var $url = '/plugins/view';
var $urlActionsDisable = $url + '/actions/disable';
var $urlActionsDelete = $url + '/actions/delete';
var $urlActionsRestart = $url + '/actions/restart';

var data = utils.init({
  userName: utils.getQueryString('userName'),
  name: utils.getQueryString('name'),
  pluginId: utils.getQueryString('pluginId'),
  activeName: 'overview',
  cmsVersion: null,
  plugin: null,
  content: null,
  changeLog: null,
  isShouldUpdate: false,
  extension: null,
  release: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url, {
      params: {
        userName: this.userName,
        name: this.name,
        pluginId: this.pluginId
      }
    }).then(function (response) {
      var res = response.data;

      $this.cmsVersion = res.cmsVersion;
      $this.plugin = res.plugin;
      $this.content = res.content;
      $this.changeLog = res.changeLog;

      var publisher = $this.userName;
      var name = $this.name;
      if ($this.plugin) {
        publisher = $this.plugin.publisher;
        name = $this.plugin.name;
      }

      cloud.getExtension($this.cmsVersion, publisher, name).then(function (response) {
        var res = response.data;

        $this.extension = res.extension;
        $this.release = res.release;

        if ($this.plugin) {
          $this.isShouldUpdate = cloud.compareVersion($this.plugin.version, $this.release.version) == -1;
        }
      }).catch(function (error) {
        console.log(error);
      }).then(function () {
        utils.loading($this, false);
      });
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

      var text = plugin.disabled ? '启用' : '禁用';
      $this.apiRestart(function () {
        utils.alertSuccess({
          title: '插件' + text + '成功',
          text: '插件' + text + '成功，系统需要重新加载',
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

  apiRestart: function (callback) {
    $api.post($urlActionsRestart).then(function (response) {
      setTimeout(function () {
        callback();
      }, 30000);
    }).catch(function (error) {
      utils.error(error);
    });
  },

  btnDisablePlugin: function (plugin) {
    var $this = this;
    var text = plugin.disabled ? '启用' : '禁用';

    utils.alertDelete({
      title: text + '插件',
      text: '此操作将会' + text + '“' + plugin.displayName + '”，确认吗？',
      button: plugin.disabled ? '确认启用' : '确认禁用',
      callback: function () {
        $this.apiDisable(plugin);
      }
    });
  },

  btnDeletePlugin: function (plugin) {
    var $this = this;

    utils.alertDelete({
      title: '卸载插件',
      text: '此操作将会卸载插件“' + plugin.displayName + '”，确认吗？',
      button: '确认卸载',
      callback: function() {
        $this.apiDelete(plugin);
      }
    });
  },

  getPluginUrl: function() {
    if (this.extension) {
      return cloud.host + '/plugins/plugin.html?userName=' + encodeURIComponent(this.extension.userName) + '&name=' + encodeURIComponent(this.extension.name);
    }
    return 'javascript:;';
  },

  getIconUrl: function () {
    if (this.plugin) {
      return this.plugin.iconUrl || utils.getAssetsUrl('images/favicon.png');
    } else if (this.extension) {
      return cloud.hostStorage + '/' + _.trim(this.extension.iconUrl, '/');
    }
    return utils.getAssetsUrl('images/favicon.png');
  },

  getTitle: function () {
    if (this.plugin) {
      return this.plugin.displayName;
    } else if (this.extension) {
      return this.extension.displayName;
    }
    return null;
  },

  getPluginId: function () {
    if (this.plugin) {
      return this.plugin.publisher + '.' + this.plugin.name;
    } else if (this.extension && this.release) {
      return this.extension.userName + '.' + this.extension.name;
    }
    return null;
  },

  getVersion: function () {
    if (this.plugin) {
      return this.plugin.version;
    } else if (this.release) {
      return this.release.version;
    }
    return null;
  },

  getDescription: function () {
    if (this.plugin) {
      return this.plugin.description;
    } else if (this.extension) {
      return this.extension.description;
    }
    return null;
  },

  getReadme: function () {
    if (this.plugin) {
      return this.content;
    } else if (this.extension) {
      return this.extension.content;
    }
    return null;
  },

  getChangeLog: function () {
    if (this.plugin) {
      return this.changeLog;
    } else if (this.extension) {
      return this.extension.changeLog;
    }
    return null;
  },

  getTagNames: function (plugin) {
    var tagNames = [];
    if (plugin.tags) {
      tagNames = plugin.tags.split(',');
    }
    return tagNames;
  },

  btnUpdateClick: function() {
    location.href = utils.getPluginsUrl('install', {
      isUpdate: true, 
      pluginIds: this.plugin.pluginId
    });
  },

  btnBuyClick: function() {
    window.open(this.getPluginUrl());
  },

  btnInstallClick: function() {
    location.href = utils.getPluginsUrl('install', {
      pluginIds: this.extension.userName + '.' + this.extension.name
    });
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