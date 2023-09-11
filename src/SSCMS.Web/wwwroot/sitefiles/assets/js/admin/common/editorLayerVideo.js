var $url = '/common/editor/layerVideo';
var $urlCloud = 'cms/vod';

var data = utils.init({
  attributeName: utils.getQueryString('attributeName'),
  rootUrl: null,
  siteUrl: null,
  isCloudVod: false,
  form: {
    siteId: utils.getQueryInt('siteId'),
    type: 'upload',
    videoUrl: '',
    isPoster: false,
    isAutoPlay: false,
    isWidth: false,
    isHeight: false,
    imageUrl: '',
    width: '100%',
    height: '500px',
    isLinkToOriginal: true,
  },
  uploadVideoUrl: null,
  uploadImageUrl: null,

  cloudUploadToken: null,
  cloudUploadUrl: null,
  cloudUploadErrorMessage: null,
  cloudUploadProgressPercent: null,
  cloudUploadProgressInterval: null,
});

var methods = {
  apiGet: function() {
    var $this = this;

    utils.loading(this, true);
    $api.get($url, {
      params: {
        siteId: this.form.siteId
      }
    }).then(function(response) {
      var res = response.data;

      $this.rootUrl = res.rootUrl;
      $this.siteUrl = res.siteUrl;
      $this.isCloudVod = res.isCloudVod;
    })
    .catch(function(error) {
      utils.error(error);
    })
    .then(function() {
      utils.loading($this, false);
    });
  },

  getPreviewVideoUrl: function(videoUrl) {
    return utils.getUrl(this.siteUrl, videoUrl);
  },

  btnSubmitClick: function () {
    var $this = this;

    if (!this.form.videoUrl) {
      utils.error('请上传需要插入的视频文件！');
      return false;
    }

    var imageUrl = this.form.isPoster && this.form.imageUrl ? ' imageUrl="' + this.form.imageUrl + '"' : '';
    var isAutoPlay = ' isAutoPlay="' + this.form.isAutoPlay + '"';
    var width = this.form.isWidth ? ' width="' + this.form.width + '"' : '';
    var height = this.form.isHeight ? ' height="' + this.form.height + '"' : '';

    parent.$vue.insertEditor($this.attributeName, '<img src="/sitefiles/assets/images/video-clip.png" style="width: 333px; height: 333px"' + imageUrl + isAutoPlay + width + height + ' playUrl="' + this.form.videoUrl + '" class="siteserver-stl-player" /><br/>');
    utils.closeLayer();
  },

  btnCancelClick: function () {
    utils.closeLayer();
  },

  uploadVideoBefore(file) {
    var re = /(\.mp4|\.flv|\.f4v|\.webm|\.m4v|\.mov|\.3gp|\.3g2)$/i;
    if(!re.exec(file.name))
    {
      utils.error('文件只能是视频格式，请选择有效的文件上传!');
      return false;
    }
    return true;
  },

  uploadImageBefore(file) {
    var re = /(\.jpg|\.jpeg|\.bmp|\.gif|\.png|\.webp)$/i;
    if(!re.exec(file.name))
    {
      utils.error('文件只能是图片格式，请选择有效的文件上传!');
      return false;
    }

    var isLt10M = file.size / 1024 / 1024 < 10;
    if (!isLt10M) {
      utils.error('上传图片大小不能超过 10MB!');
      return false;
    }
    return true;
  },

  uploadProgress: function() {
    utils.loading(this, true);
  },

  uploadVideoSuccess: function(res) {
    this.form.videoUrl = res.url;
    if (res.coverUrl) {
      this.form.isPoster = true;
      this.form.imageUrl = res.coverUrl;
    }
    this.form.type = 'url';
    utils.loading(this, false);
  },

  uploadImageSuccess: function(res) {
    this.form.imageUrl = res.url;
    utils.loading(this, false);
  },

  uploadError: function(err) {
    utils.loading(this, false);
    var error = JSON.parse(err.message);
    utils.error(error.message);
  },

  cloudUploadBefore: function(file) {
    this.cloudUploadErrorMessage = '';
    var re = /(\.3gp|\.asf|\.avi|\.dat|\.dv|\.flv|\.f4v|\.gif|\.m2t|\.m4v|\.mj2|\.mjpeg|\.mkv|\.mov|\.mp4|\.mpe|\.mpg|\.mpeg|\.mts|\.ogg|\.qt|\.rm|\.rmvb|\.swf|\.ts|\.vob|\.wmv|\.webm)$/i;
    if(!re.exec(file.name))
    {
      this.cloudUploadErrorMessage = '请选择有效的文件上传!';
      return false;
    }
    return true;
  },

  cloudUploadRequest: function(data) {
    var $this = this;
    var formData = new FormData()
    formData.append('file', data.file)
    var config = {
      onUploadProgress: function(progressEvent) {
        $this.cloudUploadProgressPercent = Number((progressEvent.loaded / progressEvent.total * 30).toFixed(2));
        if (progressEvent.loaded === progressEvent.total) {
          $this.cloudUploadProgressInterval = setInterval(function() {
            $this.cloudUploadProgressPercent += 1;
            if ($this.cloudUploadProgressPercent === 99) {
              clearInterval($this.cloudUploadProgressInterval);
            }
          }, 1000);
        }
      }
    };
    cloud.post(this.cloudUploadUrl, formData, config)
    .then(function (response) {
      var res = response.data;

      $this.form.videoUrl = res.playUrl;
      if (res.coverUrl) {
        $this.form.isPoster = true;
        $this.form.imageUrl = res.coverUrl;
      }
      $this.form.type = 'url';
    })
    .catch(function (error) {
      $this.cloudUploadProgressPercent = null;
      $this.cloudUploadErrorMessage = utils.getErrorMessage(error);
    })
    .then(function () {
      clearInterval($this.cloudUploadProgressInterval);
    });
  },
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    utils.keyPress(this.btnSubmitClick, this.btnCancelClick);
    this.uploadVideoUrl = $apiUrl + $url + '/actions/uploadVideo?siteId=' + this.form.siteId;
    this.uploadImageUrl = $apiUrl + $url + '/actions/uploadImage?siteId=' + this.form.siteId;
    this.cloudUploadToken = $cloudToken;
    this.cloudUploadUrl = cloud.defaults.baseURL + '/' + $urlCloud;
    this.apiGet();
  }
});
