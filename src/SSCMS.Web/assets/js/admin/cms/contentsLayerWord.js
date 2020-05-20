var $url = '/cms/contents/contentsLayerWord';

var data = utils.init({
  checkedLevels: null,
  form: {
    siteId: utils.getQueryInt('siteId'),
    channelId: utils.getQueryInt('channelId'),
    isFirstLineTitle: false,
    isFirstLineRemove: true,
    isClearFormat: true,
    isFirstLineIndent: true,
    isClearFontSize: true,
    isClearFontFamily: true,
    isClearImages: false,
    checkedLevel: null,
    fileNames: []
  },
  uploadUrl: null,
  uploadList: []
});

var methods = {
  apiGet: function () {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.form.siteId,
        channelId: this.form.channelId
      }
    }).then(function(response) {
      var res = response.data;

      $this.checkedLevels = res.value;
      $this.form.checkedLevel = res.checkedLevel;

      $this.uploadUrl = $apiUrl + $url + '/actions/upload?siteId=' + $this.form.siteId + '&channelId=' + $this.form.channelId;
    }).catch(function(error) {
      utils.error(error);
    }).then(function() {
      utils.loading($this, false);
    });
  },

  apiSubmit: function() {
    var $this = this;

    utils.loading(this, true);
    $api.post($url, this.form).then(function(response) {
      var res = response.data;

      utils.closeLayer();
      parent.$vue.apiList($this.form.channelId, 1, 'Word导入成功！', true);
    }).catch(function(error) {
      utils.error(error);
    }).then(function() {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    if (this.form.fileNames.length === 0) {
      return utils.error('请选择需要导入的Word文件！');
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
      utils.error('文件只能是 Word 格式，请选择有效的文件上传!');
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
    utils.error(error.message);
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