var $url = '/common/editor/layerImage';

var data = utils.init({
  attributeName: utils.getQueryString('attributeName'),
  uploadList: [],
  dialogImageUrl: '',
  dialogVisible: false,
  form: {
    siteId: utils.getQueryInt('siteId'),
    isMaterial: true,
    isThumb: false,
    thumbWidth: 500,
    thumbHeight: 500,
    isLinkToOriginal: true,
    filePaths: []
  }
});

var methods = {
  insert: function(result) {
    if (parent.$vue.runEditorLayerImage) {
      var html = '<img src="' + result.imageUrl + '" style="border: 0; max-width: 100%" />';
      if (result.previewUrl) {
        var vueHtml = '<el-image src="' + result.imageUrl + '" style="border: 0; max-width: 100%"></el-image>';
        html = '<img data-vue="' + encodeURIComponent(vueHtml) + '" src="' + result.imageUrl + '" style="border: 0; max-width: 100%" />';
      }
      parent.$vue.runEditorLayerImage(this.attributeName, html);
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

      $this.form.isMaterial = res.isMaterial;
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
          $this.insert(result);
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