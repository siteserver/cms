var $url = '/pages/cms/libraryLayerWord';
var $urlUpload = apiUrl + '/pages/cms/libraryLayerWord/actions/upload?siteId=' + utils.getQueryInt('siteId');

var data = {
  pageLoad: false,
  pageAlert: null,
  uploadList: [],
  form: {
    siteId: utils.getQueryInt('siteId'),
    isClearFormat: true,
    isFirstLineIndent: true,
    isClearFontSize: true,
    isClearFontFamily: true,
    isClearImages: false,
    fileNames: []
  }
};

var methods = {
  btnSubmitClick: function () {
    var $this = this;

    if (this.form.fileNames.length === 0) {
      this.$message.error('请选择需要导入的Word文件！');
      return false;
    }

    utils.loading(true);
    $api.post($url, this.form).then(function(response) {
      var res = response.data;

      parent.insertHtml(res.value);
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
    var re = /(\.doc|\.docx|\.wps)$/i;
    if(!re.exec(file.name))
    {
      this.$message.error('文件只能是 Word 格式，请选择有效的文件上传!');
      return false;
    }
    return true;
  },

  uploadProgress: function() {
    utils.loading(true)
  },

  uploadRemove(file) {
    if (file.response) {
      this.form.fileNames.splice(this.form.fileNames.indexOf(file.response.name), 1);
    }
  },

  uploadSuccess: function(res) {
    this.form.fileNames.push(res.name);
    utils.loading(false);
  },

  uploadError: function(err) {
    utils.loading(false);
    var error = JSON.parse(err.message);
    this.$message.error(error.message);
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