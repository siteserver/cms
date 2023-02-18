var $url = "/clouds/images";
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
  cloudType: null,
  isCloudImages: false,
  imageData: [],
  dialogVisible: false,
  image: null,
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
        $this.isCloudImages = res.isCloudImages;
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
        isCloudImages: this.isCloudImages,
      })
      .then(function (response) {
        var res = response.data;

        utils.success("免版权图库设置保存成功！");
      })
      .catch(function (error) {
        utils.error(error);
      })
      .then(function () {
        utils.loading($this, false);
      });
  },

  checkCloudType: function() {
    if (!this.cloudType || this.cloudType == 'Free') {
      alert({
        title: '免版权图库',
        text: '系统检测到您的云助手版本为免费版，使用免版权图库设置功能请升级云助手版本！',
        type: 'warning',
        confirmButtonText: '关 闭',
        showConfirmButton: true,
        showCancelButton: false,
        buttonsStyling: false,
      });
      return true;
    }
    return false;
  },

  btnSubmitClick: function () {
    if (this.checkCloudType()) return;

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
    cloud.get($urlCloud, {
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
      utils.error(error, {
        ignoreAuth: true,
      });
    })
    .then(function () {
      utils.loading($this, false);
    });
  },

  btnSearchClick: function () {
    this.count = 0;
    this.images = [];
    this.apiCloudGet();
  },

  isMore: function () {
    return this.count > this.formInline.page * this.formInline.perPage;
  },

  btnMoreClick: function() {
    this.formInline.page++;
    this.apiCloudGet();
  },

  btnImageClick: function(image) {
    this.image = image;
    this.imageData = [{
      type: 'thumb',
      size: '200 x ' + (this.image.height * (200 / this.image.width)).toFixed(0),
    }, {
      type: 'small',
      size: '400 x ' + (this.image.height * (400 / this.image.width)).toFixed(0),
    }, {
      type: 'regular',
      size: '1080 x ' + (this.image.height * (1080 / this.image.width)).toFixed(0),
    }];
    this.dialogVisible = true;
  },

  getImageType: function(type) {
    if (type == 'thumb') {
      return '小尺寸';
    } else if (type == 'small') {
      return '中尺寸';
    } else if (type == 'regular') {
      return '大尺寸';
    }
    return '';
  },

  getImageUrl: function (type) {
    var url = this.image.regularUrl;
    if (type == 'thumb') {
      url = this.image.thumbUrl;
    } else if (type == 'small') {
      url = this.image.smallUrl;
    } else if (type == 'regular') {
      url = this.image.regularUrl;
    } else if (type == 'full') {
      url = this.image.fullUrl;
    }
    return cloud.hostImages + '/' + url;
  },

  btnViewClick: function(type) {
    window.open(this.getImageUrl(type));
  },

  btnCopyClick: function(type) {
    var url = this.getImageUrl(type);
    navigator.clipboard.writeText(url);
    utils.success('图片地址已复制！')
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
    cloud.checkAuth(function () {
      $this.apiGet();
    });
  },
});
