var $urlUploadVideo = $apiUrl + '/common/material/layerVideo/actions/uploadVideo?siteId=' + utils.getQueryInt('siteId');
var $urlUploadImage = $apiUrl + '/common/material/layerVideo/actions/uploadImage?siteId=' + utils.getQueryInt('siteId');

var data = utils.init({
  activeName: 'first',
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
    height: '300px',
    isLinkToOriginal: true,
  },
  player: null
});

var methods = {
  btnTabsClick: function() {
    if (this.activeName !== 'second') return;
    if (this.player) {
      this.player.poster(this.form.isPoster ? this.form.imageUrl : '');
      this.player.src([{src: this.form.videoUrl}]);
    } else {
      this.player = videojs(this.$refs.videoPlayer, {
        poster: this.form.isPoster ? this.form.imageUrl : '',
        sources: [
          {
            src: this.form.videoUrl
          }
        ]
      });
    }
  },

  btnSubmitClick: function () {
    var $this = this;

    if (!this.form.videoUrl) {
      utils.error('请设置需要插入的视频文件！');
      return false;
    }

    var imageUrl = this.form.isPoster && this.form.imageUrl ? ' imageUrl="' + this.form.imageUrl + '"' : '';
    var isAutoPlay = ' isAutoPlay="' + this.form.isAutoPlay + '"';
    var width = this.form.isWidth ? ' width="' + this.form.width + '"' : '';
    var height = this.form.isHeight ? ' height="' + this.form.height + '"' : '';
    var clipUrl = utils.getAssetsUrl('images/video-clip.png');

    parent.$vue.insertHtml('<img ' + imageUrl + isAutoPlay + width + height + ' playUrl="' + this.form.videoUrl + '" class="siteserver-stl-player" style="width: 333px; height: 333px" src="' + clipUrl + '" /><br/>');
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
  }
};

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.pageLoad = true;
  }
});