var $url = "/settings/cloudSettingsSms"

var data = utils.init({
  isCloudSms: false,
  isCloudSmsAdministrator: false,
  isCloudSmsUser: false,
});

var methods = {
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.isCloudSms = res.isCloudSms;
      $this.isCloudSmsAdministrator = res.isCloudSmsAdministrator;
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
      isCloudSmsAdministrator: this.isCloudSmsAdministrator,
      isCloudSmsUser: this.isCloudSmsUser,
    }).then(function (response) {
      var res = response.data;

      utils.success('短信发送设置保存成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    this.apiSubmit();
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
