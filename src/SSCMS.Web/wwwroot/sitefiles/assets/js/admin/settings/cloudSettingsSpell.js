var $url = "/settings/cloudSettingsSpell"

var data = utils.init({
  isCloudSpellingCheck: false,
  isCloudSpellingCheckAuto: false,
  isCloudSpellingCheckIgnore: false,
  isCloudSpellingCheckWhiteList: false
});

var methods = {
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url).then(function (response) {
      var res = response.data;

      $this.isCloudSpellingCheck = res.isCloudSpellingCheck;
      $this.isCloudSpellingCheckAuto = res.isCloudSpellingCheckAuto;
      $this.isCloudSpellingCheckIgnore = res.isCloudSpellingCheckIgnore;
      $this.isCloudSpellingCheckWhiteList = res.isCloudSpellingCheckWhiteList;
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
      isCloudSpellingCheck: this.isCloudSpellingCheck,
      isCloudSpellingCheckAuto: this.isCloudSpellingCheckAuto,
      IsCloudSpellingCheckIgnore: this.isCloudSpellingCheckIgnore,
      IsCloudSpellingCheckWhiteList: this.isCloudSpellingCheckWhiteList,
    }).then(function (response) {
      var res = response.data;

      utils.success('错别字检查设置保存成功！');
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
