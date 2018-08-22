var $ssApi = new apiUtils.Api();

var $apiUrl = $apiConfig.innerApiUrl;
var $api = new apiUtils.Api($apiUrl + '/pages/plugins/install');
var $packageIds = pageUtils.getQueryStringByName('packageIds').split(',');
var $pageType = pageUtils.getQueryStringByName('isUpdate') === 'true' ? '升级' : '安装';

var data = {
  packageIds: $packageIds,
  pageLoad: false,
  pageAlert: null,
  pageType: $pageType,
  pageStep: 1,
  isNightly: false,
  version: null,
  downloadPlugins: null,
  downloadApi: null,
  updateApi: null,
  clearCacheApi: null,

  listPackages: [],
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

    $api.get(null, function (err, res) {
      if (err || !res) return;

      $this.isNightly = res.isNightly;
      $this.version = res.version;
      $this.downloadPlugins = res.downloadPlugins;
      $this.downloadApi = new apiUtils.Api(res.downloadApiUrl);
      $this.updateApi = new apiUtils.Api(res.updateApiUrl);
      $this.clearCacheApi = new apiUtils.Api(res.clearCacheApiUrl);

      $this.getListPackages();
    }, 'config');
  },
  getListPackages: function () {
    var $this = this;

    $ssApi.get({
      isNightly: $this.isNightly,
      version: $this.version,
      $filter: "id in '" + $this.packageIds.join(",") + "'"
    }, function (err, res) {
      if (err || !res || !res.value) return;

      for (var i = 0; i < res.value.length; i++) {
        var package = res.value[i];
        $this.listPackages.push(package);
      }
      $this.pageLoad = true;
      $this.installListPackage();
    }, 'packages');
  },
  installListPackage: function () {
    var $this = this;

    if ($this.listIndex === $this.listPackages.length) {
      $this.clearCache();
      return;
    }

    $this.package = $this.listPackages[$this.listIndex];
    $this.currentPackages.push($this.package);

    var referencePackageIds = [];

    for (var i = 0; i < $this.package.pluginReferences.length; i++) {
      var pluginReference = $this.package.pluginReferences[i];
      var installedPluginReference = $.grep($this.currentPackages, function (e) {
        return e.id == pluginReference.id;
      });
      if (installedPluginReference.length === 0) {
        referencePackageIds.push(pluginReference.id);
      }
    }
    for (var j = 0; j < $this.package.packageReferences.length; j++) {
      var packageReference = $this.package.packageReferences[j];
      var installedPackageReference = $.grep($this.currentPackages, function (e) {
        return e.id == packageReference.id;
      });
      if (installedPackageReference.length === 0) {
        referencePackageIds.push(packageReference.id);
      }
    }
    
    if (referencePackageIds.length === 0) {
      $this.download();
      return;
    }

    $ssApi.get({
      isNightly: $this.isNightly,
      version: $this.version,
      $filter: "id in '" + referencePackageIds.join(",") + "'"
    }, function (err, res) {
      if (err || !res || !res.value) return;

      for (var i = 0; i < res.value.length; i++) {
        var package = res.value[i];
        for (var j = 0; j < $this.downloadPlugins.length; j++) {
          var installedPackage = $this.downloadPlugins[j];
          if (installedPackage === (package.id + '.' + package.version)) {
            $this.currentDownloadIds.push(package.id);
            break;
          }
        }
        $this.currentPackages.push(package);
      }
      $this.download();
    }, 'packages');
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
  downloadPackage: function (packageId, packageVersion) {
    var $this = this;

    $this.downloadApi.post({
      packageId: packageId,
      version: packageVersion
    }, function (err, res) {
      if (err) {
        $this.pageAlert = {
          type: 'error',
          html: err.message
        };
      } else if (res) {
        $this.currentDownloadingId = 0;
        $this.currentDownloadIds.push(packageId);
        $this.download();
      }
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
  updatePackage: function (packageId, packageVersion, packageType) {
    var $this = this;

    $this.updateApi.post({
      packageId: packageId,
      version: packageVersion,
      packageType: packageType
    }, function (err, res) {
      if (err) {
        $this.pageAlert = {
          type: 'error',
          html: err.message
        };
      } else if (res) {
        $this.currentUpdatingId = 0;
        $this.currentUpdatedIds.push(packageId);
        $this.update();
      }
    });
  },
  updateSuccess: function() {
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
    $this.clearCacheApi.post(null, function (err, res) {
      if (err) {
        $this.pageAlert = {
          type: 'error',
          html: err.message
        };
      } else {
        setTimeout(function () {
          window.top.location.reload(true);
        }, 3000);
      }
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods
});

$vue.getConfig();