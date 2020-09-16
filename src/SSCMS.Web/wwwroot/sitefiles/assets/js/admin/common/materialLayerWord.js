var $url = '/common/material/layerWord';
var $urlUpload = $apiUrl + '/common/material/layerWord/actions/upload?siteId=' + utils.getQueryInt('siteId');

var data = utils.init({
  uploadList: [],
  form: {
    siteId: utils.getQueryInt('siteId'),
    isClearFormat: true,
    isFirstLineIndent: true,
    isClearFontSize: true,
    isClearFontFamily: true,
    isClearImages: false,
    files: []
  }
});

var methods = {
  btnSubmitClick: function () {
    var $this = this;

    if (this.form.files.length === 0) {
      utils.error('请选择需要导入的Word文件！');
      return false;
    }

    utils.loading(this, true);
    $api.post($url, this.form).then(function(response) {
      var res = response.data;

      parent.$vue.insertHtml(res.value);
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
    var re = /(\.docx)$/i;
    if(!re.exec(file.name))
    {
      utils.error('文件只能是以.docx结尾的 Word 格式，请选择有效的文件上传!');
      return false;
    }
    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadRemove(file) {
    if (file.response) {
      var startIndex = this.form.files.findIndex(function(x) {
        return x.fileName == file.response.name;
      });
      this.form.files.splice(startIndex, 1);
    }
  },

  uploadSuccess: function(res) {
    this.form.files.push({
      fileName: res.fileName,
      title: res.title
    });
    utils.loading(this, false);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
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