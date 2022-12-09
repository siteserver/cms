var $url = "/clouds/settingsBackup"
var $urlDashboard = "/clouds/dashboard"
var $urlCloud = "cms/backup";

var data = utils.init({
  activeName: "settings",
  cloudType: null,
  isCloudBackup: false,
  backupJobs: [],
});

var methods = {
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.cloudType = res.cloudType;
      $this.isCloudBackup = res.isCloudBackup;
      $this.apiCloudGet();
    }).catch(function (error) {
      utils.error(error);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      isCloudBackup: this.isCloudBackup,
    }).then(function (response) {
      var res = response.data;

      utils.success('云备份设置保存成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiDashboardSubmit: function (cloudType, expirationDate) {
    $api
      .post($urlDashboard, {
        cloudType: cloudType,
        expirationDate: expirationDate,
      })
      .then(function (response) {
        var res = response.data;
      })
      .catch(function (error) {
        utils.error(error);
      });
  },

  btnSubmitClick: function () {
    this.apiSubmit();
  },

  apiCloudGet: function () {
    var $this = this;

    cloud.get($urlCloud)
    .then(function (response) {
      var res = response.data;

      if ($this.cloudType !== res.cloudType) {
        $this.cloudType == res.cloudType;
        $this.apiDashboardSubmit(res.cloudType, res.expirationDate);
      }

      $this.backupJobs = res.backupJobs;
    })
    .catch(function (error) {
      utils.error(error, {
        ignoreAuth: true,
      });
    })
    .then(function () {
      utils.loading($this, false);
    });
  },

  btnUpgradeClick: function () {
    location.href = utils.getCloudsUrl('dashboard', {isUpgrade: true});
  },

  btnRestoreClick: function (backup) {
    console.log(backup);
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    var $this = this;
    cloud.checkAuth(function() {
      $this.apiGet();
    });
  }
});
