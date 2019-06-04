var $url = '/pages/upgrade';
var $packageId = 'SS.CMS';

var $data = {
  pageLoad: false,
  pageAlert: null,
  step: 1,
  package: {},
  isCheck: false,
  isShouldUpdate: false,
  installedVersion: null,
  isNightly: null,
  version: null,
  updatesUrl: ''
};

var $methods = {
  load: function () {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.installedVersion = res.installedVersion;
      $this.isNightly = res.isNightly;
      $this.version = res.version;
      $this.getVersion();
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  getVersion: function () {
    var $this = this;

    $ssApi.get($ssUrlUpdates, {
      params: {
        isNightly: $this.isNightly,
        pluginVersion: $this.version,
        packageIds: $packageId
      }
    }).then(function (response) {
      var res = response.data;
      $this.package = res.value[0];
      $this.isShouldUpdate = compareversion($this.installedVersion, $this.package.version) == -1;
      var major = $this.package.version.split('.')[0];
      var minor = $this.package.version.split('.')[1];
      $this.updatesUrl = ssUtils.getVersionPageUrl(major, minor);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    });
  },

  check: function () {
    this.isCheck = !this.isCheck;
  },

  updateSsCms: function () {
    var $this = this;

    if (!$this.package) return;
    $this.step = 2;

    $api.post($url, {
      version: $this.package.version
    }).then(function (response) {
      var res = response.data;

      $this.step = 3;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    });
  }
};

new Vue({
  el: '#main',
  data: $data,
  methods: $methods,
  created: function () {
    this.load();
  }
});