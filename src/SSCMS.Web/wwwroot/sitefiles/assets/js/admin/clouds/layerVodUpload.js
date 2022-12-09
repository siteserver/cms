var $urlCloud = 'cms/vod';

var data = utils.init({
  attributeName: utils.getQueryString('attributeName'),
  no: utils.getQueryInt('no'),
  uploadToken: null,
  uploadUrl: null,
  uploadErrorMessage: null,
  uploadProgressPercent: null,
  uploadProgressInterval: null,
});

var methods = {
  insert: function(result) {
    if (parent.$vue.runFormLayerVideoUpload) {
      parent.$vue.runFormLayerVideoUpload(this.attributeName, this.no, result.playUrl, result.coverUrl);
    }
  },

  uploadBefore: function(file) {
    this.uploadErrorMessage = '';
    var re = /(\.3gp|\.asf|\.avi|\.dat|\.dv|\.flv|\.f4v|\.gif|\.m2t|\.m4v|\.mj2|\.mjpeg|\.mkv|\.mov|\.mp4|\.mpe|\.mpg|\.mpeg|\.mts|\.ogg|\.qt|\.rm|\.rmvb|\.swf|\.ts|\.vob|\.wmv|\.webm)$/i;
    if(!re.exec(file.name))
    {
      this.uploadErrorMessage = '请选择有效的文件上传!';
      return false;
    }
    return true;
  },

  uploadRequest: function(data) {
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
    cloud.post(this.uploadUrl, formData, config)
    .then(function (response) {
      var res = response.data;

      $this.insert(res);
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
    utils.loading(this, false);
    this.uploadToken = $cloudToken;
    this.uploadUrl = cloud.defaults.baseURL + '/' + $urlCloud;
  }
});
