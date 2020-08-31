var $url = '/plugins/view';
var $urlActionsDisable = $url + '/actions/disable';
var $urlActionsDelete = $url + '/actions/delete';
var $urlActionsRestart = $url + '/actions/restart';

var data = utils.init({
  pluginId: utils.getQueryString('pluginId'),
  activeName: 'overview',
  version: null,
  localPlugin: null,
  content: null,
  changeLog: null,
  isShouldUpdate: false,
  cloudPlugin: null,
  cloudRelease: null,
  cloudUser: null
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

      $this.version = res.version;
      $this.localPlugin = res.localPlugin;
      $this.content = res.content;
      $this.changeLog = res.changeLog;

      cloud.getPlugin($this.pluginId, $this.version).then(function (response) {
        var res = response.data;

        $this.cloudPlugin = res.plugin;
        $this.cloudRelease = res.release;
        $this.cloudUser = res.user;

        if ($this.localPlugin) {
          $this.isShouldUpdate = cloud.compareVersion($this.localPlugin.version, $this.cloudRelease.version) == -1;
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
    if (this.localPlugin && this.localPlugin.homepage) {
      return this.localPlugin.homepage;
    }
    return cloud.getPluginsUrl('plugin.html?id=' + this.pluginId);
  },

  getIconUrl: function () {
    if (this.localPlugin) {
      return this.localPlugin.icon || utils.getAssetsUrl('images/favicon.png');
    } else if (this.cloudPlugin) {
      return this.cloudPlugin.icon || utils.getAssetsUrl('images/favicon.png');
    }
    return null;
  },

  getTitle: function () {
    if (this.localPlugin) {
      return this.localPlugin.displayName;
    } else if (this.cloudPlugin) {
      return this.cloudPlugin.title;
    }
    return null;
  },

  getAuthor: function () {
    if (this.localPlugin) {
      return this.localPlugin.publisher;
    } else if (this.cloudUser) {
      return this.cloudUser.userName;
    }
    return null;
  },

  getVersion: function() {
    if (this.localPlugin) {
      return this.localPlugin.version;
    } else if (this.cloudRelease) {
      return this.cloudRelease.version;
    }
    return null;
  },

  getRepository: function() {
    if (this.localPlugin) {
      return this.localPlugin.repository;
    } else if (this.cloudPlugin) {
      return this.cloudPlugin.projectUrl;
    }
    return null;
  },

  getSummary: function () {
    if (this.localPlugin) {
      return this.localPlugin.description;
    } else if (this.cloudPlugin) {
      return this.cloudPlugin.summary;
    }
    return null;
  },

  getReadme: function () {
    if (this.localPlugin) {
      return this.content;
    } else if (this.cloudPlugin) {
      return this.cloudPlugin.content;
    }
    return null;
  },

  getChangeLog: function () {
    if (this.localPlugin) {
      return this.changeLog;
    } else if (this.cloudPlugin) {
      return this.cloudPlugin.changeLog;
    }
    return null;
  },

  getTagNames: function (plugin) {
    var tagNames = [];
    if (plugin.tags) {
      tagNames = plugin.tags.split(',');
    }
    return tagNames;
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