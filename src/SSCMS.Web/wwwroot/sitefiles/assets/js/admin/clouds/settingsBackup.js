var $url = "/clouds/settingsBackup"
var $urlDashboard = "/clouds/dashboard"
var $urlCloud = "cms/backup";
var $urlCloudRestore = "cms/backup/actions/restore";
var $urlCloudGetRestoreProgress = "cms/backup/actions/getRestoreProgress";

var data = utils.init({
  activeName: "settings",
  cloudType: null,
  isCloudBackup: false,
  snapshots: [],
  restoreSnapshot: null,
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

      $this.snapshots = res.snapshots;
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

  apiCloudRestore: function () {
    var $this = this;

    cloud.post($urlCloudRestore, {
      snapshotId: this.restoreSnapshot.snapshotId,
      snapshotHash: this.restoreSnapshot.snapshotHash,
    })
    .then(function (response) {
      var res = response.data;

      $this.restoreId = res.value;
      $this.apiCloudGetRestoreProgress();
    })
    .catch(function (error) {
      utils.error(error, {
        ignoreAuth: true,
      });
    });
  },

  apiCloudGetRestoreProgress: function () {
    var $this = this;

    cloud.post($urlCloudGetRestoreProgress, {
      restoreId: this.restoreId,
    })
    .then(function (response) {
      var res = response.data;

      $this.restoreProgress = res.value;
      if ($this.restoreProgress == 100) {
        utils.alertSuccess({
          title: '系统恢复成功',
          text: '恭喜，系统文件与数据已成功恢复到 “' + $this.restoreSnapshot.completeDate + '” 的备份版本！',
          callback: function () {
            window.top.location.href = utils.getIndexUrl();
          }
        });
      } else {
        setTimeout(function () {
          $this.apiCloudRestore();
        }, 3000);
      }
    })
    .catch(function (error) {
      utils.error(error, {
        ignoreAuth: true,
      });
    });
  },

  btnSubmitClick: function () {
    this.apiSubmit();
  },

  btnUpgradeClick: function () {
    location.href = utils.getCloudsUrl('dashboard', {isUpgrade: true});
  },

  btnRestoreClick: function (snapshot) {
    var $this = this;

    utils.alertDelete({
      title: '系统恢复',
      text: '此操作将把系统的文件与数据恢复到 “' + snapshot.completeDate + '” 的备份版本，确定吗？',
      button: '确定恢复',
      callback: function () {
        $this.activeName = 'progress';
        $this.restoreSnapshot = snapshot;
        $this.restoreProgress = 0;
        // $this.apiCloudRestore();
        $this.apiCloudGetRestoreProgress();
      }
    });
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
