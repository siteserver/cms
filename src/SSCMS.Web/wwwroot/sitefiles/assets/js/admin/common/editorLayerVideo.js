var $url = '/common/editor/layerVideo';

var data = utils.init({
  attributeName: utils.getQueryString('attributeName'),
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
  uploadVideoUrl: null,
  uploadImageUrl: null
});

var methods = {
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

    parent.$vue.insertEditor($this.attributeName, '<img src="/sitefiles/assets/images/video-clip.png"' + imageUrl + isAutoPlay + width + height + ' playUrl="' + this.form.videoUrl + '" class="siteserver-stl-player" style="width: 333px; height: 333px" /><br/>');
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
    this.uploadVideoUrl = $apiUrl + $url + '/actions/uploadVideo?siteId=' + this.form.siteId;
    this.uploadImageUrl = $apiUrl + $url + '/actions/uploadImage?siteId=' + this.form.siteId;
    utils.loading(this, false);
  }
});