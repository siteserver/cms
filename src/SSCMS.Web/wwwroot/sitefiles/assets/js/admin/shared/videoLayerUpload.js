var $url = '/shared/videoLayerUpload';

var data = utils.init({
  siteId: utils.getQueryInt('siteId'),
  attributeName: utils.getQueryString('attributeName'),
  no: utils.getQueryInt('no'),
  files: [],
  uploadUrl: null
});

var methods = {
  btnSubmitClick: function () {
    if (!this.files.length === 0) {
      utils.error('请上传需要插入的视频文件！');
      return false;
    }

    for (var i = 0; i < this.files.length; i++) {
      var file = this.files[i];
      parent.$vue.insertText(this.attributeName, this.no + i, file.fileUrl);
    }

    utils.closeLayer();
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadFileSuccess: function(res) {
    this.files.push({
      fileName: res.name,
      fileUrl: res.url,
    });

    utils.loading(this, false);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },

  uploadRemove(file) {
    if (file.response) {
      var index = _.findIndex(this.files, function(o) { return o.fileName === file.response.name; });
      this.files.splice(index, 1);
    }
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.uploadUrl = $apiUrl + $url + '/actions/upload?siteId=' + this.siteId;
    utils.loading(this, false);
  }
});