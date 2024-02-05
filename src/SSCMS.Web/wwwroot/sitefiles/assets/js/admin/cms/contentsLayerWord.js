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
    fileNames: [],
    fileUrls: [],
  },
  uploadUrl: null,
  uploadList: [],
  orderedFileNames: [],
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

      $this.checkedLevels = res.checkedLevels;
      $this.form.checkedLevel = res.checkedLevel;
    }).catch(function(error) {
      utils.error(error);
    }).then(function() {
      utils.loading($this, false);
    });
  },

  apiSubmit: function() {
    var $this = this;

    var fileNames = [];
    var fileUrls = [];
    for (var i = 0; i < this.orderedFileNames.length; i++) {
      var fileName = this.orderedFileNames[i];
      var index = this.form.fileNames.indexOf(fileName);
      if (index !== -1) {
        fileNames.push(this.form.fileNames[index]);
        fileUrls.push(this.form.fileUrls[index]);
      }
    }
    for (var i = 0; i < this.form.fileNames.length; i++) {
      var fileName = this.form.fileNames[i];
      var index = this.orderedFileNames.indexOf(fileName);
      if (index === -1) {
        fileNames.push(this.form.fileNames[i]);
        fileUrls.push(this.form.fileUrls[i]);
      }
    }
    fileNames.reverse();
    fileUrls.reverse();

    utils.loading(this, true);
    $api.post($url, _.assign({}, this.form, {
      fileNames: fileNames,
      fileUrls: fileUrls,
    })).then(function(response) {
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
    var re = /(\.docx)$/i;
    if(!re.exec(file.name)) {
      utils.error('文件只能是以.docx结尾的 Word 格式，请选择有效的文件上传!');
      return false;
    }
    this.orderedFileNames.push(file.name);
    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadRemove(file) {
    if (file.response) {
      var index = this.form.fileNames.indexOf(file.response.name);
      this.form.fileNames.splice(index, 1);
      this.form.fileUrls.splice(index, 1);

      var selectedIndex = this.orderedFileNames.indexOf(file.response.name);
      this.orderedFileNames.splice(selectedIndex, 1);
    }
  },

  uploadSuccess: function(res) {
    this.form.fileNames.push(res.fileName);
    this.form.fileUrls.push(res.fileUrl);
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
    utils.keyPress(this.btnSubmitClick, this.btnCancelClick);
    this.apiGet();
    this.uploadUrl = $apiUrl + $url + '/actions/upload?siteId=' + this.form.siteId + '&channelId=' + this.form.channelId;
  }
});
