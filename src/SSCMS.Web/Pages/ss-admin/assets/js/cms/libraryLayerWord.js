var $url = '/admin/cms/library/libraryLayerWord';
var $urlUpload = $apiUrl + '/admin/cms/library/libraryLayerWord/actions/upload?siteId=' + utils.getQueryInt('siteId');

var data = utils.initData({
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
});

var methods = {
  btnSubmitClick: function () {
    var $this = this;

    if (this.form.fileNames.length === 0) {
      this.$message.error('请选择需要导入的Word文件！');
      return false;
    }

    utils.loading(this, true);
    $api.post($url, this.form).then(function(response) {
      var res = response.data;

      parent.$vue.insertHtml(res.value);
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
    var re = /(\.doc|\.docx|\.wps)$/i;
    if(!re.exec(file.name))
    {
      this.$message.error('文件只能是 Word 格式，请选择有效的文件上传!');
      return false;
    }
    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadRemove(file) {
    if (file.response) {
      this.form.fileNames.splice(this.form.fileNames.indexOf(file.response.name), 1);
    }
  },

  uploadSuccess: function(res) {
    this.form.fileNames.push(res.name);
    utils.loading(this, false);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    this.$message.error(error.message);
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