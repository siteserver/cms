var $url = "/clouds/settingsCdn";

var data = utils.init({
  cloudType: null,
  isCloudCdn: false,
  isCloudCdnImages: false,
  isCloudCdnFiles: false,
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api
      .get($url)
      .then(function (response) {
        var res = response.data;

        $this.cloudType = res.cloudType;
        $this.isCloudCdn = res.isCloudCdn;
        $this.isCloudCdnImages = res.isCloudCdnImages;
        $this.isCloudCdnFiles = res.isCloudCdnFiles;
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api
      .post($url, {
        isCloudCdn: this.isCloudCdn,
        isCloudCdnImages: this.isCloudCdnImages,
        isCloudCdnFiles: this.isCloudCdnFiles,
      })
      .then(function (response) {
        var res = response.data;

        utils.success("CDN设置保存成功！");
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
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

  btnUpgradeClick: function () {
    location.href = utils.getCloudsUrl('dashboard', {isUpgrade: true});
  },
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    var $this = this;
    cloud.checkAuth(function () {
      $this.apiGet();
    });
  },
});
