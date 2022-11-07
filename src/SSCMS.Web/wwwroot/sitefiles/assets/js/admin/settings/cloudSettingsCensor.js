var $url = "/settings/cloudSettingsCensor"

var data = utils.init({
  isCloudCensorText: false,
  isCloudCensorTextAuto: false,
  isCloudCensorTextIgnore: false,
  isCloudCensorTextWhiteList: false
});

var methods = {
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.isCloudCensorText = res.isCloudCensorText;
      $this.isCloudCensorTextAuto = res.isCloudCensorTextAuto;
      $this.isCloudCensorTextIgnore = res.isCloudCensorTextIgnore;
      $this.isCloudCensorTextWhiteList = res.isCloudCensorTextWhiteList;
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
      isCloudCensorText: this.isCloudCensorText,
      isCloudCensorTextAuto: this.isCloudCensorTextAuto,
      IsCloudCensorTextIgnore: this.isCloudCensorTextIgnore,
      IsCloudCensorTextWhiteList: this.isCloudCensorTextWhiteList,
    }).then(function (response) {
      var res = response.data;

      utils.success('内容违规检测设置保存成功！');
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
