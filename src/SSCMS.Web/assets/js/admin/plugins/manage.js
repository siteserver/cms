var $url = '/plugins/manage';

var data = utils.initData({
  pageType: utils.getQueryString("pageType", "1"),
  isNightly: null,
  version: null,
  allPlugins: null,
  pluginIds: null,
  enabledPlugins: [],
  disabledPlugins: [],
  errorPlugins: [],
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
      $this.enabledPlugins = res.enabledPlugins;
      $this.pluginIds = res.pluginIds;

      // for (var i = 0; i < $this.allPlugins.length; i++) {
      //   var pkg = $this.allPlugins[i];
      //   if (pkg.isRunnable && pkg.metadata) {
      //     if (pkg.isDisabled) {
      //       $this.disabledPlugins.push(pkg);
      //     } else {
      //       $this.enabledPlugins.push(pkg);
      //     }
      //   } else {
      //     $this.errorPlugins.push(pkg);
      //   }
      // }

      $apiCloud.get('updates', {
        params: {
          isNightly: $this.isNightly,
          version: $this.version,
          pluginIds: $this.pluginIds
        }
      }).then(function (response) {
        var res = response.data;

        for (var i = 0; i < res.value.length; i++) {
          var releaseInfo = res.value[i];

          var installedPlugins = $.grep($this.allPlugins, function (e) {
            return e.id == releaseInfo.pluginId;
          });
          if (installedPlugins.length == 1) {
            var installedPlugin = installedPlugins[0];
            installedPlugin.updatePlugin = releaseInfo;

            if (installedPlugin.metadata && installedPlugin.metadata.version) {
              if (utils.compareVersion(installedPlugin.metadata.version, releaseInfo.version) == -1) {
                $this.updatePlugins.push(installedPlugin);
                $this.updatePluginIds.push(installedPlugin.id);
              }
            } else {
              $this.updatePlugins.push(installedPlugin);
              $this.updatePluginIds.push(installedPlugin.id);
            }
          }
        }

      }).catch(function (error) {
        //utils.error($this, error);
      }).then(function () {
        utils.loading($this, false);
      });

    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  enablePlugin: function (pkg) {
    var $this = this;
    
    var text = pkg.isDisabled ? '启用' : '禁用';
    var isReference = this.referencePluginIds.indexOf(pkg.pluginId) !== -1;
    if (isReference) {
      return swal("无法" + text, "存在其他插件依赖此插件，需要删除依赖插件后才能进行" + text + "操作", "error");
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
          swal({
            type: 'success',
            title: '插件' + text + '成功',
            text: '插件' + text + '成功，系统需要重载页面',
            confirmButtonText: '重新载入'
          }).then(function () {
            window.top.location.reload(true);
          });
        });
      }
    });
  },

  deletePlugin: function (pkg) {
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

  handleSelect: function(key, keyPath) {
    this.pageType = key;
  },

  btnReload: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url + '/actions/reload').then(function () {
      utils.loading($this, false);
      swal({
        type: 'success',
        title: '插件重新加载成功',
        text: '插件重新加载成功，系统需要重载页面',
        confirmButtonText: '重新载入'
      }).then(function () {
        window.top.location.reload(true);
      });
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