var $url = '/common/editor/layerFile';

var data = utils.init({
  attributeName: utils.getQueryString('attributeName'),
  form: {
    siteId: utils.getQueryInt('siteId'),
    type: 'upload',
    files: [],
    fileName: '',
    fileUrl: '',
    isAutoPlay: false
  },
  uploadUrl: null
});

var methods = {
  btnSubmitClick: function () {
    if (!this.form.files.length === 0) {
      utils.error('请上传需要插入的附件文件！');
      return false;
    }

    for (var i = 0; i < this.form.files.length; i++) {
      var file = this.form.files[i];
      parent.$vue.insertEditor(this.attributeName, '<a href="' + file.fileUrl + '" target="_blank">' + file.fileName + '</a><br />');
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
    this.form.files.push({
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
      var index = _.findIndex(this.form.files, function(o) { return o.fileName === file.response.name; });
      this.form.files.splice(index, 1);
    }
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