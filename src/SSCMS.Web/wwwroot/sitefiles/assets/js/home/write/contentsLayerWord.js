var $url = '/write/contentsLayerWord';

var data = utils.init({
  page: utils.getQueryInt('page'),
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
    files: []
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

    utils.loading(this, true);
    $api.post($url, this.form).then(function(response) {
      var res = response.data;

      utils.success('Word导入成功！');
      if (parent.$vue.apiList) {
        parent.$vue.apiList($this.page);
      } else {
        parent.location.href = utils.getRootUrl('write/contents', {
          siteId: $this.siteId,
          channelId: $this.channelId,
          page: 1
        });
      }
      
      utils.closeLayer();
    }).catch(function(error) {
      utils.error(error);
    }).then(function() {
      utils.loading($this, false);
    });
  },

  btnSubmitClick: function () {
    if (this.form.files.length === 0) {
      return utils.error('请选择需要导入的Word文件！');
    }

    this.apiSubmit();
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
    this.apiGet();
    this.uploadUrl = $apiUrl + $url + '/actions/upload?siteId=' + this.form.siteId + '&channelId=' + this.form.channelId;
  }
});