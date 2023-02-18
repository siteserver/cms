var $url = "/clouds/sms"

var data = utils.init({
  isCloudSms: false,
  isCloudSmsAdmin: false,
  isCloudSmsAdminAndDisableAccount: false,
  isCloudSmsUser: false,
});

var methods = {
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.isCloudSms = res.isCloudSms;
      $this.isCloudSmsAdmin = res.isCloudSmsAdmin;
      $this.isCloudSmsAdminAndDisableAccount = res.isCloudSmsAdminAndDisableAccount;
      $this.isCloudSmsUser = res.isCloudSmsUser;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, {
      isCloudSms: this.isCloudSms,
      isCloudSmsAdmin: this.isCloudSmsAdmin,
      isCloudSmsAdminAndDisableAccount: this.isCloudSmsAdminAndDisableAccount,
      isCloudSmsUser: this.isCloudSmsUser,
    }).then(function (response) {
      var res = response.data;

      utils.success('短信集成设置保存成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    this.apiSubmit();
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
