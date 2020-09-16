var $url = '/cms/material/layerImageUpload';

var data = utils.init({
  uploadUrl: null,
  dialogImageUrl: '',
  dialogVisible: false,
  form: {
    siteId: utils.getQueryInt('siteId'),
    isEditor: false,
    isLibrary: true,
    isThumb: false,
    thumbWidth: 500,
    thumbHeight: 500,
    isLinkToOriginal: true,
    filePaths: []
  }
});

var methods = {
  parentInsert: function(no, result) {
    var vue = parent.$vue;
    if (vue.runMaterialLayerImageUploadText) {
      vue.runMaterialLayerImageUploadText(result.imageUrl);
    }
    if (vue.runMaterialLayerImageUploadEditor && this.form.isEditor) {
      var html = '<img src="' + result.imageUrl + '" style="border: 0; max-width: 100%" />';
      if (result.previewUrl) {
        var vueHtml = '<el-image src="' + result.imageUrl + '" style="border: 0; max-width: 100%"></el-image>';
        html = '<img data-vue="' + encodeURIComponent(vueHtml) + '" src="' + result.imageUrl + '" style="border: 0; max-width: 100%" />';
      }
      vue.runMaterialLayerImageUploadEditor(html);
    }
  },

  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.form.siteId
      }
    }).then(function(response) {
      var res = response.data;

      $this.form.isEditor = res.isEditor;
      $this.form.isLibrary = res.isLibrary;
      $this.form.isThumb = res.isThumb;
      $this.form.thumbWidth = res.thumbWidth;
      $this.form.thumbHeight = res.thumbHeight;
      $this.form.isLinkToOriginal = res.isLinkToOriginal;
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  apiSubmit: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function(response) {
      var res = response.data;

      if (res && res.length > 0) {
        for (var i = 0; i < res.length; i++) {
          var result = res[i];
          $this.parentInsert($this.no + i, result);
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

  btnSubmitClick: function () {
    if (this.form.filePaths.length === 0) {
      utils.error('请选择需要插入的图片文件！');
      return false;
    }

    this.apiSubmit();
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },

  uploadBefore(file) {
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
    this.uploadUrl = $apiUrl + $url + '/actions/upload?siteId=' + this.form.siteId;
    this.apiGet();
  }
});