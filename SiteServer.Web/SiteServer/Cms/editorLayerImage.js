var $url = '/pages/cms/editorLayerImage';
var $urlUpload = apiUrl + '/pages/cms/editorLayerImage/actions/upload?siteId=' + utils.getQueryInt('siteId') + '&channelId=' + utils.getQueryInt('channelId');

var data = {
  attributeName: utils.getQueryString('attributeName'),
  pageLoad: false,
  pageAlert: null,
  uploadList: [],
  dialogImageUrl: '',
  dialogVisible: false,
  form: {
    siteId: utils.getQueryInt('siteId'),
    channelId: utils.getQueryInt('channelId'),
    isThumb: false,
    thumbWidth: 500,
    thumbHeight: 500,
    isLinkToOriginal: true,
    filePaths: []
  }
};

var methods = {
  insert: function(result) {
    var html = '<img src="' + result.imageUrl + '" style="border: 0; max-width: 100%" />';
    if (result.previewUrl) {
      var vueHtml = '<el-image src="' + result.imageUrl + '" style="border: 0; max-width: 100%"></el-image>';
      html = '<img data-vue="' + encodeURIComponent(vueHtml) + '" src="' + result.imageUrl + '" style="border: 0; max-width: 100%" />';
    }
    parent.insertHtml(this.attributeName, html);
  },

  btnSubmitClick: function () {
    var $this = this;

    if (this.form.filePaths.length === 0) {
      this.$message.error('请选择需要插入的图片文件！');
      return false;
    }

    utils.loading(true);
    $api.post($url, this.form).then(function(response) {
      var res = response.data;

      if (res && res.length > 0) {
        for (var i = 0; i < res.length; i++) {
          var result = res[i];
          $this.insert(result);
          // if (result.vueType && result.vueValue) {
          //   parent.insertHtml($this.attributeName, '<img data-vue-type="' + result.vueType + '" data-vue-value="' + result.vueValue + '" src="' + result.url + '" style="border: 0; max-width: 100%" /><br/>');
          // } else {
          //   parent.insertHtml($this.attributeName, '<img src="' + result.url + '" style="border: 0; max-width: 100%" /><br/>');
          // }
        }
      }
      
      parent.layer.closeAll();
    })
    .catch(function(error) {
      $this.pageAlert = utils.getPageAlert(error);
    })
    .then(function() {
      utils.loading(false);
    });
  },

  btnCancelClick: function () {
    parent.layer.closeAll();
  },

  uploadBefore(file) {
    var re = /(\.jpg|\.jpeg|\.bmp|\.gif|\.png|\.webp)$/i;
    if(!re.exec(file.name))
    {
      this.$message.error('文件只能是图片格式，请选择有效的文件上传!');
      return false;
    }

    var isLt10M = file.size / 1024 / 1024 < 10;
    if (!isLt10M) {
      this.$message.error('上传图片大小不能超过 10MB!');
      return false;
    }
    return true;
  },

  uploadProgress: function() {
    utils.loading(true)
  },

  uploadSuccess: function(res) {
    this.form.filePaths.push(res.path);
    utils.loading(false);
  },

  uploadError: function(err) {
    utils.loading(false);
    var error = JSON.parse(err.message);
    this.$message.error(error.message);
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

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.pageLoad = true;
  }
});