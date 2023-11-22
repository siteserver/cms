var $url = '/cms/material/layerVideoUpload';

var data = utils.init({
  uploadUrl: null,
  uploadErrorMessage: null,
  uploadProgressPercent: null,
  uploadProgressInterval: null,
  form: {
    siteId: utils.getQueryInt('siteId'),
    groupId: utils.getQueryInt('groupId'),
  },
  reExtensions: null,
  showExtensions: null,
});

var methods = {
  uploadBefore: function(file) {
    if(!this.reExtensions.exec(file.name))
    {
      this.uploadErrorMessage = '请选择有效的文件上传!';
      return false;
    }
    return true;
  },

  uploadRequest: function(data) {
    this.uploadUrl = $url + '?siteId=' + this.form.siteId + '&groupId=' + this.form.groupId;

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

      utils.success('视频上传成功！');
      parent.$vue.apiGet(1);
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
