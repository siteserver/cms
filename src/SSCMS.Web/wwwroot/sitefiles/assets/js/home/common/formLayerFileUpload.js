var $url = '/common/form/layerFileUpload';

var data = utils.init({
  attributeName: utils.getQueryString('attributeName'),
  no: utils.getQueryInt('no'),
  uploadUrl: null,
  form: {
    siteId: utils.getQueryInt('siteId'),
    isChangeFileName: true,
    filePaths: []
  }
});

var methods = {
  insert: function(no, result) {
    if (parent.$vue.runFormLayerFileUpload) {
      parent.$vue.runFormLayerFileUpload(this.attributeName, no, result.fileVirtualUrl);
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

      $this.form.isChangeFileName = res.isChangeFileName;
      $this.uploadUrl = $apiUrl + $url + '/actions/upload?siteId=' + $this.form.siteId + '&isChangeFileName=' + $this.form.isChangeFileName;
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
          $this.insert($this.no + i, result);
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
      utils.error('请上传需要插入的附件文件！');
      return false;
    }

    this.apiSubmit();
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadFileSuccess: function(res) {
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