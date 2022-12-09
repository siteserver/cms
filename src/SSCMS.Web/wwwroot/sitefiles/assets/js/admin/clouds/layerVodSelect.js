var $urlCloud = 'cms/vod';

var data = utils.init({
  inputType: utils.getQueryString('inputType'),
  attributeName: utils.getQueryString('attributeName'),
  no: utils.getQueryInt('no'),

  count: 0,
  videos: [],
  formInline: {
    keyword: "",
    currentPage: 1,
    offset: 0,
    limit: 30,
  },
});

var methods = {
  insert: function(vod) {
    if (this.inputType === 'Video') {
      if (parent.$vue.runFormLayerVideoUpload) {
        parent.$vue.runFormLayerVideoUpload(this.attributeName, this.no, vod.playUrl, vod.coverUrl);
      }
    } else if (this.inputType === 'TextEditor') {
      parent.$vue.insertEditor(this.attributeName, '<img src="/sitefiles/assets/images/video-clip.png" style="width: 333px; height: 333px" imageUrl="' + vod.coverUrl + '"' + ' playUrl="' + vod.playUrl + '" class="siteserver-stl-player" /><br/>');
    }
  },

  apiCloudGet: function () {
    var $this = this;

    utils.loading(this, true);
    cloud.get($urlCloud, {
      params: this.formInline,
    })
    .then(function (response) {
      var res = response.data;

      $this.count = res.count;
      $this.videos = res.videos;
    })
    .catch(function (error) {
      utils.error(error, {
        ignoreAuth: true,
      });
    })
    .then(function () {
      utils.loading($this, false);
    });
  },

  btnSearchClick: function () {
    this.apiCloudGet();
  },

  handleCurrentChange: function (val) {
    this.formInline.currentPage = val;
    this.formInline.offset = this.formInline.limit * (val - 1);

    this.btnSearchClick();
  },

  btnViewClick: function (video) {
    utils.openLayer({
      title: "预览视频",
      url: utils.getCommonUrl("editorLayerPreviewVideo", {
        videoUrl: video.playUrl
      }),
      width: 600,
      height: 500,
    });
  },

  getSize: function(vod) {
    return (vod.size / 1024 / 1024).toFixed(2);
  },

  getDuration: function(vod) {
    return new Date(parseInt(vod.duration) * 1000).toISOString().slice(14, 19);
  },

  btnSelectClick: function(vod) {
    this.insert(vod);
    utils.closeLayer();
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
    var $this = this;
    cloud.checkAuth(function () {
      $this.apiCloudGet();
    });
  }
});
