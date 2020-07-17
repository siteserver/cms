var $url = '/common/library/layerImage';
var $urlUpload = $apiUrl + '/common/library/layerImage/actions/upload?siteId=' + utils.getQueryInt('siteId');

var data = utils.init({
  uploadList: [],
  dialogImageUrl: '',
  dialogVisible: false,
  form: {
    siteId: utils.getQueryInt('siteId'),
    isThumb: false,
    thumbWidth: 500,
    thumbHeight: 500,
    isLinkToOriginal: true,
    filePaths: []
  }
});

var methods = {
  btnSubmitClick: function () {
    var $this = this;

    if (this.form.filePaths.length === 0) {
      utils.error('请选择需要插入的图片文件！');
      return false;
    }

    utils.loading(this, true);
    $api.post($url, this.form).then(function(response) {
      var res = response.data;

      if (res && res.length > 0) {
        for (var i = 0; i < res.length; i++) {
          var result = res[i];
          if (result.thumbUrl) {
            parent.$vue.insertHtml('<a href="' + result.url + '" target="_blank"><img src="' + result.thumbUrl + '" border="0" /></a><br/>');
          } else {
            parent.$vue.insertHtml('<img src="' + result.url + '" border="0" /><br/>');
          }
        }
      }
      
      utils.closeLayer();
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  btnCancelClick: function () {
    utils.closeLayer();
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

  uploadSuccess: function(res) {
    this.form.filePaths.push(res.path);
    utils.loading(this, false);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },

  uploadRemove(file) {
    if (file.response) {
      this.form.filePaths.splice(this.form.filePaths.indexOf(file.response.path), 1);
    }
  },

  uploadPictureCardPreview(file) {
    this.dialogImageUrl = file.url;
    this.dialogVisible = true;
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.pageLoad = true;
  }
});