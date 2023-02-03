var $url = "/clouds/backup"
var $urlRestore = "/clouds/backup/actions/restore";
var $urlGetRestoreProgress = "/clouds/backup/actions/getRestoreProgress";
var $urlRestart = '/clouds/backup/actions/restart';

var $urlDashboard = "/clouds/dashboard"
var $urlCloud = "cms/backup";
var $urlCloudRestore = "cms/backup/actions/restore";
var $urlCloudGetRestoreProgress = "cms/backup/actions/getRestoreProgress";

var data = utils.init({
  activeName: "settings",
  cloudType: null,
  isCloudBackup: false,
  backups: [],
  restoreBackup: null,
  restoreId: null,
  restoreProgress: 0,
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

  apiRestore: function () {
    var $this = this;

    $api.post($urlRestore, {
      backupId: this.restoreBackup.backupId,
    })
    .then(function (response) {
      var res = response.data;
      $this.restoreId = res.value;

      $this.apiGetRestoreProgress();
    })
    .catch(function (error) {
      utils.error(error);
    });
  },

  apiGetRestoreProgress: function () {
    var $this = this;

    $api.post($urlGetRestoreProgress, {
      restoreId: this.restoreId,
    })
    .then(function (response) {
      var res = response.data;

      $this.restoreProgress = res.value;
      if ($this.restoreProgress == 100) {
        utils.alertSuccess({
          title: '系统恢复成功',
          text: '恭喜，系统文件与数据已成功恢复到 “' + $this.restoreBackup.createdDate + '” 的备份版本，恢复后需要重启，请点击重启按钮重新启动系统！',
          button: '重启系统',
          callback: function () {
            $this.apiRestart();
          }
        });
      } else {
        setTimeout(function () {
          $this.apiGetRestoreProgress();
        }, 3000);
      }
    })
    .catch(function (error) {
      utils.error(error);
    });
  },

  apiRestart: function () {
    utils.loading(this, true);
    $api.post($urlRestart).then(function (response) {
      setTimeout(function() {
        utils.alertSuccess({
          title: '系统重启成功',
          text: '恭喜，系统重启成功',
          callback: function() {
            window.top.location.href = utils.getIndexUrl();
          }
        });
      }, 30000);
    }).catch(function (error) {
      utils.error(error);
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

  apiCloudGet: function () {
    var $this = this;

    cloud.get($urlCloud)
    .then(function (response) {
      var res = response.data;

      if ($this.cloudType !== res.cloudType) {
        $this.cloudType == res.cloudType;
        $this.apiDashboardSubmit(res.cloudType, res.expirationDate);
      }

      $this.backups = res.backups;
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

  btnSubmitClick: function () {
    this.apiSubmit();
  },

  btnUpgradeClick: function () {
    location.href = utils.getCloudsUrl('dashboard', {isUpgrade: true});
  },

  btnRestoreClick: function (backup) {
    var $this = this;

    utils.alertDelete({
      title: '系统恢复',
      text: '此操作将把系统的文件与数据恢复到 “' + backup.createdDate + '” 的备份版本，确定吗？',
      button: '确定恢复',
      callback: function () {
        $this.activeName = 'progress';
        $this.restoreBackup = backup;
        $this.restoreProgress = 0;
        $this.apiRestore();
      }
    });
  },

  btnCloseClick: function() {
    utils.removeTab();
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSubmitClick, this.btnCloseClick);
    var $this = this;
    cloud.checkAuth(function() {
      $this.apiGet();
    });
  }
});
