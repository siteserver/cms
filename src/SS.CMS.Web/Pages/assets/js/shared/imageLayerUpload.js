var $url = '/admin/shared/imageLayerUpload';

var data = utils.initData({
  attributeName: utils.getQueryString('attributeName'),
  no: utils.getQueryInt('no'),
  editorAttributeName: utils.getQueryString('editorAttributeName'),
  uploadUrl: null,
  dialogImageUrl: '',
  dialogVisible: false,
  form: {
    siteId: utils.getQueryInt('siteId'),
    isEditor: false,
    isThumb: false,
    thumbWidth: 500,
    thumbHeight: 500,
    isLinkToOriginal: true,
    filePaths: []
  }
});

var methods = {
  insert: function(no, result) {
    parent.$vue.insertText(this.attributeName, no, result.imageUrl);
    if (this.editorAttributeName && result.isEditor) {
      var html = '<img src="' + result.imageUrl + '" style="border: 0; max-width: 100%" />';
      if (result.previewUrl) {
        var vueHtml = '<el-image src="' + result.imageUrl + '" style="border: 0; max-width: 100%"></el-image>';
        html = '<img data-vue="' + encodeURIComponent(vueHtml) + '" src="' + result.imageUrl + '" style="border: 0; max-width: 100%" />';
      }
      parent.$vue.insertEditor(this.editorAttributeName, html);
    }
  },

  btnSubmitClick: function () {
    var $this = this;

    if (this.form.filePaths.length === 0) {
      this.$message.error('请选择需要插入的图片文件！');
      return false;
    }

    utils.loading(this, true);
    $api.post($url, this.form).then(function(response) {
      var res = response.data;

      if (res && res.length > 0) {
        for (var i = 0; i < res.length; i++) {
          var result = res[i];
          $this.insert($this.no + i, result);
        }
      }
      
      utils.closeLayer();
    })
    .catch(function(error) {
      utils.error($this, error);
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
    utils.loading(this, true);
  },

  uploadSuccess: function(res) {
    this.form.filePaths.push(res.path);
    utils.loading(this, false);
  },

  uploadError: function(err) {
    utils.loading(this, false);
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

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.uploadUrl = $apiUrl + $url + '/actions/upload?siteId=' + this.form.siteId;
    utils.loading(this, false);
  }
});