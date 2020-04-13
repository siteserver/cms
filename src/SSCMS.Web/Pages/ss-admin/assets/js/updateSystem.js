var $url = '/admin/updateSystem';

var data = utils.initData({
  packageId: null,
  installedVersion: null,
  isNightly: null,
  version: null,
  
  step: 1,
  package: {},
  isCheck: false,
  isShouldUpdate: false,
  updatesUrl: '',
  errorMessage: null
});

var methods = {
  load: function() {
    var $this = this;

    $api.get($url).then(function (response) {
      var res = response.data;

      $this.packageId = res.packageId;
      $this.installedVersion = res.installedVersion;
      $this.isNightly = res.isNightly;
      $this.version = res.version;

      utils.loading($this, false);

      $this.getVersion();
    }).catch(function (error) {
      utils.loading($this, false);
      utils.error($this, error);
    });
  },

  getVersion: function () {
    var $this = this;

    $apiCloud.get('updates', {
      params: {
        isNightly: this.isNightly,
        pluginVersion: this.version,
        packageIds: this.packageId
      }
    }).then(function (response) {
      var res = response.data;

      $this.package = res.value[0];
      $this.isShouldUpdate = utils.compareVersion($this.installedVersion, $this.package.version) == -1;
      var major = $this.package.version.split('.')[0];
      var minor = $this.package.version.split('.')[1];
      $this.updatesUrl = 'https://www.siteserver.cn/updates/v' + major + '_' + minor + '/index.html';

    }).catch(function (error) {
      utils.error($this, error);
    }).then(function () {
      utils.loading($this, false);
    });

    // ssApi.get({
    //   isNightly: isNightly,
    //   version: version
    // }, function (err, res) {
    //   if (err || !res || !res.value) return;

    //   $this.package = res.value;
    //   $this.isShouldUpdate = utils.compareVersion($this.installedVersion, $this.package.version) == -1;
    //   var major = $this.package.version.split('.')[0];
    //   var minor = $this.package.version.split('.')[1];
    //   $this.updatesUrl = 'https://www.siteserver.cn/updates/v' + major + '_' + minor + '/index.html';
    // }, 'packages', packageId);
  },

  check: function () {
    this.isCheck = !this.isCheck;
  },

  updateSsCms: function () {
    if (!this.package) return;
    this.step = 2;
    var $this = this;

    $api.post($url, {
      version: $this.package.version
    }, function (err, res) {
      if (err) {
        $this.errorMessage = err.message;
      } else if (res) {
        $this.step = 3;
      }
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.load();
  }
});
