var $url = '/cms/settings/settingsWaterMark';

var data = utils.init({
  siteId: utils.getQueryInt("siteId"),
  urlUpload: null,
  pageType: null,
  form: null,
  families: null,
  imageUrl: null
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.urlUpload = $apiUrl + $url + '/actions/upload?siteId=' + $this.siteId;

      $this.form = {
        siteId: $this.siteId,
        isWaterMark: res.site.isWaterMark,
        waterMarkPosition: res.site.waterMarkPosition,
        waterMarkTransparency: res.site.waterMarkTransparency,
        waterMarkMinWidth: res.site.waterMarkMinWidth,
        waterMarkMinHeight: res.site.waterMarkMinHeight,
        isImageWaterMark: res.site.isImageWaterMark,
        waterMarkFormatString: res.site.waterMarkFormatString,
        waterMarkFontName: res.site.waterMarkFontName,
        waterMarkFontSize: res.site.waterMarkFontSize,
        waterMarkImagePath: res.site.waterMarkImagePath,
      };
      $this.families = res.families;
      $this.imageUrl = res.imageUrl;
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function (response) {
      var res = response.data;

      utils.success('图片水印设置保存成功！');
    }).catch(function (error) {
      utils.error(error);
    }).then(function () {
      utils.loading($this, false);
    });
  },

  uploadBefore(file) {
    var re = /(\.jpg|\.jpeg|\.bmp|\.gif|\.png|\.webp)$/i;
    if(!re.exec(file.name))
    {
      utils.error('文件只能是图片格式，请选择有效的文件上传!');
      return false;
    }

    var isLt10M = file.size / 1024 / 1024 < 10;
    if (!isLt10M) {
      utils.error('上传图片大小不能超过 10MB!');
      return false;
    }
    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadSuccess: function(res, file) {
    utils.loading(this, false);
    this.imageUrl = res.imageUrl;
    this.form.waterMarkImagePath = res.virtualUrl;
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$refs.form.validate(function(valid) {
      if (valid) {
        $this.apiSubmit();
      }
    });
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});