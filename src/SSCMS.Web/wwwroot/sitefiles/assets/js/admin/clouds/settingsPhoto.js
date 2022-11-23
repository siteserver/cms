var $url = "/clouds/settingsPhoto";
var $urlCloud = "cms/images";

var data = utils.init({
  activeName: "settings",
  count: 0,
  images: [],
  formInline: {
    keyword: "",
    page: 1,
    perPage: 28,
  },
  isCloudPhoto: false,
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api
      .get($url)
      .then(function (response) {
        var res = response.data;

        $this.isCloudPhoto = res.isCloudPhoto;
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
        isCloudPhoto: this.isCloudPhoto,
      })
      .then(function (response) {
        var res = response.data;

        utils.success("视频点播设置保存成功！");
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  btnSubmitClick: function () {
    this.apiSubmit();
  },

  btnTabsClick: function () {
    if (this.activeName == "images") {
      this.apiCloudGet();
    }
  },

  getSmallUrl: function (image) {
    return cloud.hostImages + '/' + image.smallUrl;
  },

  getRegularUrl: function (image) {
    return cloud.hostImages + '/' + image.regularUrl;
  },

  apiCloudGet: function () {
    var $this = this;

    utils.loading(this, true);
    cloud
      .get($urlCloud, {
        params: this.formInline,
      })
      .then(function (response) {
        var res = response.data;

        $this.count = res.count;
        for (var image of res.images) {
          $this.images.push(image);
        }
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  btnSearchClick: function () {
    this.apiCloudGet();
  },

  btnMoreClick: function() {
    this.formInline.page++;
    this.apiCloudGet();
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
