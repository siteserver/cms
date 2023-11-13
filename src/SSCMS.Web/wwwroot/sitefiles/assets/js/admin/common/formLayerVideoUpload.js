var $url = '/common/form/layerVideoUpload';

var data = utils.init({
  attributeName: utils.getQueryString('attributeName'),
  no: utils.getQueryInt('no'),
  uploadUrl: null,
  uploadErrorMessage: null,
  uploadProgressPercent: null,
  uploadProgressInterval: null,
  form: {
    siteId: utils.getQueryInt('siteId'),
    isChangeFileName: true,
    isLibrary: true,
  },
  reExtensions: null,
  showExtensions: null,
});

var methods = {
  insert: function(no, result) {
    if (parent.$vue.runFormLayerVideoUpload) {
      parent.$vue.runFormLayerVideoUpload(this.attributeName, no, result.virtualUrl, result.coverUrl);
    }
  },

  uploadBefore: function(file) {
    if(!this.reExtensions.exec(file.name))
    {
      this.uploadErrorMessage = '请选择有效的文件上传!';
      return false;
    }
    return true;
  },

  uploadRequest: function(data) {
    this.uploadUrl = $url + '?siteId=' + this.form.siteId + '&isChangeFileName=' + this.form.isChangeFileName + '&isLibrary=' + this.form.isLibrary;

    var $this = this;
    var formData = new FormData()
    formData.append('file', data.file)
    var config = {
      onUploadProgress: function(progressEvent) {
        $this.uploadProgressPercent = Number((progressEvent.loaded / progressEvent.total * 30).toFixed(2));
        if (progressEvent.loaded === progressEvent.total) {
          $this.uploadProgressInterval = setInterval(function() {
            $this.uploadProgressPercent += 1;
            if ($this.uploadProgressPercent === 99) {
              clearInterval($this.uploadProgressInterval);
            }
          }, 1000);
        }
      }
    };
    $api.post(this.uploadUrl, formData, config)
    .then(function (response) {
      var res = response.data;

      $this.insert($this.no, res);
      utils.closeLayer();
    })
    .catch(function (error) {
      $this.uploadProgressPercent = null;
      $this.uploadErrorMessage = utils.getErrorMessage(error);
    })
    .then(function () {
      clearInterval($this.uploadProgressInterval);
    });
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

      if (res.isCloudVod) {
        location.href = utils.getCloudsUrl('layerVodUpload', {
          attributeName: $this.attributeName,
          no: $this.no,
        });
      }

      $this.form.isChangeFileName = res.isChangeFileName;
      $this.form.isLibrary = res.isLibrary;
      $this.reExtensions = new RegExp('(' + res.videoUploadExtensions.replace(/,/g, '|').replace(/\./g, '\\.') + ')$', 'i');
      $this.showExtensions = res.videoUploadExtensions.replace(/,/g, '、').toUpperCase();
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
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(null, this.btnCancelClick);
    this.apiGet();
  }
});
