var $url = '/plugins/install';
var $urlActionsDownload = $url + '/actions/download';
var $urlActionsUpdate = $url + '/actions/update';
var $urlActionsRestart = $url + '/actions/restart';

var data = utils.init({
  pluginIds: _.split(utils.getQueryString('pluginIds'), ','),
  pageType: utils.getQueryBoolean('isUpdate') ? '升级' : '安装',
  active: 0,
  success: false,
  isNightly: false,
  version: null,

  listPackages: [],
  listPackageIds: [],
  listIndex: 0,

  currentPackage: {},
  currentPackages: [],
  currentDownloadingId: 0,
  currentDownloadIds: [],
  currentUpdatingId: 0,
  currentUpdatedIds: []
});

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.isNightly = res.isNightly;
      $this.version = res.version;

      $this.getPackages();
    }).catch(function (error) {
      utils.error(error);
    });
  },

  apiDownload: function (pluginId, version) {
    var $this = this;

    $api.post($urlActionsDownload, {
      pluginId: pluginId,
      version: version
    }).then(function (response) {
      var res = response.data;

      $this.currentDownloadingId = 0;
      $this.currentDownloadIds.push(pluginId);
      $this.download();
    }).catch(function (error) {
      utils.error(error);
    });
  },

  apiUpdate: function (pluginId, version, packageType) {
    var $this = this;

    $api.post($urlActionsUpdate, {
      pluginId: pluginId,
      version: version,
      packageType: packageType
    }).then(function (response) {
      var res = response.data;

      $this.currentUpdatingId = 0;
      $this.currentUpdatedIds.push(pluginId);
      $this.update();
    }).catch(function (error) {
      utils.error(error);
    });
  },

  apiRestart: function (callback) {
    var $this = this;

    $api.post($urlActionsRestart).then(function (response) {
      if (callback) {
        callback();
      } else {
        utils.alertSuccess({
          title: '插件' + $this.pageType + '成功',
          text: '插件' + $this.pageType + '成功，系统需要重载页面',
          callback: function() {
            window.top.location.reload(true);
          }
        });
      }
    }).catch(function (error) {
      utils.error(error);
    });
  },

  getPackages: function () {
    var $this = this;

    cloud.getUpdates($this.isNightly, $this.version, $this.pluginIds).then(function (response) {
      var res = response.data;

      var plugins = res.plugins;

      for (var i = 0; i < plugins.length; i++) {
        var release = plugins[i];

        // for (var j = 0; j < release.pluginReferences.length; j++) {
        //   var reference = release.pluginReferences[j];

        //   if ($this.listPackageIds.indexOf(reference.id) === -1) {
        //     $this.listPackageIds.push(reference.id);
        //     $this.listPackages.push({
        //       id: reference.id,
        //       version: reference.version,
        //       packageType: 'Plugin'
        //     });
        //   }
        // }

        // for (var k = 0; k < release.libraryReferences.length; k++) {
        //   var reference = release.libraryReferences[k];
        //   if ($this.listPackageIds.indexOf(reference.id) === -1) {
        //     $this.listPackageIds.push(reference.id);
        //     $this.listPackages.push({
        //       id: reference.id,
        //       version: reference.version,
        //       packageType: 'Library'
        //     });
        //   }
        // }

        if ($this.listPackageIds.indexOf(release.pluginId) === -1) {
          $this.listPackageIds.push(release.pluginId);
          $this.listPackages.push({
            id: release.pluginId,
            version: release.version,
            packageType: 'Plugin'
          });
        }
      }

      $this.apiRestart(function() {
        setTimeout(function() {
          $this.installListPackage();
        }, 3000);
      });
      
    }).catch(function (error) {
      utils.error(error);
    });
  },

  installListPackage: function () {
    var $this = this;

    if ($this.listIndex === $this.listPackages.length) {
      this.apiRestart();
      return;
    }

    $this.package = $this.listPackages[$this.listIndex];
    $this.currentPackages.push($this.package);

    $this.download();
  },

  download: function () {
    var $this = this;

    for (var i = 0; i < $this.currentPackages.length; i++) {
      var package = $this.currentPackages[i];
      if ($this.currentDownloadIds.indexOf(package.id) == -1) {
        $this.currentDownloadingId = package.id;
        $this.apiDownload(package.id, package.version)
        return;
      }
    }

    $this.update();
  },

  update: function () {
    var $this = this;
    $this.active = 1;

    for (var i = 0; i < $this.currentPackages.length; i++) {
      var package = $this.currentPackages[i];
      if ($this.currentUpdatedIds.indexOf(package.id) == -1) {
        $this.currentUpdatingId = package.id;
        $this.apiUpdate(package.id, package.version, package.packageType);
        return;
      }
    }

    $this.updateSuccess();
  },

  updateSuccess: function () {
    var $this = this;

    $this.listIndex++;
    $this.active = 0;

    $this.currentPackage = {};
    $this.currentPackages = [];
    $this.currentDownloadingId = 0;
    $this.currentDownloadIds = [];
    $this.currentUpdatingId = 0;
    $this.currentUpdatedIds = [];

    $this.installListPackage();
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.loading(this, true);
    this.apiGet();
  }
});