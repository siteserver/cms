var $url = '/pages/plugins/install';

var $pluginIds = pageUtils.getQueryStringByName('pluginIds').split(',');
var $pageType = pageUtils.getQueryStringByName('isUpdate') === 'true' ? '升级' : '安装';

var data = {
  pluginIds: $pluginIds,
  pageLoad: false,
  pageAlert: null,
  pageType: $pageType,
  pageStep: 1,
  isNightly: false,
  pluginVersion: null,
  downloadPlugins: null,

  listPackages: [],
  listPackageIds: [],
  listIndex: 0,

  currentPackage: {},
  currentPackages: [],
  currentDownloadingId: 0,
  currentDownloadIds: [],
  currentUpdatingId: 0,
  currentUpdatedIds: []
};

var methods = {
  getConfig: function () {
    var $this = this;

    $api.get($url + '/config').then(function (response) {
      var res = response.data;

      $this.isNightly = res.isNightly;
      $this.pluginVersion = res.pluginVersion;
      $this.downloadPlugins = res.downloadPlugins;

      $this.getPackages();
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    });
  },

  getPackages: function () {
    var $this = this;

    $apiCloud.get('updates', {
      params: {
        isNightly: $this.isNightly,
        pluginVersion: $this.pluginVersion,
        packageIds: $this.pluginIds.join(",")
      }
    }).then(function (response) {
      var res = response.data;

      for (var i = 0; i < res.value.length; i++) {
        var releaseInfo = res.value[i];

        for (var j = 0; j < releaseInfo.pluginReferences.length; j++) {
          var reference = releaseInfo.pluginReferences[j];

          if ($this.listPackageIds.indexOf(reference.id) === -1) {
            $this.listPackageIds.push(reference.id);
            $this.listPackages.push({
              id: reference.id,
              version: reference.version,
              packageType: 'Plugin'
            });
          }
        }

        for (var k = 0; k < releaseInfo.libraryReferences.length; k++) {
          var reference = releaseInfo.libraryReferences[k];
          if ($this.listPackageIds.indexOf(reference.id) === -1) {
            $this.listPackageIds.push(reference.id);
            $this.listPackages.push({
              id: reference.id,
              version: reference.version,
              packageType: 'Library'
            });
          }
        }

        if ($this.listPackageIds.indexOf(releaseInfo.pluginId) === -1) {
          $this.listPackageIds.push(reference.id);
          $this.listPackages.push({
            id: releaseInfo.pluginId,
            version: releaseInfo.version,
            packageType: 'Plugin'
          });
        }
      }
      $this.installListPackage();
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  installListPackage: function () {
    var $this = this;

    if ($this.listIndex === $this.listPackages.length) {
      $this.clearCache();
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
        $this.downloadPackage(package.id, package.version)
        return;
      }
    }

    $this.update();
  },

  downloadPackage: function (packageId, version) {
    var $this = this;

    $api.post($url + '/download', {
      packageId: packageId,
      version: version
    }).then(function (response) {
      var res = response.data;

      $this.currentDownloadingId = 0;
      $this.currentDownloadIds.push(packageId);
      $this.download();
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    });
  },

  update: function () {
    var $this = this;
    $this.pageStep = 2;

    for (var i = 0; i < $this.currentPackages.length; i++) {
      var package = $this.currentPackages[i];
      if ($this.currentUpdatedIds.indexOf(package.id) == -1) {
        $this.currentUpdatingId = package.id;
        $this.updatePackage(package.id, package.version, package.packageType);
        return;
      }
    }

    $this.updateSuccess();
  },

  updatePackage: function (packageId, version, packageType) {
    var $this = this;

    $api.post($url + '/update', {
      packageId: packageId,
      version: version,
      packageType: packageType
    }).then(function (response) {
      var res = response.data;

      $this.currentUpdatingId = 0;
      $this.currentUpdatedIds.push(packageId);
      $this.update();
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    });
  },

  updateSuccess: function () {
    var $this = this;

    $this.listIndex++;
    $this.pageStep = 1;

    $this.currentPackage = {};
    $this.currentPackages = [];
    $this.currentDownloadingId = 0;
    $this.currentDownloadIds = [];
    $this.currentUpdatingId = 0;
    $this.currentUpdatedIds = [];

    $this.installListPackage();
  },

  clearCache: function () {
    var $this = this;

    $this.pageStep = 0;

    $api.post($url + '/cache').then(function (response) {
      var res = response.data;

      setTimeout(function () {
        window.top.location.reload(true);
      }, 3000);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getConfig();
  }
});