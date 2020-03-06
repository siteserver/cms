var $url = '/admin/shared/editorLayerWord';

var data = utils.initData({
  attributeName: utils.getQueryString('attributeName'),
  uploadList: [],
  form: {
    siteId: utils.getQueryInt('siteId'),
    isClearFormat: true,
    isFirstLineIndent: true,
    isClearFontSize: true,
    isClearFontFamily: true,
    isClearImages: false,
    fileNames: []
  },
  uploadUrl: null
});

var methods = {
  apiSubmit: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function(response) {
      var res = response.data;

      parent.$vue.insertEditor($this.attributeName, res.value);
      utils.closeLayer();
    })
    .catch(function(error) {
      utils.error($this, error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    if (this.form.fileNames.length === 0) {
      return this.$message.error('请选择需要导入的Word文件！');
    }

    this.apiSubmit();
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
    this.uploadUrl = $apiUrl + $url + '/actions/upload?siteId=' + this.form.siteId;
    utils.loading(this, false);
  }
});